﻿FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-job

# Install dotnet tools
RUN dotnet tool install --framework net7.0 -g Microsoft.Crank.AzureDevOpsWorker --version "0.2.0-*"
RUN dotnet tool install --framework net7.0 -g Microsoft.Crank.Controller --version "0.2.0-*"

ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["crank-azdo", "--connection-string", "SERVICE_BUS_CONNECTION_STRING", "--queue", "SERVICE_BUS_QUEUE"]
