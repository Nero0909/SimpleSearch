#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SimpleSearch.Analyzer.Functions/SimpleSearch.Analyzer.Functions.csproj", "SimpleSearch.Analyzer.Functions/"]
COPY ["SimpleSearch.Messages/SimpleSearch.Messages.csproj", "SimpleSearch.Messages/"]
RUN dotnet restore "SimpleSearch.Analyzer.Functions/SimpleSearch.Analyzer.Functions.csproj"
COPY . .
WORKDIR "/src/SimpleSearch.Analyzer.Functions"
RUN dotnet build "SimpleSearch.Analyzer.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleSearch.Analyzer.Functions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleSearch.Analyzer.Functions.dll"]