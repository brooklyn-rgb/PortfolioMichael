# 1. Build Stage
FROM ://mcr.microsoft.com AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["PortfolioMichael.csproj", "./"]
RUN dotnet restore "PortfolioMichael.csproj"

# Copy the rest of the code and build
COPY . .
RUN dotnet publish "PortfolioMichael.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime Stage
FROM ://mcr.microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set Port for Render (Required)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "PortfolioMichael.dll"]
