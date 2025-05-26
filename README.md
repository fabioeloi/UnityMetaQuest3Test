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

### Interaction Flow

The project handles user interactions with VR objects through a defined flow:

1.  **XR Event Trigger**: Unity's XR Interaction Toolkit detects user actions (e.g., grab, use) on GameObjects.
2.  **Event Handling**: `VRInteractionController` (Presentation Layer) listens to these XR events.
3.  **Entity Mapping**:
    *   The `VRInteractionController` uses the `UnityVRInteractionRepository` (Infrastructure Layer) to find the domain entity associated with the interacted GameObject.
    *   This mapping is facilitated by the `EntityIdentifier` component (Infrastructure Layer) attached to GameObjects, which holds the domain entity's ID.
4.  **Service Call**: `VRInteractionController` calls `IVRInteractionService.ProcessInteraction()`, passing the entity ID and interaction type.
5.  **Domain Logic Execution**: The `VRInteractionService` (Application Layer) retrieves the `VRInteractionEntity` and invokes its `Interact()` method.
6.  **Entity Reaction**: The `VRInteractionEntity.Interact()` method (Domain Layer) contains the specific business logic for how the entity responds to the interaction (e.g., changing state, logging).

This flow ensures that Unity-specific XR events are translated into domain-specific actions, keeping the core logic independent of the XR implementation details.

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
3. Select "Edit Mode" tab to run domain and application layer unit tests.
4. Select "Play Mode" tab to run tests that involve MonoBehaviour behaviors and interactions within a simulated Unity environment (like the XR interaction flow tests).
5. Click "Run All" in the respective tab to execute tests.

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