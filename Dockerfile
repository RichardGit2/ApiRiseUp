FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RiseUpAPI.csproj", "./"]
RUN dotnet restore "RiseUpAPI.csproj"
COPY . .
RUN dotnet build "RiseUpAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RiseUpAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RiseUpAPI.dll"]