FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -r linux-musl-x64 -o out
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
RUN apk update
RUN apk upgrade
RUN apk add libintl libssl3 libcrypto3 libstdc++ icu
COPY --from=build /app/out .
EXPOSE 8080
# RUN chown -R nobody /app
USER nobody
ENTRYPOINT ["dotnet", "AspTemplate.dll"]

# Build the image
# docker build -t asp-template .

# Run the image
# mkdir -p $(pwd)/wwwroot && sudo chown -R nobody $(pwd)/wwwroot
# mkdir -p $(pwd)/logs && sudo chown -R nobody $(pwd)/logs
# docker run -d -p 5000:8080 -v $(pwd)/wwwroot:/app/wwwroot -v $(pwd)/logs:/app/logs asp-template

# Run image with host user
# docker run -d -p 5000:8080 -v $(pwd)/wwwroot:/app/wwwroot -v $(pwd)/logs:/app/logs -u $(id -u ${USER}):$(id -g ${USER}) asp-template