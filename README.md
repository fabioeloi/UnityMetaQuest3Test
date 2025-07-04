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

1.  **XR Event Trigger**: Unity's XR Interaction Toolkit detects user actions on GameObjects. This includes:
    *   `selectEntered`: Typically when a user grabs an object.
    *   `selectExited`: When a user releases a grabbed object.
    *   `activated`: When a user activates an object (e.g., presses a button on it, or uses a tool).
    *   `hoverEntered`: When a user's controller pointer hovers over an object.
    *   `hoverExited`: When the controller pointer stops hovering over an object.
2.  **Event Handling**: `VRInteractionController` (Presentation Layer) listens to these XR events from `XRBaseInteractable` components.
3.  **Entity Mapping**:
    *   The `VRInteractionController` uses `UnityVRInteractionRepository` (Infrastructure Layer) to find the domain entity ID associated with the interacted GameObject via its `EntityIdentifier` component.
4.  **Service Call**: Based on the XR event, `VRInteractionController` calls the appropriate method on `IVRInteractionService`:
    *   `selectEntered` -> `ProcessGrabInteraction(entityId, interactorName)`
    *   `selectExited` -> `ProcessReleaseInteraction(entityId, interactorName)`
    *   `activated` -> `ProcessUseInteraction(entityId, interactorName)`
    *   `hoverEntered` -> `ProcessHoverEnter(entityId, interactorName)`
    *   `hoverExited` -> `ProcessHoverExit(entityId, interactorName)`
5.  **Domain Logic Execution**:
    *   The `IVRInteractionService` (Application Layer) retrieves the `VRInteractionEntity`.
    *   It then invokes the corresponding method on the entity (e.g., `Grab()`, `Release()`, `Use()`, `HoverEnter()`, `HoverExit()`).
6.  **Entity Reaction & State Update**:
    *   The methods within `VRInteractionEntity` (Domain Layer) execute specific business logic.
    *   This includes updating entity state, such as `IsGrabbed` (true on grab, false on release) and `IsHovered` (true on hover enter, false on hover exit), and incrementing `InteractionCount`.
    *   Appropriate log messages are generated.
7.  **Visual Feedback (Hover)**:
    *   When `IVRInteractionService` calls `_repository.Update(entity)` after a hover state change, the `UnityVRInteractionRepository` detects the change in `entity.IsHovered`.
    *   It then updates the material color of the corresponding GameObject (e.g., to yellow for hover, back to white for non-hover) providing visual feedback.

This flow ensures that specific XR events are mapped to distinct domain actions and state changes, including visual cues for hover interactions.

### Key Interaction Features
- **Grab**: Objects can be picked up. Their state changes to `IsGrabbed = true`.
- **Release**: Grabbed objects can be let go. Their state changes to `IsGrabbed = false`.
- **Use**: Objects can be "used" (e.g., pressing a button on them).
- **Hover**: Pointing at objects provides visual feedback (color change) and updates their `IsHovered` state.
- **Interaction Tracking**: All interactions update `WasInteracted` and `InteractionCount` on the entity.

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