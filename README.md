
# WebApiControllers

This is a learning project focused on developing a RESTful API with .NET 8, featuring JWT authentication, automated testing with xUnit, containerization using Docker, and Continuous Integration (CI) via GitHub Workflows.

## Technologies Used

- .NET 8
- ASP.NET Core Web API
- JWT (JSON Web Token) for authentication and security
- xUnit for unit testing
- Docker for containerization
- GitHub Actions for CI/CD

## Features

- REST endpoints for CRUD operations
- Secure authentication using JWT
- Automated tests covering application logic
- CI pipeline configured for build, testing, and static analysis
- Docker container configured for easy deployment and development

## Running Locally

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional, for running via container)

### Steps to run locally

1. Clone the repository:

```bash
git clone https://github.com/giovannimedici/webapicontrollers.git
cd webapicontrollers
```

2. Restore packages and run the application:

```bash
dotnet restore
dotnet run --project src/WebApiControllers.Api
```

3. Access the API at `https://localhost:5001` (or configured port)

### Running with Docker

```bash
docker build -t webapicontrollers .
docker run -p 5000:80 webapicontrollers
```

The API will be available at `http://localhost:5000`.

## Tests

To run automated tests:

```bash
dotnet test
```

## Continuous Integration (CI)

This project includes a GitHub Actions workflow that automatically builds and tests the code on each push or pull request, ensuring continuous quality.

## Importance of Security with JWT

JWT-based authentication ensures the API only allows authorized users to access protected resources, safeguarding sensitive data and preventing unauthorized access — a critical feature for modern APIs.

---

## Contributions

Contributions are welcome! Feel free to open issues or pull requests to suggest improvements.

## License

MIT License © Giovanni Medici
