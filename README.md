# Introduction 
Implementation to address the depth chart specifications.

This solution is constrained to the code chalendge only, which the implemented architecture does not cater for the needs from a real application running in production, like authentication + authorization, logging, real database  and etc.

This solution uses an in memory database, so every time you run the solution it will start with an empty database.

The implementation uses the concept of Clean Architecture + Domain Driven Design

For a real world application I would have used the [CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture) template from Jason Taylor

For a real world scenario I would also have had written integration tests using SpecFlow to come up with different scenarios. This integration tests would target the APIs inside the "DepthChartsWeb" project. That would not only increase the code coverage but would also be testing the data layer.

Note that for the queries written using Entity Framework, I am not taking performance into consideration for this exercise

Note that the APIs is driven by plain text, in real world scenario they would be driven by IDs considering we would have UI

# Getting Started
To run + analyze the implemented solution, please use one IDE of your choice like: Visual Studio, Rider or VS Code

Please follow the steps below to get started:
1. Open the "CodingChallengeSB.sln" solution file using your IDE;
2. Setup the "DepthChartsWeb" project as the start up project;
3. Run the solution either in "Debug" or "Release" mode;
4. The project will be running on https://localhost:7034/swagger
5. You can use swagger for http requests;

# Tests
Unit tests are under the project "DepthChartsAppUnitTests"

# Assumptions

1. Depth position start from zero, not one;
2. There can be no gaps in between depth positions;
3. For the scenario "getBackups(“QB”, JaelonDarden)" where the expected output is #10 – Scott Miller, I am assuming the specification is wrong, and for Scott Miller to be returned we should call "getBackups(“LWR”, JaelonDarden)"

# How do we scale the solution ?
### Adding all the NFL teams?
We can create a new entity "Team". Note that the "DepthChart" entity has "TeamId". Each depth chart will belong to one team

### Adding more Sports? MLB, NHL, NBA
We create an entity "Sport". Each sport can have many teams

