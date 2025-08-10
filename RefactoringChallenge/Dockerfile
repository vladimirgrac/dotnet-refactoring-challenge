FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["RefactoringChallenge/RefactoringChallenge.csproj", "RefactoringChallenge/"]
COPY ["RefactoringChallenge.Bussiness/RefactoringChallenge.Bussiness.csproj", "RefactoringChallenge.Bussiness/"]
COPY ["RefactoringChallenge.Data/RefactoringChallenge.Data.csproj", "RefactoringChallenge.Data/"]
COPY ["RefactoringChallenge.Infrastructure/RefactoringChallenge.Infrastructure.csproj", "RefactoringChallenge.Infrastructure/"]
COPY ["RefactoringChallenge.Model/RefactoringChallenge.Model.csproj", "RefactoringChallenge.Model/"]

RUN dotnet restore "RefactoringChallenge/RefactoringChallenge.csproj"
COPY . .
WORKDIR "/src/RefactoringChallenge"
RUN dotnet build "RefactoringChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RefactoringChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RefactoringChallenge.dll"]
