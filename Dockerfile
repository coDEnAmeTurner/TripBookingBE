FROM mcr.microsoft.com/dotnet/sdk:9.0.203 AS build-env
WORKDIR /app

COPY TripBookingBE.sln ./
COPY TripBookingBE.Web/TripBookingBE.Web.csproj ./
COPY TripBookingBE.Services/TripBookingBE.Services.csproj ./
COPY TripBookingBE.DALs/TripBookingBE.DALs.csproj ./
COPY TripBookingBE.Commons/TripBookingBE.Commons.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0.4
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TripBookingBE.Web.dll"]
