Sample file service that utilizes s3 for file managment.

**Contracts project** contains models and interface required to interact with the service via **httpclient**, implementation of which is in **communication project**.

Hangfire starts background task that cleans up stuck uploads after time specified in appsettings, the time specification ued in code via IOptions pattern supplied by the framework.

Uses Redis for cachine download presigned links to avoid remaking them every time a file is requested

Contains docker compose with related services that are needed for the mcroservice to function.
