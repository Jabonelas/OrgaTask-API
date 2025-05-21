using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Repository.Tarefa;
using BlazorAPI.Interfaces.Repository.Usuario;
using BlazorAPI.Interfaces.Service.Cache;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Interfaces.Service.Usuario;
using BlazorAPI.Models;
using BlazorAPI.Repository;
using BlazorAPI.Services.Autenticacao;
using BlazorAPI.Services.Cache;
using BlazorAPI.Services.Tarefa;
using BlazorAPI.Services.Usuario;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlazorAPI.Extensions
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AdicionarInjecoesDependencias(this IServiceCollection service)
        {
            //Usuario
            service.AddScoped<IUsuarioService, UsuarioService>();
            service.AddScoped<IUsuarioRepository, UsuarioRepository>();

            //Tarefa
            service.AddScoped<ITarefaService, TarefaService>();
            service.AddScoped<ITarefaRepository, TarefaRepository>();

            //Cache
            service.AddScoped<ITarefaCacheService, TarefaCacheService>();

            //Autenticacao
            service.AddScoped<IAutenticacao, AutenticacaoService>();

            return service;
        }
    }
}