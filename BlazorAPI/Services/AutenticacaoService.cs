using BlazorAPI.Interfaces.Autenticacao;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorAPI.Services
{
    public class AutenticacaoService : IAutenticacao
    {
        private readonly IConfiguration configuration;

        public AutenticacaoService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public string GenerateToken(string _login, string _senha)
        {
            var claims = new[]
            {
                new Claim("login", _login),
                new Claim("senha", _senha),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var chavePrivada = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:secretKey"]));

            var credenciais = new SigningCredentials(chavePrivada, SecurityAlgorithms.HmacSha256);

            var expiracao = DateTime.UtcNow.AddSeconds(30);
            //var expiracao = DateTime.UtcNow.AddMinutes(10);

            JwtSecurityToken token = new JwtSecurityToken(

                issuer: configuration["jwt:issuer"],
                audience: configuration["jwt:audience"],
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais

            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}