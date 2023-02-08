#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChatChallenge.Web/ChatChallenge/ChatChallenge.WebApp.csproj", "ChatChallenge.Web/ChatChallenge/"]
COPY ["ChatChallenge.Common/Contracts/ChatChallenge.Contracts.csproj", "ChatChallenge.Common/Contracts/"]
RUN dotnet restore "ChatChallenge.Web/ChatChallenge/ChatChallenge.WebApp.csproj"
COPY . .
WORKDIR "/src/ChatChallenge.Web/ChatChallenge"
RUN dotnet build "ChatChallenge.WebApp.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "ChatChallenge.WebApp.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatChallenge.WebApp.dll"]