FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY ["src/ProgresiumToDo.API/ProgresiumToDo.API.csproj", "src/ProgresiumToDo.API/"]
COPY ["src/ProgresiumToDo.Infrastructure/ProgresiumToDo.Infrastructure.csproj", "src/ProgresiumToDo.Infrastructure/"]
COPY ["src/ProgresiumToDo.Application/ProgresiumToDo.Application.csproj", "src/ProgresiumToDo.Application/"]
COPY ["src/ProgresiumToDo.Domain/ProgresiumToDo.Domain.csproj", "src/ProgresiumToDo.Domain/"]
RUN dotnet restore "src/ProgresiumToDo.API/ProgresiumToDo.API.csproj"
COPY . .
WORKDIR "/src/src/ProgresiumToDo.API"
RUN dotnet build "./ProgresiumToDo.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProgresiumToDo.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProgresiumToDo.API.dll"]
