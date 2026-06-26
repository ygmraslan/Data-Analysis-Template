FROM cgr.dev/chainguard/dotnet-sdk:latest AS build

WORKDIR /source

COPY --chown=nonroot:nonroot *.sln ./
COPY --chown=nonroot:nonroot API/DataAnalysis.API.csproj ./API/
COPY --chown=nonroot:nonroot Application/DataAnalysis.Application.csproj ./Application/
COPY --chown=nonroot:nonroot Domain/DataAnalysis.Domain.csproj ./Domain/
COPY --chown=nonroot:nonroot Persistence/DataAnalysis.Persistence.csproj ./Persistence/
COPY --chown=nonroot:nonroot Infrastructure/DataAnalysis.Infrastructure.csproj ./Infrastructure/

RUN dotnet restore "./API/DataAnalysis.API.csproj"

COPY --chown=nonroot:nonroot . .

RUN dotnet publish "./API/DataAnalysis.API.csproj" \
    --no-restore \
    --self-contained false \
    -c Release \
    -o /source/publish

FROM cgr.dev/chainguard/aspnet-runtime:latest AS final

WORKDIR /app
COPY --from=build /source/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DataAnalysis.API.dll"]