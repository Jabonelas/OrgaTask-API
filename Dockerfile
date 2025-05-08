FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
# Copia o banco para a pasta de execução
COPY Banco.db ./Banco.db
ENTRYPOINT ["dotnet", "BlazorAPI.dll", "--urls", "http://0.0.0.0:5000"]