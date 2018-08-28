FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 62779
EXPOSE 44313

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Qbank.Questions.WebApi/Qbank.Questions.WebApi.csproj Qbank.Questions.WebApi/
RUN dotnet restore Qbank.Questions.WebApi/Qbank.Questions.WebApi.csproj
COPY . .
WORKDIR /src/Qbank.Questions.WebApi
RUN dotnet build Qbank.Questions.WebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Qbank.Questions.WebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Qbank.Questions.WebApi.dll"]
