# SimpleSearch

Provides API for uploading files and search which files contain the given word.

## Architecture 

![](img/Diagram.png)

## API

**Start upload session**

This API takes the file's name, file size and extension.

```
POST /api/v1/uploads

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
