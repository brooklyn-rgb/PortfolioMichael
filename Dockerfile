# 1. Build Stage
FROM ://mcr.microsoft.com AS build
WORKDIR /src
COPY ["PortfolioMichael.csproj", "."]
RUN dotnet restore "./PortfolioMichael.csproj"
COPY . .
RUN dotnet publish "PortfolioMichael.csproj" -c Release -o /app/publish

# 2. Runtime Stage
FROM ://mcr.microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the port Render expects for 2026 web services
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "PortfolioMichael.dll"]
