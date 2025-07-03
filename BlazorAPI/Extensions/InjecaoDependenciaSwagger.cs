using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BlazorAPI.Extensions
{
    public static class InjecaoDependenciaSwagger
    {
        public static IServiceCollection AdicionarConfiguracaoSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "OrgaTask API",
                    Description = "API RESTful desenvolvida em .NET 8. " +
                                  "Inclui autenticação JWT, operações CRUD e validações customizadas."
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,

                    Description = "Descrição geral da API\n\n" +
                                  "## Autenticação\n\n" +
                                  "🔒 **Autenticação JWT**\n\n" +
                                  "Para acessar endpoints protegidos:\n" +
                                  "1. Obtenha um token em `/Usuarios/login`\n" +
                                  "2. Adicione no header:\n\n" +
                                  "```\n" +
                                  "Authorization: Bearer {token}\n" +
                                  "```\n\n" +
                                  "⚠️ **Atenção**:\n\n" +
                                  "Tokens expiram após 10 minutos\n\n" +
                                  "Tokens expirados retornam status `401 Unauthorized`",
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return service;
        }
    }
}