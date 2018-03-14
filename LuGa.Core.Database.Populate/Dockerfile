FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

ARG LUGA_NUGET_PAT

# copy csproj and restore as distinct layers
COPY *.csproj ./
COPY NuGet.config ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out 

# build runtime image
FROM microsoft/aspnetcore:2.0
ENV ASPNETCORE_ENVIRONMENT="Production"
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "LuGa.Core.Database.Migrations.dll"]