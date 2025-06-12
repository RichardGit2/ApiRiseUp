FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RiseUpAPI.csproj", "./"]
RUN dotnet restore "RiseUpAPI.csproj"
COPY . .
RUN dotnet build "RiseUpAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RiseUpAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "RiseUpAPI.dll"] 