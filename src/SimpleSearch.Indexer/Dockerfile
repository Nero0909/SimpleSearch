#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SimpleSearch.Indexer/SimpleSearch.Indexer.csproj", "SimpleSearch.Indexer/"]
COPY ["SimpleSearch.Indexer.Shared/SimpleSearch.Indexer.Shared.csproj", "SimpleSearch.Indexer.Shared/"]
COPY ["SimpleSearch.Storage.DocumentDb/SimpleSearch.Storage.DocumentDb.csproj", "SimpleSearch.Storage.DocumentDb/"]
RUN dotnet restore "SimpleSearch.Indexer/SimpleSearch.Indexer.csproj"
COPY . .
WORKDIR "/src/SimpleSearch.Indexer"
RUN dotnet build "SimpleSearch.Indexer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleSearch.Indexer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleSearch.Indexer.dll"]