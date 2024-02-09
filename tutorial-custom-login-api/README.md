# API for Authentication & Authorization with JSON Web Tokens (JWT), Refresh Tokens & Roles

In this project Icreated an api to create an account and save it in a database using hash & salt. Furthermore registerd users are able to log back in. In addition to that I will use JWT & Refresh Tokens. A registerd user can get a role and based on this role the user will be able to call specific API endpoints.

<br>

## Pictures

![](https://i.imgur.com/za8WRme.png)

<br>

## Tech Stack

- ASP.NET Core Web API (.NET 6)
	- Authentication Type -> None
- Class Library (.NET 6)
- Sqlite
- JWT

<br>

## Project Structure

1. Blazor Client
2. ASP.NET Core Web API
	1. Models / DTO
	2. Services
3. Class Library 
	1. Business Logic Layer
4. Class Library 
	1. Data Access Layer

<br>

## Notes

- frontend has to check if the refresh token is expired or close to expiring to request a new one
