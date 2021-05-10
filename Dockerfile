FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN ls
RUN dotnet restore ./CavCash.Node/CavCash.Node.csproj
RUN dotnet publish ./CavCash.Node/CavCash.Node.csproj -c Release -o out

EXPOSE PORT_NUM

# Development Version 
ENV VAR VAL

ENTRYPOINT ["dotnet","./out/CavCash.Node.dll"]
