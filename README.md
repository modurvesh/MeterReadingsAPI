# Introduction 
This project contains the source code for a Meter Reading API service. Currently you are able to upload meter readings, search for users and search for meter readings. The project uses a SQL lite database for storage of the users and meter readings.

# Getting Started
You will need Visual Studio 2019+ with the following tools installed in order to run the project locally:
-	.NET Core 5.0
-	ASP.NET Core 5.0
-	Entity Framework Core 5.6.3
-	CsvHelper 27.1.1

# API references
Here is a list of all API references available for use in the project:

**Route** 
```http
POST /meter-reading-uploads
```
**Body** 
multipart/form-data

**Response** 
Uploads all csv records to the database that pass the validations.

**Route** 
```http
GET /meter-readings
```
**Response** 
Returns a list of all meter readings in the database.

**Route** 
```http
GET /meter-reading?accountId=1234
```

**Parameters** 
| Parameter | Type | Description |
| :--- | :--- | :--- |
| `accountId` | `string` | Id of the user account |

**Response** 
Returns the meter readings for the user specified via the accountId provided. 

**Route** 
```http
GET /users
```

**Response** 
Returns a list of all users in the database.

**Route** 
```http
GET /user?firstName=test&lastName=test
```

**Parameters** 
| Parameter | Type | Description |
| :--- | :--- | :--- |
| `firstName` | `string` | First name of the user |
| `lastName` | `string` | Last name of the user |

**Response** 
Returns the user account that matches the firstName and lastName provided. 

# Build and Test
The project can be ran directly from Visual Studio using IIS Express. Currently there are no tests present. 

# Contribute 
The API currently only contains the core components. In future releases, here are a few features we will be looking to implement:
-	Route to POST users and store them in the database (set accountId, firstName, lastName). 
-	Authentication header using Basic Auth to login to a user account (for routes that contain sensitive data e.g. user details search)
-	Update POST user route to add additional details (e.g. DOB, address)
-	Route to GET a specific meter reading from a date range set