FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HMS.Api/HMS.Api.csproj", "HMS.Api/"]
COPY ["HMS.Application/HMS.Application.csproj", "HMS.Application/"]
COPY ["HMS.Domain/HMS.Domain.csproj", "HMS.Domain/"]
COPY ["HMS.Infrastructure/HMS.Infrastructure.csproj", "HMS.Infrastructure/"]
COPY ["HMS.Common/HMS.Common.csproj", "HMS.Common/"]

RUN dotnet restore "HMS.Api/HMS.Api.csproj"

COPY . .
WORKDIR "/src/HMS.Api"
RUN dotnet build "HMS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HMS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HMS.Api.dll"]

