FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN if ! dotnet nuget list source | grep -q "https://api.nuget.org/v3/index.json"; then dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org; fi

COPY ["src/SME.Sondagem.API/SME.Sondagem.API.csproj", "SME.Sondagem.API/"]
COPY ["src/SME.Sondagem.Aplicacao/SME.Sondagem.Aplicacao.csproj", "SME.Sondagem.Aplicacao/"]
COPY ["src/SME.Sondagem.Dados/SME.Sondagem.Dados.csproj", "SME.Sondagem.Dados/"]
COPY ["src/SME.Sondagem.Dominio/SME.Sondagem.Dominio.csproj", "SME.Sondagem.Dominio/"]
COPY ["src/SME.Sondagem.Infra/SME.Sondagem.Infra.csproj", "SME.Sondagem.Infra/"]
COPY ["src/SME.Sondagem.IoC/SME.Sondagem.IoC.csproj", "SME.Sondagem.IoC/"]

RUN dotnet restore "SME.Sondagem.API/SME.Sondagem.API.csproj"
COPY . .

WORKDIR /src/src/SME.Sondagem.API
RUN dotnet build "SME.Sondagem.API.csproj" -c Release -o /app/build


FROM build AS publish
WORKDIR /src/src/SME.Sondagem.API
RUN dotnet publish "SME.Sondagem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV TZ=America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update \
    && apt-get install -yq tzdata locales \
    && dpkg-reconfigure --frontend noninteractive tzdata \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/* \
    && sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf || true

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SME.Sondagem.API.dll"]
