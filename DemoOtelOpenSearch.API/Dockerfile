FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DemoOtelOpenSearch.API/DemoOtelOpenSearch.API.csproj", "DemoOtelOpenSearch.API/"]
RUN dotnet restore "DemoOtelOpenSearch.API/DemoOtelOpenSearch.API.csproj"
COPY . .
WORKDIR "/src/DemoOtelOpenSearch.API"
RUN dotnet build "DemoOtelOpenSearch.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoOtelOpenSearch.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoOtelOpenSearch.API.dll"]
