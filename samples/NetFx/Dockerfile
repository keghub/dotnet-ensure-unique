# escape=`

# BUILDER
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 as build
WORKDIR /src
COPY src/ ./
RUN dotnet restore ConsoleApp/ConsoleApp.csproj
RUN dotnet publish ConsoleApp/ConsoleApp.csproj -c Release -o /out

# RUNNER
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8 AS runtime
ADD https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.ps1 C:\temp\
RUN powershell C:\temp\dotnet-install.ps1 -Channel LTS
ARG DOTNET_PATH=C:\Users\ContainerAdministrator\AppData\Local\Microsoft\dotnet\
RUN SETX DOTNET_ROOT %DOTNET_PATH%
RUN SETX PATH %PATH%;%DOTNET_PATH%
ENV DotNetEnsureUnique__S3__BucketName= `
    DotNetEnsureUnique__S3__FilePrefix= `
    DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN dotnet tool install -g EMG.Tools.EnsureUnique
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ensure-unique", "exe", "ConsoleApp.exe"]
