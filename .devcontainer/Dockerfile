# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.140.1/containers/dotnetcore/.devcontainer/base.Dockerfile

ARG VARIANT="3.1"
FROM mcr.microsoft.com/vscode/devcontainers/dotnetcore:0-${VARIANT}

COPY library-scripts/github-debian.sh /tmp/library-scripts/
RUN apt-get update && bash /tmp/library-scripts/github-debian.sh
