# CraftHouse
CraftHouse is an app build for a restaurant. It's main target is to allow customers to browse through the menu. Application has an session based auth system. In order to place an order customer need to create an account. Application implements basic roles to distinguish customers from stuff. From the perspctive of privilidged user we can modify products (add/update/delete), process orders and view all accounts.

## Technologies Used
- Razor pages
- Entity Framwork Core
- Serilog
- FluentValidation


## How to run
To run this app you need docker and .NET with .NET 7 sdk installed.
1. Clone the repository
```
git clone https://github.com/AdamCzerwonka/CraftHouse
```
2. Run MS SQL docker instance with docker compose
```
docker compose up -d
```
3. Start the app
```
dotnet run
```
After this you can check the app with https://localhost:7280.
