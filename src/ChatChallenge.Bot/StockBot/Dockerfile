#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChatChallenge.Bot/StockBot/ChatChallenge.StockBot.csproj", "ChatChallenge.Bot/StockBot/"]
COPY ["ChatChallenge.Common/Contracts/ChatChallenge.Contracts.csproj", "ChatChallenge.Common/Contracts/"]
COPY ["ChatChallenge.Bot/BotCommandValidator/ChatChallenge.BotCommandValidator.csproj", "ChatChallenge.Bot/BotCommandValidator/"]
RUN dotnet restore "ChatChallenge.Bot/StockBot/ChatChallenge.StockBot.csproj"
COPY . .
WORKDIR "/src/ChatChallenge.Bot/StockBot"
RUN dotnet build "ChatChallenge.StockBot.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "ChatChallenge.StockBot.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatChallenge.StockBot.dll"]