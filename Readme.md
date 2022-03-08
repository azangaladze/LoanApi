## Table of contents
* [General info](#general-info)
* [Highlights](#highlights)
* [Setup](#setup)
* [Functionality Overview](#functionality-overview)
* [Structure Overview](#structure-overview)
* [Users and Roles](#users-and-roles)


## General info
This is Loans WEB API, in this project you can register as user,
login as Accountant or User and perform CRUD operations on Users and Loans
All the information is stored in Database
	
## Highlights
There are some features in this project:
* Authentication
* AutoMapper
* SeriLog
* JWT bearer token
* Roles
* Storing hashed passwords in db
* Fluent Validation
* Custom error messages for 401 and 403 codes
* Authorize in Swagger 
* xUnit test with fluent assertion
	
## Setup
To run this project: 
* Clone or download .zip
* Start project in visual studio
* In order to create database open Package Manager Console and run add-migration, then update-database
* In order to perform CRUD operations you need to authorize
* There are two sample users in DB User and Accountant 
* You can register a new User or use existing ones (usernames and passwords provided at the end of file)
* After successful login JWT Bearer token is generated
* Copy JWT Bearer token, press Authorize button in Swagger and paste token or use postman Auth(type bearer token)
* After that you can use methods

## Functional Overview
There are roles in this project: Accountant and User
- After successful authorization Accountant is able to:
* Get information on all users/loans
* Update any user/loan information
* Delete any user/loan, except own account
* Change status of user (IsBlocked true/false)
* Change status of loan (from Processing to Negative or Positive)

- After successful authorization User is able to:
* Get information only about own user/loan
* Take loan if user status IsBlocked is false
* Delete/modify onw loan only if status is Processing

## Structure Overview
Application consists of following parts:
### src
* #### Api
This layer contains all application logic. Here are all controllers and validations
* #### Core
This layer contains entities, entity fields, interfaces
* #### Infrastructure
This layer contains classes for accessing database, services, models
### tests
Contains all tests for this application


## Users and Roles
```
User:
username: user
Password: user1234

Accountant
username: acc
password: pass1234
```
