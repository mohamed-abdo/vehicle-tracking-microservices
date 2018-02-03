FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Ping/Ping.csproj Ping/
COPY Components/BackgroundMiddleware/BackgroundMiddleware.csproj Components/BackgroundMiddleware/
COPY DomainModels/DomainModels.csproj DomainModels/
COPY Components/BuildingAspects/BuildingAspects.csproj Components/BuildingAspects/
COPY Components/WebComponents/WebComponents.csproj Components/WebComponents/
RUN dotnet restore
COPY . .
WORKDIR /src/Ping
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ping.dll"]
