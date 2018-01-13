FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY EventSourcing/EventSourcingMiddleware/EventSourcingMiddleware.csproj EventSourcing/EventSourcingMiddleware/
COPY DomainModels/DomainModels.csproj DomainModels/
COPY Components/BackgroundMiddleware/BackgroundMiddleware.csproj Components/BackgroundMiddleware/
COPY Components/BuildingAspects/BuildingAspects.csproj Components/BuildingAspects/
RUN dotnet restore
COPY . .
WORKDIR /src/EventSourcing/EventSourcingMiddleware
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EventSourcingMiddleware.dll"]
