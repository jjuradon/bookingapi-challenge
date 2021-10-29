# Booking API

Booking API is a Net Core WebApi for a Cancun Hotel booking process.

# Preparation
To build and execute this project is required:
- .NET 5.0.11
- 'localdb'

'''
cd booking-challenge
dotnet restore
dotnet ef database update -p .\src\API\Booking.API.csproj
'''


# Testing

'''
dotnet test
'''

# Run