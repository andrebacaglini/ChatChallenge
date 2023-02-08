#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChatChallenge.Bot/ChatChallenge.StockBot/ChatChallenge.StockBot.csproj", "ChatChallenge.Bot/ChatChallenge.StockBot/"]
COPY ["ChatChallenge.Common/ChatChallenge.Contracts/ChatChallenge.Contracts.csproj", "ChatChallenge.Common/ChatChallenge.Contracts/"]
COPY ["ChatChallenge.Bot/ChatChallenge.BotCommandValidator/ChatChallenge.BotCommandValidator.csproj", "ChatChallenge.Bot/ChatChallenge.BotCommandValidator/"]
RUN dotnet restore "ChatChallenge.Bot/ChatChallenge.StockBot/ChatChallenge.StockBot.csproj"
COPY . .
WORKDIR "/src/ChatChallenge.Bot/ChatChallenge.StockBot"
RUN dotnet build "ChatChallenge.StockBot.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "ChatChallenge.StockBot.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatChallenge.StockBot.dll"]