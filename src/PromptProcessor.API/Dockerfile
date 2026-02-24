FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/PromptProcessor.API/PromptProcessor.API.csproj", "src/PromptProcessor.API/"]
COPY ["src/PromptProcessor.Domain/PromptProcessor.Domain.csproj", "src/PromptProcessor.Domain/"]
COPY ["src/PromptProcessor.Infrastructure/PromptProcessor.Infrastructure.csproj", "src/PromptProcessor.Infrastructure/"]
RUN dotnet restore "src/PromptProcessor.API/PromptProcessor.API.csproj"
COPY . .
WORKDIR "/src/src/PromptProcessor.API"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PromptProcessor.API.dll"]