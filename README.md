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
docker run -it --rm -p 5001:5001 my-chat-app
```

## Notes
* this app is using cashe memory only, its mean that after  each run all the data will disapere. if you wat to connect to your local data cbase the sharedDb is already contains important classes to enable fluent change to use another DB.
* currrently the app is configured to run on port 5009. you can change it easely in two files - ChatWeb\ChatWeb\ChatWeb.http and ChatWeb\ChatWeb\Properties\launchSettings.json
