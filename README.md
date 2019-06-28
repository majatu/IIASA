# IIASA - EOCS Full-Stack Challenge

## Challenge
Develop a micro-service solution for image processing, storage and provisioning in the Docker environment. Photos uploaded by mobile apps like FotoQuest Go should be enhanced, persisted and made available in different resolutions through an ASP.NET Core API.
### Background
FotoQuest Go (https://fotoquest-go.org/en) is one of our mobile apps in which the users visit locations in nature, directed by the map in the application. At the point the users take photos in the 4 cardinal directions (north, east, south, west) plus one photo of the ground and answer some questions about the land cover and land use at the point. 

### FotoQuest Go

#### Constraints & Goals
- The solution should be delivered with a docker-compose.yml file, that contains all necessary components (API, DB, etc.)
- The actual API should be dockerized and ready to be used as a container image
- The API should be written in ASP.NET Core and should be documented using the OpenAPI 3 specification or a Swagger endpoint
- The API should have endpoints for submitting and retrieving photos and their metadata
- The data should be stored in a database of your choice, the photos should be persisted in the file system
- The API provides different endpoints to retrieve the photos for different applications as resized versions (e.g. /thumbnail -> 128px, /small -> 512px, /large -> 2048px) but also an endpoint with customizable size
- Photos that have been uploaded to the API should automatically be enhanced (brightness, contrast, sharpen, etc.) (optional)

#### Submission & Result
- Feel free to choose proper development paradigms/concepts for this solution (SoC, DI, CQRS, TDD, DDD, ...) and implement additional functionality, etc. that you consider useful for the task.
- Please send the results as a GitHub link or any other git-based source control.
- Please also feel free to host your solution somewhere and send us the link.

You can also provide your comments and thoughts about your way of thinking to the form below.
