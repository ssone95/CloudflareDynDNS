FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

RUN mkdir -p /src
RUN mkdir -p /publish
WORKDIR /src
COPY *.csproj .

RUN dotnet restore

COPY Cloudflare .
COPY Program.cs .

RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/runtime:7.0 as runtime

WORKDIR /publish

COPY --from=build /publish .
COPY appsettings*.json .
COPY config.json .

ENTRYPOINT [ "dotnet", "CloudflareDynDns.dll" ]