# PurrNet - Finite User Repository 

## Overview

PurrNet is a web application for the Finite User Repository (Purr) package management system. It provides a user-friendly interface to browse, search, and submit packages to the Purr ecosystem.

## Features

- **Package Browsing**: View all available packages with sorting and filtering options
- **Package Details**: View comprehensive information about each package
- **Package Submission**: Submit new packages to the repository through a web interface
- **API Integration**: Seamlessly communicates with the Purr API backend
- **Responsive Design**: Works on desktop and mobile devices

## Technology Stack

- ASP.NET Core 8.0
- Razor Pages
- Bootstrap for styling
- In-memory caching for performance optimization

## Development

### Prerequisites

- .NET SDK 8.0 or later
- Visual Studio 2022 or Visual Studio Code

### Getting Started

1. Clone the repository
    ```bash
    git clone https://github.com/fy-nite/Purrnet.git
    cd Purrnet
    ```

2. Run the application
    ```bash
    dotnet run
    ```

3. Open your browser to `https://localhost:5001` or `http://localhost:5000`

## API Documentation

PurrNet integrates with the Purr API. For more details on available endpoints, refer to [api.md](api.md).

## Package Configuration

PurrNet uses `Purrconfig.json` files to define package metadata. For detailed specifications, see [Purrconfig.md](Purrconfig.md).

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

- Project: Purr (Finite User Repository)
- Website: [https://github.com/fy-nite/Purrnet](https://github.com/fy-nite/Purrnet)
- Issues: Please report issues through our issue tracker

# PurrNet

## Setup

1. Copy `.env.example` to `.env`
2. Update the GitHub OAuth credentials in `.env`:
   - Go to https://github.com/settings/applications/new
   - Create a new OAuth app
   - Set Application name: `PurrNet Testing`
   - Set Homepage URL: `http://testing.finite.ovh:8080`
   - Set Authorization callback URL: `http://testing.finite.ovh:8080/signin-github`
   - Copy Client ID and Client Secret to your `.env` file

## Environment Variables

The application uses the following environment variables:

- `GITHUB_CLIENT_ID`: GitHub OAuth Client ID
- `GITHUB_CLIENT_SECRET`: GitHub OAuth Client Secret  
- `CONNECTION_STRING`: Database connection string (optional)
- `ASPNETCORE_ENVIRONMENT`: Application environment (Development/Production)

## Running the Application

```bash
dotnet restore
dotnet watch run --urls="http://192.168.0.145:8080"
```

For testing mode:
```bash
dotnet watch run --urls="http://192.168.0.145:8080" -- --test
```

## OAuth Setup Notes

When setting up GitHub OAuth for testing with subdomain:
- Homepage URL: `http://testing.finite.ovh:8080`
- Authorization callback URL: `http://testing.finite.ovh:8080/signin-github`
- Make sure your DNS/proxy is correctly forwarding `testing.finite.ovh:8080` to `192.168.0.145:8080`