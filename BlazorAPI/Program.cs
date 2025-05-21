using BlazorAPI.Extensions;
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
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace BlazorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Registra o contexto do banco de dados com SQLite, usando a string de conexão do appsettings.json.
            builder.Services.AddDbContext<BlazorAPIBancodbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //Pegando o link na appsettings
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

            //Permitir interacao com a aplicacao blazor
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirBlazor", policy =>
                {
                    policy.WithOrigins(allowedOrigins) // Porta do seu app Blazor
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            ////Permitir interacao com a aplicacao blazor
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("PermitirBlazor", policy =>
            //    {
            //        policy.WithOrigins("https://localhost:7170") // Porta do seu app Blazor
            //              .AllowAnyHeader()
            //              .AllowAnyMethod();
            //    });
            //});

            //Limitação de Taxa (Rate Limiting) - impedir que seja feita varias requisições em um curto periodo
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("ApiPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 50, // 50 requisições
                            Window = TimeSpan.FromMinutes(1), // por minuto
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 10
                        }));
            });

            // Configuração do cache
            //builder.Services.AddStackExchangeRedisCache(options =>
            //{
            //    // Configuração segura usando variáveis de ambiente
            //    options.Configuration = builder.Configuration.GetConnectionString("Redis") ??
            //                          builder.Configuration["REDIS_CONNECTION_STRING"];
            //    options.InstanceName = "BlazorAPI_";
            //});

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            //Adicionando as injeções de dependencias
            builder.Services.AdicionarInjecoesDependencias();

            //Swagger
            builder.Services.AdicionarConfiguracaoSwagger();

            //JWT
            builder.Services.AdicionarConfiguracaoJwtEF(builder.Configuration);

            var app = builder.Build();

            //Permitir interacao com a aplicacao blazor
            app.UseCors("PermitirBlazor");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}