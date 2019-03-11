FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY . .
RUN dotnet build IIASA.Api /p:Configuration=Debug
RUN dotnet publish IIASA.Api /p:Configuration=Debug /p:TrimUnusedDependencies=true -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app .
RUN apt-get update
RUN apt-get install -y --no-install-recommends libgdiplus libc6-dev
RUN rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "IIASA.Api.dll", "--environment=Development"]