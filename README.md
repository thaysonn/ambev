# Developer Evaluation API

This project is a .NET 8 Web API that can be run either in a **local development environment** using Visual Studio or via **Docker Compose**.

It includes automatic **database migrations** and exposes **Swagger** for API documentation and testing.

---

## 🚀 Running in Development (Visual Studio)

1. Clone the repository to your machine.
2. Open the solution in **Visual Studio**.
3. Ensure you have **PostgreSQL** running locally.
4. Create a **blank database** in PostgreSQL with the credentials configured in `appsettings.json` (default: `developer_evaluation`).
5. Run the project with the **https** profile in Visual Studio.

By default, the API will be available at:
https://localhost:7181/swagger/index.html



Swagger will automatically load so you can test the API endpoints.

---

## 🐳 Running with Docker Compose

1. Ensure Docker and Docker Compose are installed on your machine.
2. In the root folder of the project (backend), run:

```bash
docker compose up -d --build
````

By default, the API will be available at: 
http://localhost:8080/swagger/index.html


📌 Notes

    Swagger is enabled for both local development and Docker environments.

    Database migrations run automatically — no need for manual dotnet ef commands.

    Default database is PostgreSQL. Connection string can be configured in appsettings.json or via environment variables.

    When running locally (Visual Studio), you must create an empty PostgreSQL database before starting the application.

    When running with Docker Compose, PostgreSQL is included as a service and initialized automatically.
