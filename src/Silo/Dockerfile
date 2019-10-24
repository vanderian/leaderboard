FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

COPY src/Silo/*.csproj ./src/Silo/
WORKDIR /app/src/Silo
RUN dotnet restore

WORKDIR /app/src
COPY src/. .
WORKDIR /app/src/Silo
RUN dotnet publish -c Release -o out

#FROM build AS testrunner
#WORKDIR /app/tests
#COPY tests/. .
#WORKDIR /app/tests/Grains.Tests
#RUN dotnet build
#ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM mcr.microsoft.com/dotnet/core/runtime:3.0 AS runtime
ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
WORKDIR /app
COPY --from=build /app/src/Silo/out ./
ENTRYPOINT ["dotnet", "Silo.dll"]