FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

ENV ASPNETCORE_URLS=http://*:10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PortfolioMichael.csproj", "./"]
RUN dotnet restore "PortfolioMichael.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PortfolioMichael.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PortfolioMichael.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PortfolioMichael.dll"]