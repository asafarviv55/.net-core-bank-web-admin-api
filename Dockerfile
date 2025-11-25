FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebAdminAPI/WebAdminAPI.csproj", "WebAdminAPI/"]
RUN dotnet restore "WebAdminAPI/WebAdminAPI.csproj"
COPY . .
WORKDIR "/src/WebAdminAPI"
RUN dotnet build "WebAdminAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAdminAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAdminAPI.dll"]
