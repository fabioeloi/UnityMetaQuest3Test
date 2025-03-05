# Unity Meta Quest 3 Test Project

A test project for Meta Quest 3 virtual environment development using Infrastructure as Code (IaC), Test-Driven Development (TDD), Domain-Driven Design (DDD), and Agile methodologies.

## Project Overview

This project demonstrates a structured approach to VR development for Meta Quest 3 using:

- **Domain-Driven Design (DDD)**: Clear separation of concerns with distinct layers
- **Test-Driven Development (TDD)**: Unit tests for core business logic
- **Infrastructure as Code (IaC)**: Automated environment setup with shell scripts and GitHub Actions
- **Agile Methodologies**: Project managed using GitHub Projects with a Kanban board

## Architecture

The project follows a layered architecture based on DDD principles:

- **Domain Layer**: Core business logic, entities, and interfaces
  - VR interaction entities
  - Domain services
  - Repository interfaces
- **Application Layer**: Use cases and application services
  - Implementation of domain services
  - Coordination between domain and infrastructure
- **Infrastructure Layer**: Technical implementations
  - Unity-specific repository implementations
  - Setup scripts and tooling
- **Presentation Layer**: User interface components
  - Unity MonoBehaviour components
  - Scene setup and XR integration

## Development Setup

### Prerequisites

- macOS (script designed for MacBook Pro M1 Pro)
- Unity Hub
- Unity 2022.3 LTS or newer with Android Build Support
- Git
- GitHub CLI

### Automated Setup

1. Clone the repository:
   ```
   git clone https://github.com/fabioeloi/UnityMetaQuest3Test.git
   cd UnityMetaQuest3Test
   ```

2. Run the setup script:
   ```
   chmod +x Scripts/Infrastructure/setup_dev_environment.sh
   ./Scripts/Infrastructure/setup_dev_environment.sh
   ```

3. Open the project in Unity Hub and install required packages:
   - XR Interaction Toolkit
   - Meta XR Toolkit

## Testing

The project uses Unity's test framework for unit testing. To run the tests:

1. Open the project in Unity
2. Open Test Runner window (Window > General > Test Runner)
3. Select "Edit Mode" tab
4. Click "Run All" to execute tests

## CI/CD Pipeline

The project uses GitHub Actions for continuous integration:

- **Automated Testing**: Runs unit tests on every push and pull request
- **Build Pipeline**: Creates Android builds for Meta Quest 3

## Project Management

This project is managed using GitHub Projects in an Agile manner. View the board at:
https://github.com/users/fabioeloi/projects/5

## Directory Structure

```
UnityMetaQuest3Test/
├── Scripts/
│   ├── Domain/          # Core business logic
│   ├── Application/     # Use cases and services
│   ├── Infrastructure/  # Technical implementations
│   └── Presentation/    # UI components
├── Test/                # Unit tests
├── Unity/               # Unity project files
├── Documentation/       # Project documentation
└── .github/workflows/   # CI/CD configuration
```

## Contributing

1. Create a new branch for your feature
2. Implement changes following TDD approach
3. Submit a pull request
4. Wait for CI pipeline to validate your changes

## License

MIT License