# .NET Refactoring Challenge

This repository contains a sample project designed to practice and improve your refactoring skills. The project includes a database schema, sample code, and various challenges that encourage you to apply best practices in software design and code refactoring.

## Table of Contents
- [Overview](#overview)
- [Database Schema](#database-schema)
- [Setup](#setup)
- [Tasks](#tasks)
- [License](#license)

## Overview
The **Refactoring Challenge** is a practical exercise for developers who want to:
- Improve their code readability and maintainability.
- Learn and apply design patterns.

This project uses a relational database to model a simple e-commerce system, including customers, products, orders, and inventory management.

## Database Schema
The repository provides an SQL script (`DatabaseSchemaScript.sql`) for setting up the initial database schema. The schema includes the following tables:
- **`Customers`**: Stores customer information.
- **`Products`**: Contains product details.
- **`Inventory`**: Tracks stock levels for each product.
- **`Orders`**: Manages customer orders and their statuses.
- **`OrderItems`**: Represents individual items within an order.
- **`OrderLogs`**: Logs actions and changes related to orders.

Refer to `DatabaseSchemaScript.sql` for the full schema definition.

## Setup
Follow these steps to set up and run the project:

### Requirements
- **Docker**: Required to run the SQL Server instance.
- **.NET SDK**: For running the application.

### Steps
1. Fork this repository to your own GitHub account:
    - Click the `Fork` button in the top-right corner of this repository.

2. Clone your forked repository:
   ```bash
   git clone https://github.com/your-username/dotnet-refactoring-challenge.git
   cd refactoring-challenge
   ```

3. Set up whole environment using Docker:
    ```bash
      docker-compose up -d
    ```

4. Create the database schema by `DatabaseSchema.sql`.

## Tasks
As part of this challenge, you are expected to complete the following tasks:

1. **Refactor the Codebase**:
    - Use appropriate design patterns such as **Repository** and **Service** to improve modularity and maintainability.
    - Separate the data access layer from the business logic (e.g., using the Repository pattern).

2. **Implement Dependency Injection**:
    - Use a Dependency Injection framework to inject dependencies where needed.
    - Ensure that the application adheres to the Inversion of Control principle.

3. **Write Unit Tests**:
    - Write unit tests for the business logic.

## License
This project is licensed under the [MIT License](LICENSE).

