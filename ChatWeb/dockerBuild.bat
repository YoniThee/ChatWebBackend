@echo off

:: Build the Docker image
docker build -t chatapp .

:: Run the Docker container
docker run -it --rm -p 8080:8080 chatapp