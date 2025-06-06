# FUR Api endpoints

## Getting Package Information
### `GET /api/v1/packages/{package_name}/{version}`
Gets information about a packages. This endpoint returns the furconfig.json for the package. <br>
Version is optional

### `GET /api/v1/packages?sort={sort}`
Gets all packages <br>
Sort can be <br>
- mostDownloads - Most Downloaded
- leastDownloads - Least Downloaded
- recentlyUpdated - Most Recently Updated
- recentlyUploaded - Most Recently Uploaded
- oldestUpdated - Package with longest time since updated
- oldestUploaded - First Uploaded<br>
Returns json data in the format of 
```json
{
    "package_count": 100,
    "packages": [
        "name",
        "name",
        "name",
        "name",
        "name"
    ]
}
```

### `GET /api/v1/packages?search={query}`
Search for packages. Returns the same style of data as the previous endpoint

### `POST /api/v1/packages`
Uploads a package <br>
Data should be the packages furconfig.json
