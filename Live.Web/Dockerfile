#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY . /app
WORKDIR /app
EXPOSE 80
VOLUME /app/logs
ENV DB_CONNECTION=1 ASPNETCORE_ENVIRONMENT=2 SERVICE_PORT=3 SERVICE_IP=4
ENTRYPOINT ["dotnet","Live.Web.dll"]