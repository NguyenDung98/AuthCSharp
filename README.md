# AuthC# Project

This repository contains a simple authentication project built with C#. It demonstrates basic authentication and authorization concepts.

## Features

- User registration and login with hashed passwords (BCrypt)
- JWT-based authentication
- Refresh token support
- User sign-out (token revocation)
- MySQL database integration via Entity Framework Core

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 9.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)

## Getting Started

1. **Clone the repository:**
    ```bash
    git clone https://github.com/NguyenDung98/AuthCSharp.git
    cd AuthCSharp
    ```

2. **Configure your database and JWT settings:**

    Create a file named `appsettings.Development.json` in the `AuthC#` project directory with the following content:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "server=localhost;database=authcsharpdb;user=root;password=yourpassword"
      },
      "Jwt": {
        "Key": "your_super_secret_key_here",
        "Issuer": "yourdomain.com",
        "Audience": "yourdomain.com",
        "ExpiresInMinutes": "60"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    ```

    > **Note:** Replace the connection string and JWT values with your own.

3. **Restore dependencies:**
    ```bash
    dotnet restore
    ```

4. **Apply database migrations:**
    ```bash
    dotnet ef database update --project AuthC#
    ```

5. **Build the project:**
    ```bash
    dotnet build
    ```

6. **Run the project:**
    ```bash
    dotnet run --project AuthC#
    ```

    The API will be available at `https://localhost:7163` or `http://localhost:5195` (see launch settings).

## Testing

To run the tests:

```bash
dotnet test
```

## License

This project is licensed under the MIT License.