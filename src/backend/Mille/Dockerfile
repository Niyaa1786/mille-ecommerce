
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Mille.Api/Mille.Api.csproj Mille.Api/
COPY Mille.Application/Mille.Application.csproj Mille.Application/
COPY Mille.Infrastructure/Mille.Infrastructure.csproj Mille.Infrastructure/
COPY Mille.Domain/Mille.Domain.csproj Mille.Domain/
RUN  dotnet restore "Mille.Api/Mille.Api.csproj"

COPY . .
RUN dotnet publish "Mille.Api/Mille.Api.csproj" -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet","Mille.Api.dll" ]