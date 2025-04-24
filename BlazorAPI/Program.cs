using BlazorAPI.Extensions;
using BlazorAPI.Interfaces.Repository;
using BlazorAPI.Interfaces.Service;
using BlazorAPI.Models;
using BlazorAPI.Repository;
using BlazorAPI.Services;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            //Usuario
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            //Swagger
            builder.Services.AdicionarConfiguracaoSwagger();

            var app = builder.Build();

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