using BlazorAPI.Interfaces.Repository.Usuario;
using BlazorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAPI.Repository.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BlazorAPIBancodbContext context;

        public UsuarioRepository(BlazorAPIBancodbContext _context)
        {
            context = _context;
        }

        public async Task AdicionarAsync(TbUsuario _dadosCadastroUsuario)
        {
            await context.TbUsuarios.AddAsync(_dadosCadastroUsuario);
        }

        public async Task<bool> LoginExisteAsync(string _login)
        {
            return await context.TbUsuarios.AnyAsync(x => x.UsLogin == _login);
        }

        public async Task<int> BuscarIdUsuarioAsync(string _login)
        {
            return await context.TbUsuarios
            .Where(x => x.UsLogin == _login)
            .Select(x => x.IdUsuario)
            .FirstOrDefaultAsync();
        }

        public async Task<bool> LoginSenhaValidosAsync(TbUsuario _dadosUsuarioLogin)
        {
            return await context.TbUsuarios.AnyAsync(x => x.UsLogin == _dadosUsuarioLogin.UsLogin && x.UsSenha == _dadosUsuarioLogin.UsSenha);
        }
    }
}