# ChatWeb
## Overview
This repository contains the backend server for a real-time chat application. The server is built using ASP.NET Core and SignalR to enable real-time communication between clients.
For enable full using in this server, clone the front(client-side) for this app from this link https://github.com/YoniThee/chatwebfront.git

## Prerequisites
* .NET 7.0 SDK or later
* Docker (Optional, for containerization)

## Getting Started
### Clone the Repository:
```
 Bash
 git clone https://github.com/YoniThee/ChatWebBackend.git
```

### Restore Dependencies:
```
  Bash
  cd ChatWebBackend
  dotnet restore
```

### connect to sqlServer
in your microsoft sqlServer app create a new DB
(at the objectExplorer click right on "Database" folder under your the sql server and choose "new DataBase")

now from visual studio after your repo is up go to server explorer and choose connect to dataBase
![image](https://github.com/user-attachments/assets/edd44bb1-2082-4a35-8333-61cfc0e0a183)
here enter your server name and select the DB that you created at the previues step.

now test the connection and make sure is working sueccessfully.

now in your project there is 2 places that you need to change according your serverName and DB name.

in appSetting file there is string
```
"ConnectionStrings": {
  "DefaultConnection": "Server=server-name;Database=my-DB-name;Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True;"
},
```

here change the Server and Datbase to your correct name

and also in chatDbContext file 
```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("Server=server-name;Database=my-DB-name;Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True;");

}
```

change according yours.

now you need to create the tables in your DB, you can do it manually or using this commands
go to packege meneger terminal and run:

```
Install-Package Microsoft.EntityFrameworkCore

Add-Migration InitialCreate

Update-Database

```

make sure that you have all the necessary tables and that each is defined as well including the correct Keys!
(if something wrong in this procces you can use the attached files - create**table to create the correct tables manually)

 
### Run the Application:
#### Without Docker:

Start the Development Server:
```
Bash
dotnet run
```
Or
using IDE (visual studio 2022 for example) and run the repo

#### With Docker:

1. Build the Docker Image:
```
Bash
docker build -t my-chat-app .
```

2. Run the Docker Container:
```
Bash
docker run -it --rm -p 5009:8080 app-chat-web
```

## Notes
* this app is using cashe memory only, its mean that after  each run all the data will disapere. if you wat to connect to your local data cbase the sharedDb is already contains important classes to enable fluent change to use another DB.
* currrently the app is configured to run on port 5009. you can change it easely in two files - ChatWeb\ChatWeb\ChatWeb.http and ChatWeb\ChatWeb\Properties\launchSettings.json
