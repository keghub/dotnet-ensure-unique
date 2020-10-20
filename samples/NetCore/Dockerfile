# escape=`
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY src/ ./
RUN dotnet restore ConsoleApp/ConsoleApp.csproj
RUN dotnet publish ConsoleApp/ConsoleApp.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
WORKDIR /app
COPY --from=build /out .
ENV PATH="/root/.dotnet/tools:${PATH}" `
    DotNetEnsureUnique__S3__BucketName= `
    DotNetEnsureUnique__S3__FilePrefix=

RUN dotnet tool install -g EMG.Tools.EnsureUnique

ENTRYPOINT dotnet ensure-unique exe ./ConsoleApp
