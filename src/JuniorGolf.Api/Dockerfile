FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /src
COPY ["src/JuniorGolf.Api/JuniorGolf.Api.csproj", "src/JuniorGolf.Api/"]
COPY ["src/JuniorGolf.Core/JuniorGolf.Core.csproj", "src/JuniorGolf.Core/"]
COPY ["src/JuniorGolf.Infrastructure/JuniorGolf.Infrastructure.csproj", "src/JuniorGolf.Infrastructure/"]
COPY ["src/JuniorGolf.AI/JuniorGolf.AI.csproj", "src/JuniorGolf.AI/"]
COPY ["src/JuniorGolf.Shared/JuniorGolf.Shared.csproj", "src/JuniorGolf.Shared/"]
RUN dotnet restore "src/JuniorGolf.Api/JuniorGolf.Api.csproj"
COPY . .
WORKDIR "/src/src/JuniorGolf.Api"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JuniorGolf.Api.dll"]
