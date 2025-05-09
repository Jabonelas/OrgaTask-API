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

        public string GenerateToken(int _idUsuario, string _login)
        {
            var claims = new[]
   {
        new Claim("idUsuario", _idUsuario.ToString()), // Claim com o ID do usuário
        new Claim(ClaimTypes.Name, _login), // Claim padrão para o nome
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único do token
    };

            var chavePrivada = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:secretKey"]));
            var credenciais = new SigningCredentials(chavePrivada, SecurityAlgorithms.HmacSha256);

            var expiracao = DateTime.UtcNow.AddMinutes(60);

            var token = new JwtSecurityToken(
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