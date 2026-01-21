# Stage 1: Build the application
FROM ://mcr.microsoft.com AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["PortfolioMichael.csproj", "."]
RUN dotnet restore "./PortfolioMichael.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "PortfolioMichael.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run the application
FROM ://mcr.microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the port Render expects (default is 10000)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "PortfolioMichael.dll"]
