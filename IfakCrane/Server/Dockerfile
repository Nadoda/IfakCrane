#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IfakCrane/Server/IfakCrane.Server.csproj", "IfakCrane/Server/"]
COPY ["IfakCrane/Client/IfakCrane.Client.csproj", "IfakCrane/Client/"]
COPY ["IfakCrane/Shared/IfakCrane.Shared.csproj", "IfakCrane/Shared/"]
RUN dotnet restore "IfakCrane/Server/IfakCrane.Server.csproj"
COPY . .
WORKDIR "/src/IfakCrane/Server"
RUN dotnet build "IfakCrane.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IfakCrane.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IfakCrane.Server.dll"]