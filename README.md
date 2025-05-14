# AnquetteLTD

A lightweight ASP.NET Core MVC web app for creating, sharing and analysing online polls.

## Features

* **Create** single- or multi-choice polls in seconds  
* **Anonymous or authenticated voting** – owners can toggle  
* **Date window & manual enable/disable** controls  
* **Real-time results** – horizontal bar chart (Chart.js)  
* **Vote overwrite** – logged-in user always has max **1** submission per poll  
* **Unit tests** – business rules covered with xUnit + EF Core InMemory

## Tech stack

| Layer          | Library / Framework |
|----------------|---------------------|
| Backend        | ASP.NET Core 8 MVC, EF Core 8 |
| Auth           | ASP.NET Core Identity |
| UI             | Razor Pages, Bootstrap 5, Chart.js 4 |
| Database       | SQL Server (LocalDB by default) |
| Tests          | xUnit, Microsoft.EntityFrameworkCore.InMemory |

## Quick start

> Prerequisites: **.NET 8 SDK** (or higher) + **SQL Server LocalDB** (ships with Visual Studio).

```bash
git clone https://github.com/username/AnquetteLTD.git
cd AnquetteLTD

dotnet restore                    # restore NuGet packages
dotnet tool update -g dotnet-ef   # ensure EF CLI
dotnet ef database update         # create/upgrade the DB
dotnet run                        # http://localhost:5000
