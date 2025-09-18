# \# BookingSystem

# 

# A .NET 8 Web API for managing settlement bookings. This system handles booking reservations for property settlement meetings between conveyancers and mortgage providers.

# 

# \## 📋 Overview

# 

# InfoTrack provides a settlement service where property purchasers' conveyancers meet with mortgage provider representatives and vendor conveyancers at agreed times. The system has fixed capacity and can only provide a limited number of simultaneous settlements.

# 

# \### Business Rules

# 

# \- \*\*Business Hours\*\*: 9:00 AM - 4:00 PM (bookings must complete by 5:00 PM)

# \- \*\*Booking Duration\*\*: 1 hour (e.g., 9:00 AM booking holds the spot from 9:00-9:59 AM)

# \- \*\*Maximum Capacity\*\*: Up to 4 simultaneous settlements

# \- \*\*Same Day Only\*\*: All bookings are assumed to be for the same day

# 

# \## 🏗️ Architecture

# 

# The solution follows Clean Architecture principles with clear separation of concerns:

# ├── Infotrack/                     # Main Web API project

# │   ├── Controllers/               # API controllers

# │   ├── Services/                  # Business logic layer

# │   ├── Repositories/              # Data access layer

# │   ├── Models/                    # Domain models

# │   ├── Converters/                # JSON converters

# │   ├── Filters/                   # Exception filters

# │   └── CustomExceptions/          # Custom exception types

# └── BookingSystemTests/            # Unit tests

# \### Key Components

# 

# \- \*\*BookingController\*\*: REST API endpoint for creating bookings

# \- \*\*BookingService\*\*: Business logic for booking validation and creation

# \- \*\*BookingRepository\*\*: In-memory storage for bookings

# \- \*\*TimeOnlyJsonConverter\*\*: Custom JSON converter for time format handling

# \- \*\*BookingExceptionFilter\*\*: Global exception handling for proper HTTP status codes

# 

# \## 🚀 Getting Started

# 

# \### Prerequisites

# 

# \- \[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

# \- IDE (Visual Studio 2022, VS Code, or Rider)

# 

# \### Installation

# 

# 1\. \*\*Clone the repository\*\*git clone <repository-url>

# cd BookingSystem

# 2\. \*\*Restore dependencies\*\*dotnet restore

# 3\. \*\*Build the solution\*\*dotnet build

# 4\. \*\*Run the application\*\*cd Infotrack

# dotnet run

# The API will be available at:

# \- \*\*HTTP\*\*: `http://localhost:5000`

# \- \*\*HTTPS\*\*: `https://localhost:5001`

# \- \*\*Swagger UI\*\*: `https://localhost:5001/swagger`

# 

# \## 📚 API Documentation

# 

# \### Create Booking

# 

# Creates a new settlement booking for the specified time and name.

# 

# \*\*Endpoint:\*\* `POST /booking`

# 

# \*\*Request Body:\*\*{

# &nbsp; "bookingTime": "09:30",

# &nbsp; "name": "John Smith"

# }

# \*\*Request Fields:\*\*

# \- `bookingTime` (string, required): 24-hour time format (HH:mm) between 09:00-16:00

# \- `name` (string, required): Non-empty client name

# 

# \*\*Response Codes:\*\*

# 

# | Status Code | Description | Response Body |

# |-------------|-------------|---------------|

# | 200 OK | Booking created successfully | `{"bookingId": "d90f8c55-90a5-4537-a99d-c68242a6012b"}` |

# | 400 Bad Request | Invalid request data | Validation error details |

# | 409 Conflict | No available slots at requested time | Error message |

# 

# \### Example Requests

# 

# \#### ✅ Successful Bookingcurl -X POST "https://localhost:5001/booking" \\

# &nbsp; -H "Content-Type: application/json" \\

# &nbsp; -d '{

# &nbsp;   "bookingTime": "10:30",

# &nbsp;   "name": "John Smith"

# &nbsp; }'

# \*\*Response:\*\*

# {

# &nbsp; "bookingId": "d90f8c55-90a5-4537-a99d-c68242a6012b"

# }

# \#### ❌ Invalid Time (Out of Hours)curl -X POST "https://localhost:5001/booking" \\

# &nbsp; -H "Content-Type: application/json" \\

# &nbsp; -d '{

# &nbsp;   "bookingTime": "17:00",

# &nbsp;   "name": "John Smith"

# &nbsp; }'

# \*\*Response (400 Bad Request):\*\*{

# &nbsp; "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",

# &nbsp; "title": "One or more validation errors occurred.",

# &nbsp; "status": 400,

# &nbsp; "errors": {

# &nbsp;   "BookingTime": \[

# &nbsp;     "Booking time must be between 09:00 and 16:00"

# &nbsp;   ]

# &nbsp; }

# }

# \#### ❌ Booking Conflict (Fully Booked)curl -X POST "https://localhost:5001/booking" \\

# &nbsp; -H "Content-Type: application/json" \\

# &nbsp; -d '{

# &nbsp;   "bookingTime": "10:00",

# &nbsp;   "name": "Jane Doe"

# &nbsp; }'

# \*\*Response (409 Conflict):\*\*{

# &nbsp; "message": "Cannot create booking due to maximum (4) simultaneous bookings reached."

# }

# \## 🧪 Testing

# 

# \### Run Unit Testsdotnet test

# \### Test Coverage

# The test suite includes comprehensive coverage of:

# \- ✅ Successful booking scenarios

# \- ✅ Validation edge cases

# \- ✅ Conflict detection logic

# \- ✅ Business rule enforcement

# \- ✅ Algorithm correctness

# 

# \*\*Test Results:\*\*

# \- \*\*Total Tests\*\*: 9

# \- \*\*Passing\*\*: 9 ✅

# \- \*\*Coverage\*\*: Core business logic and edge cases

# 

# \### Example Test Scenarios

# \- Valid bookings with empty repository

# \- Maximum capacity enforcement (4 simultaneous bookings)

# \- Overlapping time window calculations

# \- Business hours validation

# \- Boundary condition testing

# 

# \## 🔧 Development

# 

# \### Project Structure

# Infotrack/

# ├── Controllers/

# │   └── BookingController.cs      # REST API endpoints

# ├── Services/

# │   ├── IBookingService.cs        # Service interface

# │   └── BookingService.cs         # Business logic implementation

# ├── Repositories/

# │   ├── IBookingRepository.cs     # Repository interface

# │   └── BookingRepository.cs      # In-memory data storage

# ├── Models/

# │   ├── Booking.cs                # Request model with validation

# │   └── BookingResult.cs          # Response model

# ├── Converters/

# │   └── TimeOnlyJsonConverter.cs  # Custom JSON time handling

# ├── Filters/

# │   └── BookingExceptionFilter.cs # Global exception handling

# └── CustomExceptions/

# &nbsp;   └── BookingConflictException.cs # Custom exception for conflicts

# \### Key Design Decisions

# 

# 1\. \*\*In-Memory Storage\*\*: As per requirements, bookings are stored in memory and lost on restart

# 2\. \*\*TimeOnly Type\*\*: Uses .NET 6+ `TimeOnly` for better time handling

# 3\. \*\*Custom JSON Converter\*\*: Handles "HH:mm" format conversion

# 4\. \*\*Exception Filter\*\*: Converts business exceptions to appropriate HTTP status codes

# 5\. \*\*Dependency Injection\*\*: Clean separation with interface-based design

# 

# \### Adding New Features

# 

# 1\. \*\*Add new validation rules\*\*: Extend the `Booking` model with additional `DataAnnotation` attributes

# 2\. \*\*Modify business logic\*\*: Update the `BookingService` class

# 3\. \*\*Change capacity limits\*\*: Modify `MaxSimultaneousBookings` constant

# 4\. \*\*Add new endpoints\*\*: Extend the `BookingController`

# 

# \### Time Handling

# \- Uses `TimeOnly` (.NET 6+) for precise time-only operations

# \- Custom JSON converter handles "HH:mm" format

# \- Business hours validation via `Range` attribute

# 

# \### Concurrency

# \- Current implementation uses in-memory storage

# \- For production: consider thread-safe collections or database storage

# \- Repository pattern allows easy swapping of storage implementations

# 

# \### Error Handling

# \- Model validation returns 400 Bad Request

# \- Business rule violations return 409 Conflict

# \- Global exception filter ensures consistent error responses

# 

# \## 🤝 Contributing

# 

# 1\. Fork the repository

# 2\. Create a feature branch (`git checkout -b feature/amazing-feature`)

# 3\. Commit your changes (`git commit -m 'Add amazing feature'`)

# 4\. Push to the branch (`git push origin feature/amazing-feature`)

# 5\. Open a Pull Request

# 

# \## 📄 License

# 

# This project is part of the InfoTrack technical assessment.

# 

# ---

