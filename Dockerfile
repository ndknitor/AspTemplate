# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out
# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
RUN apk update
RUN apk add libintl libssl1.1 libcrypto1.1 libstdc++ icu
COPY --from=build /app/out .
# RUN chown -R nobody /app
USER nobody
ENTRYPOINT ["dotnet", "AspTemplate.dll"]

# RUN chown -R www-data /app
# USER www-data:www-data
# build the image
# docker build -t asp-template .

# run the image
# mkdir -p $(pwd)/wwwroot && sudo chown -R nobody $(pwd)/wwwroot
# mkdir -p $(pwd)/logs && sudo chown -R nobody $(pwd)/logs
# docker run -d -p 5000:8080 -v $(pwd)/wwwroot:/app/wwwroot -v $(pwd)/logs:/app/logs asp-template