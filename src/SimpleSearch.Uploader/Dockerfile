#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SimpleSearch.Uploader/SimpleSearch.Uploader.csproj", "SimpleSearch.Uploader/"]
COPY ["SimpleSearch.EventBus/SimpleSearch.EventBus.csproj", "SimpleSearch.EventBus/"]
COPY ["SimpleSearch.Messages/SimpleSearch.Messages.csproj", "SimpleSearch.Messages/"]
COPY ["SimpleSearch.Storage.Blobs/SimpleSearch.Storage.Blobs.csproj", "SimpleSearch.Storage.Blobs/"]
COPY ["SimpleSearch.Storage.DocumentDb/SimpleSearch.Storage.DocumentDb.csproj", "SimpleSearch.Storage.DocumentDb/"]
RUN dotnet restore "SimpleSearch.Uploader/SimpleSearch.Uploader.csproj"
COPY . .
WORKDIR "/src/SimpleSearch.Uploader"
RUN dotnet build "SimpleSearch.Uploader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleSearch.Uploader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleSearch.Uploader.dll"]