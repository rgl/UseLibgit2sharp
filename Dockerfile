# syntax=docker/dockerfile:1.4

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src
COPY UseLibgit2sharp.csproj ./
RUN dotnet restore
COPY *.cs ./
RUN dotnet publish --no-restore --configuration Release -warnAsError

FROM mcr.microsoft.com/dotnet/runtime:6.0
COPY --from=build /src/bin/Release/net6.0/publish /app
ENTRYPOINT ["/app/UseLibgit2sharp"]
