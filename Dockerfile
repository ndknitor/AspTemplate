# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Set the working directory
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out
# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# Set the working directory
WORKDIR /app
# Copy the published output from the build image
COPY --from=build /app/out .
RUN chown -R www-data /app
USER www-data:www-data
# Expose the desired port (replace 80 with your port number if needed)
ENTRYPOINT ["dotnet", "AspTemplate.dll"]

#build the image
#docker build -t new-template .

# run the image
# mkdir -p $(pwd)/wwwroot && sudo chown www-data:www-data $(pwd)/wwwroot && sudo chmod 775 $(pwd)/wwwroot
# mkdir -p $(pwd)/logs && sudo chown www-data:www-data $(pwd)/logs && sudo chmod 775 $(pwd)/logs
# docker run -d -p 5000:80 -v $(pwd)/wwwroot:/app/wwwroot -v $(pwd)/logs:/app/logs new-template