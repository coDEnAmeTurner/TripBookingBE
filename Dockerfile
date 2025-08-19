FROM mcr.microsoft.com/dotnet/sdk:9.0.203 AS build-env
WORKDIR /app

COPY TripBookingBE.sln ./
COPY TripBookingBE.Web/TripBookingBE.Web.csproj TripBookingBE.Web/
COPY TripBookingBE.Services/TripBookingBE.Services.csproj TripBookingBE.Services/
COPY TripBookingBE.DALs/TripBookingBE.DALs.csproj TripBookingBE.DALs/
COPY TripBookingBE.Commons/TripBookingBE.Commons.csproj TripBookingBE.Commons/
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0.4
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TripBookingBE.Web.dll"]
