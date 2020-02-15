# SimpleSearch

Provides API for uploading files and search which files contain the given word. The whole architecture is developed to be hosted in Azure Cloud.

## Architecture 

![](img/Diagram.png)

## Prerequisite

* Docker (Linux containers)

## How to build

Probably the only way is to build the docker-compose file in Visual Studio 2019

## API

**Start upload session**

This API takes the file's name, file size and extension.

```
POST /api/v1/uploads

Body:
{
  "FileName": "someFile",
  "SizeInBytes": 24,
  "Extension": "txt"
}
```

```
200 OK

{
  "id": "5e4765792ded3a00176eeb65",
  "sizeInBytes": 24,
  "fileName": "someFile",
  "extension": "Txt",
  "parts": [
      {
          "id": "MDAwMDAwMA==",
          "offset": 0,
          "sizeInBytes": 24
      }
  ]
}
```

**Upload part**

Aftert starting the session, clients can start uploading different parts of the file. Uploading is allowed only for sessions that are not completed yet.

```
PUT /api/v1/uploads/:uploadid/parts/:partid

Body: binary data
```

```
200 OK
or
400 BadRequest
```

**Complete uploading**

When all parts have been uploaded the session needs to be completed. 

```
PUT /api/v1/uploads/:uploadid
```

```
200 OK
or
400 BadRequest
{
  "corruptedParts": [
      {
          "id": "MDAwMDAwMA==",
          "offset": 0,
          "actualSizeInBytes": 0,
          "expectedSizeInBytes": 24,
          "state": "NotUploaded"
      },
      {
          "id": "MDAwMDAwMB==",
          "offset": 24,
          "actualSizeInBytes": 12,
          "expectedSizeInBytes": 24,
          "state": "Corrupted"
      },
  ]
}
```

**Search for words**

Only search for tags is supported for now 

```
POST /api/v1/search

Body:
{
  "query": {
    "tag": "word"
  }
}
```

```
200 OK

{
  "tag": "word",
  "documents": [
      {
          "fileName": "someFile",
          "extension": "Txt"
      }
  ]
}
```

## Features

* Reliable uploading 
   * No limitation for file size (in theory)
   * Can be done in parallel
   * Parts can be reuploaded
 
* Caching search results
  * Search queries are cached for 1 minute (by default) in order to reduce the load on the database

* Scalable architecture 
  * Each component does not have any state and can be scaled independently

## Future imporvements (aka known issues) 

* Add implementation for Azure Service Bus message broker
* Add end-to-end and integration tests
* Add security for uploading (right now there is no checks at all, anyone can upload parts to any session, which is not good)
* Add appropriate logging. Need some centralized log store with dashboards
* Add health checks
* Analyzer is very simple, probably need to filter stop words and add stamming for better search experience
* Add infrastructure as a service (Pulumi, Terraform)
* CI/CD
