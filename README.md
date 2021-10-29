# Booking API

Booking API is a Net Core WebApi for a Cancun Hotel booking process.

# Requirements
To build and run this project are required:
- .NET 5.0.11
- SQL Server Express LocalDB for local run.

```powershell
cd booking-challenge # Go into project folder
dotnet restore # Restore dependencies
dotnet ef database update -p .\src\API\Booking.API.csproj # DB creation
```


# Testing
The project has 2 projects of tests. `Core.Tests` for validation model unit testing and `API.Tests` with API features testing. Both of them are located on folder `\tests\`.

To execute all tests:

```
dotnet test
```

# Run
```
dotnet run
```