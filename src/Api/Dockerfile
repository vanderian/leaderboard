FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

COPY src/Api/*.csproj ./src/Api/
WORKDIR /app/src/Api
RUN dotnet restore

WORKDIR /app/src
COPY src/. .
WORKDIR /app/src/Api
RUN dotnet publish  -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
WORKDIR /app
COPY --from=build /app/src/Api/out ./
ENTRYPOINT ["dotnet", "Api.dll"]