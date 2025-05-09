FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlazorAPI/BlazorAPI.csproj", "BlazorAPI/"]
RUN dotnet restore "BlazorAPI/BlazorAPI.csproj"
COPY . .
WORKDIR "/src/BlazorAPI"
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
# CORREÇÃO AQUI - especifique o destino completo:
COPY ["BlazorAPI/Banco.db", "./Banco.db"]
RUN chmod 644 ./Banco.db
ENTRYPOINT ["dotnet", "BlazorAPI.dll", "--urls", "http://0.0.0.0:5000"]