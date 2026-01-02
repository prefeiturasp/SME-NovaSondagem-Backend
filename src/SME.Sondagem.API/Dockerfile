FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN if ! dotnet nuget list source | grep -q "https://api.nuget.org/v3/index.json"; then dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org; fi

COPY ["SME.Sondagem.Api/SME.Sondagem.Api.csproj", "SME.Sondagem.Api/"]
COPY ["SME.Sondagem.Aplicacao/SME.Sondagem.Aplicacao.csproj", "SME.Sondagem.Aplicacao/"]
COPY ["SME.Sondagem.Dados/SME.Sondagem.Dados.csproj", "SME.Sondagem.Dados/"]
COPY ["SME.Sondagem.Dominio/SME.Sondagem.Dominio.csproj", "SME.Sondagem.Dominio/"]
COPY ["SME.Sondagem.Infra/SME.Sondagem.Infra.csproj", "SME.Sondagem.Infra/"]
COPY ["SME.Sondagem.IoC/SME.Sondagem.IoC.csproj", "SME.Sondagem.IoC/"]
RUN dotnet restore "SME.Sondagem.Api/SME.Sondagem.Api.csproj"
COPY . .
WORKDIR "/src/SME.Sondagem.Api"
RUN dotnet build "SME.Sondagem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.Sondagem.Api.csproj" -c Release -o /app/publish

FROM base AS final 
WORKDIR /app
ENV TZ=America/Sao_Paulo
RUN apt-get update \
    && apt-get install -yq tzdata locales -y \
    && dpkg-reconfigure --frontend noninteractive tzdata \
	  && locale-gen en_US.UTF-8
	  
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SME.Sondagem.Api.dll"]
