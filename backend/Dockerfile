FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY ["tms-api.csproj", ""]
RUN dotnet restore "./tms-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "tms-api.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "tms-api.csproj" -c Release -o /app/publish

FROM publish AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "tms-api.dll"]
