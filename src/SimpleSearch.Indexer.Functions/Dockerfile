#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SimpleSearch.Indexer.Functions/SimpleSearch.Indexer.Functions.csproj", "SimpleSearch.Indexer.Functions/"]
COPY ["SimpleSearch.Messages/SimpleSearch.Messages.csproj", "SimpleSearch.Messages/"]
COPY ["SimpleSearch.Storage.DocumentDb/SimpleSearch.Storage.DocumentDb.csproj", "SimpleSearch.Storage.DocumentDb/"]
RUN dotnet restore "SimpleSearch.Indexer.Functions/SimpleSearch.Indexer.Functions.csproj"
COPY . .
WORKDIR "/src/SimpleSearch.Indexer.Functions"
RUN dotnet build "SimpleSearch.Indexer.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleSearch.Indexer.Functions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleSearch.Indexer.Functions.dll"]