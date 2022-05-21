FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app

# Need to run from /src
COPY CollectIt.MVC/ CollectIt.MVC
COPY CollectIt.Database/ CollectIt.Database
COPY CollectIt.sln .


RUN dotnet restore CollectIt.MVC/CollectIt.MVC.View
RUN dotnet publish CollectIt.MVC/CollectIt.MVC.View -c Release -o out --no-restore --no-dependencies

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as production
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "CollectIt.MVC.View.dll"]