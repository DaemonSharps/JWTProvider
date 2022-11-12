#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# сборка docker build -t daemonsharps/jwt-auth-api:dev .

# запуск docker run -d -p 3000:80 --rm --name test -e DOTNET_ENVIRONMENT='Development' daemonsharps/jwt-auth-api:dev
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/JWTProvider/JWTProvider.csproj", "src/JWTProvider/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/JWTProvider/JWTProvider.csproj"
COPY . .
WORKDIR "/src/src/JWTProvider"
RUN dotnet build "JWTProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JWTProvider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JWTProvider.dll"]
