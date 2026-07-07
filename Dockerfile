FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskFlow.slnx ./
COPY src/TaskFlow.Api/TaskFlow.Api.csproj src/TaskFlow.Api/
COPY src/TaskFlow.Application/TaskFlow.Application.csproj src/TaskFlow.Application/
COPY src/TaskFlow.Domain/TaskFlow.Domain.csproj src/TaskFlow.Domain/
COPY src/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj src/TaskFlow.Infrastructure/
RUN dotnet restore src/TaskFlow.Api/TaskFlow.Api.csproj

COPY src/ src/
RUN dotnet publish src/TaskFlow.Api/TaskFlow.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN mkdir -p /app/data
COPY --from=build /app/publish .

ENV ConnectionStrings__TaskFlowDb="Data Source=/app/data/taskflow.db"
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskFlow.Api.dll"]
