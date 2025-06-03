﻿using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Repository.Tarefa;
using BlazorAPI.Interfaces.Repository.Usuario;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Interfaces.Service.Usuario;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Repository;
using BlazorAPI.Services.Autenticacao;
using BlazorAPI.Services.Tarefa;
using BlazorAPI.Services.Usuario;
using BlazorAPI.Unit_O_fWork;

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

            //Unit Of Work
            service.AddScoped<IUnitOfWork, UnitOfWork>();

            //Cache
            //service.AddScoped<ITarefaCacheService, TarefaCacheService>();

            //Autenticacao
            service.AddScoped<IAutenticacao, AutenticacaoService>();

            return service;
        }
    }
}