# Dotnet Project Starter Template With Layered Achitecture

## Table of Contents

  - [Setting up the Dotnet App](#setting-up-the-dotnet-app)


### Setting up the Dotnet App

#### Navigate to the Server Directory:

Open the solution file located in the server directory.

#### Open Package Manager Console or Terminal:

In Visual Studio, open the Package Manager Console.
Apply Database Migration: Add the database migration using the following command:

```
dotnet ef database update --project Server.Infrastructure --startup-project Server.API
```

#### Run the Dotnet App:

Now, start the Dotnet Application and you are good to go.
