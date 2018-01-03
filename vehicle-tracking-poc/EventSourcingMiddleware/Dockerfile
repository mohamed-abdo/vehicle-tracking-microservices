FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY EventSourcingMiddleware/EventSourcingMiddleware.csproj EventSourcingMiddleware/
COPY Components/BackgroundMiddleware/BackgroundMiddleware.csproj Components/BackgroundMiddleware/
COPY DomainModels/DomainModels.csproj DomainModels/
COPY Components/BuildingAspects/BuildingAspects.csproj Components/BuildingAspects/
RUN dotnet restore
COPY . .
WORKDIR /src/EventSourcingMiddleware
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EventSourcingMiddleware.dll"]
