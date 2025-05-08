FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY BlazorAPI/BlazorAPI.csproj .
RUN dotnet restore
COPY BlazorAPI/ .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
COPY BlazorAPI/Banco.db ./Banco.db  # Copia o banco para a pasta de execução

# Adicione esta linha para dar permissões ao arquivo:
RUN chmod 777 ./Banco.db

ENTRYPOINT ["dotnet", "BlazorAPI.dll", "--urls", "http://0.0.0.0:5000"]