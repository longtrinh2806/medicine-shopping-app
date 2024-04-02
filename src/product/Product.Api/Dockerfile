#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/product/Product.Api/Product.Api.csproj", "src/product/Product.Api/"]
COPY ["src/product/Product.Data/Product.Data.csproj", "src/product/Product.Data/"]
COPY ["src/product/Product.Services/Product.Services.csproj", "src/product/Product.Services/"]
RUN dotnet restore "./src/product/Product.Api/./Product.Api.csproj"
COPY . .
WORKDIR "/src/src/product/Product.Api"
RUN dotnet build "./Product.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Product.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Api.dll"]