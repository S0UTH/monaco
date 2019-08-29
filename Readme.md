# Project Monaco
## SF Food Truck Finder API
This is a simple REST API with one endpoint that takes in a point (latitude,longitude) and returns 5 closest food trucks in San Francisco downtown area.

The food truck data is sourced from [Mobile Food Facility Permit](https://dev.socrata.com/foundry/data.sfgov.org/rqzj-sfat) information published by [San Francisco Data API](https://datasf.org/opendata/).

### Getting Started
#### Endpoint
Project Monaco has been deployed as an Azure Function and the endpoint can be activated with a pre-shared key.

```json
GET https://monaco.azurewebsites.net/api/trucks?lat={latitude}&lon={longitude}&code={key}
```
#### Parameters
| Name  | Description   |
|---|---|
| {latitude}  | The base point latitude of the location being passed. Example: 47.622942.  |
| {longitude}  | The base point longitude of the location being passed. Example: -122.316456.  |
| {key}  | The endpoint is protected by this key  |

#### Responses
| Name  | Description   |
|---|---|
| 200 OK  | Valid response  |
| 400 Bad Request  | The input {latitude}/{longitude} are invalid or out of bounds |
| 401 Unauthorized  | Access denied due to invalid {key} parameter |
| 500 Internal Server Error  | An error occurred while processing the request. Please try again later. |

#### Example
##### Sample request
```json
GET https://monaco.azurewebsites.net/api/trucks?lat=37.72&lon=-122.43&code=TdterTRUNCATEDxcdw==
```
##### Response

###### Status Code 200

###### JSON
```json
{
    "summary": {
        "requestedLocation": {
            "lat": 37.72,
            "lon": -122.43
        },
        "responseTime": 192
    },
    "result": [
        {
            "distanceInMeters": 663.18,
            "position": {
                "lat": 37.7236612795053,
                "lon": -122.43594382524
            },
            "facilityType": "",
            "name": "Fruteria Serrano",
            "address": "4650 MISSION ST",
            "zip": 28861,
            "location": "MISSION ST: OCEAN AVE to PERSIA AVE (4650 - 4699)",
            "daysHours": "Mo/Th/Fr:11AM-6PM"
        },
        {
            "distanceInMeters": 868.79,
            "position": {
                "lat": 37.7123026047444,
                "lon": -122.431644190373
            },
            "facilityType": "Truck",
            "name": "Giant Burrito",
            "address": "1500 GENEVA AVE",
            "zip": 28861,
            "location": "GENEVA AVE: PRAGUE ST to LINDA VISTA STPS (1500 - 1598) -- SOUTH --",
            "daysHours": ""
        },
        {
            "distanceInMeters": 881.73,
            "position": {
                "lat": 37.7275665375917,
                "lon": -122.432969701989
            },
            "facilityType": "",
            "name": "tacos y pupusas los trinos",
            "address": "4384 MISSION ST",
            "zip": 28861,
            "location": "MISSION ST: AVALON AVE to COTTER ST (4368 - 4439)",
            "daysHours": ""
        },
        {
            "distanceInMeters": 1996.09,
            "position": {
                "lat": 37.7331423951426,
                "lon": -122.414568632419
            },
            "facilityType": "Truck",
            "name": "Natan's Catering",
            "address": "400 ALEMANY BLVD",
            "zip": 28859,
            "location": "ALEMANY BLVD: FOLSOM ST to ELLSWORTH ST \\ I-280 S OFF RAMP \\ I-280 S ON RAMP (400 - 498) -- NORTH --",
            "daysHours": ""
        },
        {
            "distanceInMeters": 2106.27,
            "position": {
                "lat": 37.7350052991284,
                "lon": -122.415417409745
            },
            "facilityType": "Truck",
            "name": "Golden Catering",
            "address": "601 CRESCENT AVE",
            "zip": 28859,
            "location": "CRESCENT AVE: ANDERSON ST to ELLSWORTH ST (600 - 699)",
            "daysHours": ""
        }
    ]
}
```
### Design
#### Data - Azure Maps Service
The food truck data from [this geojson](https://dev.socrata.com/foundry/data.sfgov.org/rqzj-sfat#) was downloaded and modified to include 'geometryId' which is unique per entry. This updated version is available here for reference.

This application uses Azure Maps service to find closest points for a given location. So the _geojson_ was uploaded to Maps service using [Data - Upload Preview API](https://docs.microsoft.com/en-us/rest/api/maps/data/uploadpreview). This Maps service returns a UUID - the unique data id for the uploaded data - that must be included in future Maps API calls.
#### Data - SQL
The CSV formatted food truck data from [here](https://data.sfgov.org/resource/rqzj-sfat.csv) was imported into Azure SQL server database table using SSMS flat-file import feature. The primary purpose of this storage is to persist and retrieve properties of the food facility like name, address, location and hours of operation.

#### Application 
It is a .NET Core 2.1, HTTP-triggered Azure Function app developed in Visual Studio 2019.

Azure Function hosted API service was chosen based on following:
- light-weight endpoint service
- out-of-box basic security 
- easy to scale up

Azure SQL Server was chosen primarily due to the ease of use and scaling features.

It uses Application Insights to help monitor performance and track down issues.

#### Enhancements
The following items could have been possible with more time budget. 

- Periodic auto-import of Azure Maps and SQL data from SF City API
- Consider using NoSQL databases like CosmosDB or use SQL server's JSON features
- More unit test coverage.
- Deployment using Azure DevOps to streamline release to test and live environment.
- A simple web SPA to get latitude/longitude from the browser and invoke the API to show nearby food trucks.