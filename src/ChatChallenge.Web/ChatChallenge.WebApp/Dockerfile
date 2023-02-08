#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChatChallenge.Web/ChatChallenge.WebApp/ChatChallenge.WebApp.csproj", "ChatChallenge.Web/ChatChallenge.WebApp/"]
COPY ["ChatChallenge.Common/ChatChallenge.Contracts/ChatChallenge.Contracts.csproj", "ChatChallenge.Common/ChatChallenge.Contracts/"]
RUN dotnet restore "ChatChallenge.Web/ChatChallenge.WebApp/ChatChallenge.WebApp.csproj"
COPY . .
WORKDIR "/src/ChatChallenge.Web/ChatChallenge.WebApp"
RUN dotnet build "ChatChallenge.WebApp.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "ChatChallenge.WebApp.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatChallenge.WebApp.dll"]