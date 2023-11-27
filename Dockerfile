# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Set the working directory
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out
# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
# Set the working directory
WORKDIR /app
# Copy the published output from the build image
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "AspTemplate.dll"]

#build the image
#docker build -t asp-template .

# run the image
# mkdir -p $(pwd)/wwwroot && mkdir -p $(pwd)/logs 
# docker run -d -p 5000:8080 -v $(pwd)/wwwroot:/app/wwwroot -v $(pwd)/logs:/app/logs asp-template