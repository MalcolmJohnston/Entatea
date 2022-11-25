### Build Status

[![.NET Core](https://github.com/MalcolmJohnston/Entatea/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/MalcolmJohnston/Entatea/actions/workflows/dotnet-core.yml)

### NuGet Packages
| Package | Version |
| :--- | :--- |
| Entatea | ![Nuget](https://img.shields.io/nuget/v/Entatea) |
| Entatea.SqlServer | ![Nuget](https://img.shields.io/nuget/v/Entatea.SqlServer) |
| Entatea.MySql | ![Nuget](https://img.shields.io/nuget/v/Entatea.MySql) |
| Entatea.Sqlite | ![Nuget](https://img.shields.io/nuget/v/Entatea.Sqlite) |
| Entatea.InMemory | ![Nuget](https://img.shields.io/nuget/v/Entatea.InMemory) |

# Entatea

Entatea (phonectic-ish spelling of Entity) provides a simple data context for database operations, including;

+ Create
+ Read (with paging and predicates system)
+ Update
+ Delete

Entatea includes an In Memory implementation which can be used to for unit/integration tests and proof of concepts.
The purpose of the In Memory implemenation is two-fold;

1. to reduce the number of stubs/mocks required when unit testing data access scenarios
2. to reduce friction moving from proof of concept to product

Both of the above should also boost that magic metric... developer productivity! *yay*

Entatea is not intended to be THE solution for every database interaction, if you need something more complex, or need to tune performance then you will likely still be better off writing your own repositories for those scenarios.

For everything else there is Entatea.

Entatea is inspired by a number of existing libraries including Dapper.SimpleCRUD and Dapper-Extensions from .NET and Ebeans from well the JVM world.
