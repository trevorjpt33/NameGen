# Stage 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first for layer caching
COPY NameGen.sln .
COPY src/NameGen.API/NameGen.API.csproj src/NameGen.API/
COPY src/NameGen.Core/NameGen.Core.csproj src/NameGen.Core/
COPY src/NameGen.Infrastructure/NameGen.Infrastructure.csproj src/NameGen.Infrastructure/
COPY src/NameGen.Web/NameGen.Web.csproj src/NameGen.Web/

# Restore dependencies
RUN dotnet restore

# Copy remaining source files
COPY src/ src/

# Publish the API
RUN dotnet publish src/NameGen.API/NameGen.API.csproj -c Release -o /app/publish

# Stage 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "NameGen.API.dll"]