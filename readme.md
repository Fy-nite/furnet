# FurNet - Finite User Repository 

## Overview

FurNet is a web application for the Finite User Repository (FUR) package management system. It provides a user-friendly interface to browse, search, and submit packages to the FUR ecosystem.

## Features

- **Package Browsing**: View all available packages with sorting and filtering options
- **Package Details**: View comprehensive information about each package
- **Package Submission**: Submit new packages to the repository through a web interface
- **API Integration**: Seamlessly communicates with the FUR API backend
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
    git clone https://github.com/fy-nite/furnet.git
    cd furnet
    ```

2. Run the application
    ```bash
    dotnet run
    ```

3. Open your browser to `https://localhost:5001` or `http://localhost:5000`

## API Documentation

FurNet integrates with the FUR API. For more details on available endpoints, refer to [api.md](api.md).

## Package Configuration

FurNet uses `furconfig.json` files to define package metadata. For detailed specifications, see [furconfig.md](furconfig.md).

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

- Project: FUR (Finite User Repository)
- Website: [https://github.com/fy-nite/furnet](https://github.com/fy-nite/furnet)
- Issues: Please report issues through our issue tracker