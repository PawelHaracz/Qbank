FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 62779
EXPOSE 44313

ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=https://+:443;http://+:80
#ENV ASPNETCORE_HTTPS_PORT=44313

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /Qbank
COPY Qbank.sln  ./
#COPY Qbank.Core/Qbank.Core.csproj Qbank.Core/
#COPY Qbank.Questions/Qbank.Questions.csproj Qbank.Questions/
COPY Qbank.Questions.WebApi/Qbank.Questions.WebApi.csproj Qbank.Questions.WebApi/

RUN dotnet restore Qbank.Questions.WebApi/Qbank.Questions.WebApi.csproj
#RUN dotnet restore -nowarn:msb3202,nu1503 -p:RestoreUseSkipNonexistentTargets=false
COPY . .
WORKDIR /Qbank/Qbank.Questions.WebApi
RUN dotnet build Qbank.Questions.WebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Qbank.Questions.WebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Qbank.Questions.WebApi.dll"]
