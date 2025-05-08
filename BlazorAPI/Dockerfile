FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copia primeiro apenas o .csproj
COPY BlazorAPI/BlazorAPI.csproj .
RUN dotnet restore

# Depois copia todo o resto
COPY BlazorAPI/ .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "BlazorAPI.dll", "--urls", "http://0.0.0.0:5000"]

# Copia o banco para a pasta de execução
COPY Banco.db ./Banco.db
ENTRYPOINT ["dotnet", "BlazorAPI.dll", "--urls", "http://0.0.0.0:5000"]