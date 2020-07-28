#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY . /app
WORKDIR /app
EXPOSE 5000
ENV DB_CONNECTION=1 ASPNETCORE_ENVIRONMENT=2
ENTRYPOINT ["dotnet","Live.Web.dll"]