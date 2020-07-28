FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./src/*.csproj ./src
RUN dotnet restore src

# Copy everything else and build
COPY . ./
RUN dotnet publish src -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY --from=build-env /app/src/out .
ENTRYPOINT ["dotnet", "ddb-challenge.dll"]