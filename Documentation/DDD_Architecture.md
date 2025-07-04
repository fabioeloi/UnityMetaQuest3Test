# Domain-Driven Design Architecture

This document explains the Domain-Driven Design (DDD) architecture implemented in the Meta Quest 3 VR test project.

## Key Concepts

DDD is a software design approach that focuses on modeling the software to match the business domain. In our VR context, we've applied these principles to create a clean, maintainable architecture for VR interactions.

## Layers

### Domain Layer

The core of our application containing the business logic with no dependencies on other layers.

**Components:**
- `VRInteractionEntity`: Core domain entity representing an interactable object in VR. It now includes:
    - Properties:
        - `IsGrabbed`: Boolean state indicating if the entity is currently grabbed.
        - `IsHovered`: Boolean state indicating if the entity is currently being hovered over.
    - Specific interaction methods:
        - `Grab(string interactorId)`: Sets `IsGrabbed = true`, updates interaction counters, and logs.
        - `Release(string interactorId)`: Sets `IsGrabbed = false` and logs.
        - `Use(string interactorId)`: Handles "use" logic, updates interaction counters, and logs.
        - `HoverEnter(string interactorId)`: Sets `IsHovered = true` and logs.
        - `HoverExit(string interactorId)`: Sets `IsHovered = false` and logs.
    - The generic `Interact()` method is marked `[System.Obsolete]`.
- `IVRInteractionRepository`: Repository interface for persistence of `VRInteractionEntity` instances.
- `IVRInteractionService`: Domain service interface defining high-level operations.

**Key Characteristics:**
- Contains business rules and logic, primarily within `VRInteractionEntity` (e.g., state changes like `IsGrabbed`, `IsHovered`).
- Uses value objects (e.g., `Position`)
- Has no dependencies on external frameworks or libraries
- Implements domain events for state changes

### Application Layer

Coordinates the domain objects to perform specific application tasks.

**Components:**
- `VRInteractionService`: Implementation of `IVRInteractionService`. This service now includes methods for each specific interaction type:
    - `ProcessGrabInteraction(interactableId, interactorId)`: Retrieves entity, calls `entity.Grab()`, updates repository.
    - `ProcessReleaseInteraction(interactableId, interactorId)`: Retrieves entity, calls `entity.Release()`, updates repository.
    - `ProcessUseInteraction(interactableId, interactorId)`: Retrieves entity, calls `entity.Use()`, updates repository.
    - `ProcessHoverEnter(interactableId, interactorId)`: Retrieves entity, calls `entity.HoverEnter()`, updates repository.
    - `ProcessHoverExit(interactableId, interactorId)`: Retrieves entity, calls `entity.HoverExit()`, updates repository.
    - The generic `ProcessInteraction()` method is marked `[System.Obsolete]`.
- Use cases for VR interactions are orchestrated by this service.

**Key Characteristics:**
- Depends on the domain layer (`IVRInteractionRepository`, `VRInteractionEntity`).
- Orchestrates domain objects for specific interaction types.
- Delegates business rule execution to domain entities.
- Triggers repository updates, which in turn can lead to visual feedback (e.g., for hover).

### Infrastructure Layer

Provides implementations for interfaces defined in the domain layer.

**Components:**
- `UnityVRInteractionRepository`: Unity-specific implementation of `IVRInteractionRepository`. It manages the lifecycle of GameObjects corresponding to `VRInteractionEntity` instances. This includes:
    - Creating GameObjects.
    - Attaching the `EntityIdentifier` component to link the GameObject to its domain entity ID.
    - Dynamically adding necessary XR components based on `VRInteractionEntity` properties.
    - **Hover Visual Feedback**: When its `Update(VRInteractionEntity entity)` method is called (typically by `VRInteractionService` after an entity state change), it checks the `entity.IsHovered` state. If `IsHovered` is true, it changes the material color of the corresponding GameObject to `hoverColor`; otherwise, it reverts to `originalColor`. This provides immediate visual feedback for hover interactions. It has public `originalColor` and `hoverColor` fields for configuration.
    - In its `Add()` method, it also ensures new GameObjects are given a `MeshRenderer`, a primitive mesh (cube), and an initial material set to `originalColor` so they are visible and ready for color changes.
- `EntityIdentifier`: A MonoBehaviour component that links a GameObject to its domain entity ID.
- Setup scripts for development environment and deployment.

**Key Characteristics:**
- Implements persistence and framework integration.
- Contains Unity-specific code for GameObject creation, XR component setup, and visual feedback mechanisms like color changes.
- Depends on the domain and application layers.
- Bridges Unity's visual/physical world with the domain model.

### Presentation Layer

Handles the user interface and interactions with the system.

**Components:**
- `VRInteractionController`: Connects Unity's XR Interaction Toolkit events with the application's domain logic by handling various `XRBaseInteractable` events:
    - `selectEntered` -> `HandleSelectEntered` -> `IVRInteractionService.ProcessGrabInteraction()`
    - `selectExited` -> `HandleSelectExited` -> `IVRInteractionService.ProcessReleaseInteraction()`
    - `activated` -> `HandleActivated` -> `IVRInteractionService.ProcessUseInteraction()`
    - `hoverEntered` -> `HandleHoverEntered` -> `IVRInteractionService.ProcessHoverEnter()`
    - `hoverExited` -> `HandleHoverExited` -> `IVRInteractionService.ProcessHoverExit()`
    - This ensures a clear mapping from specific XR hardware events to appropriate domain responses.
- `VRSceneSetup`: (If exists) Sets up the Unity scene for VR.

**Key Characteristics:**
- Contains Unity MonoBehaviour components.
- Listens to and translates specific XR events into calls to the application service.
- Uses application layer services to trigger domain logic.
- Decouples XR hardware specifics from the application and domain layers.

## Benefits of DDD in VR Development

1. **Clear Boundaries**: DDD helps clearly separate VR interaction logic from Unity-specific implementation
2. **Testability**: Core domain logic can be tested without Unity dependencies
3. **Maintainability**: Changes to Unity's XR system require updates only in the infrastructure layer
4. **Flexibility**: Can swap out VR frameworks without affecting core business logic

## Implemented Patterns

- **Repository Pattern**: Abstracts data storage operations
- **Service Pattern**: Encapsulates domain operations
- **Value Objects**: Immutable objects that represent concepts with no identity
- **Entity**: Objects with distinct identity and lifecycle
- **Domain Events**: For communicating changes in the domain

## Example Flow

The following examples illustrate the refined interaction flows:

**A. Grab and Release Cycle:**

1.  **User Action (Grab)**: User points at a grabbable GameObject and presses the grab button.
2.  **XR Event (`selectEntered`)**: Detected by XR Interaction Toolkit on the `XRGrabInteractable`.
3.  **`VRInteractionController` (`HandleSelectEntered`)**:
    *   Identifies entity ID via `UnityVRInteractionRepository.GetEntityIdByGameObject()`.
    *   Calls `IVRInteractionService.ProcessGrabInteraction(entityId, interactorName)`.
4.  **`VRInteractionService` (`ProcessGrabInteraction`)**:
    *   Retrieves `VRInteractionEntity`.
    *   Calls `entity.Grab(interactorName)`.
5.  **`VRInteractionEntity` (`Grab`)**:
    *   Sets `IsGrabbed = true`.
    *   Updates `WasInteracted`, `InteractionCount`.
    *   Logs "GRABBED" message.
6.  **`VRInteractionService`**: Calls `_repository.Update(entity)`.
7.  **User Action (Release)**: User releases the grab button.
8.  **XR Event (`selectExited`)**: Detected on the `XRGrabInteractable`.
9.  **`VRInteractionController` (`HandleSelectExited`)**:
    *   Identifies entity ID.
    *   Calls `IVRInteractionService.ProcessReleaseInteraction(entityId, interactorName)`.
10. **`VRInteractionService` (`ProcessReleaseInteraction`)**:
    *   Retrieves `VRInteractionEntity`.
    *   Calls `entity.Release(interactorName)`.
11. **`VRInteractionEntity` (`Release`)**:
    *   Sets `IsGrabbed = false`.
    *   Logs "RELEASED" message.
12. **`VRInteractionService`**: Calls `_repository.Update(entity)`.

**B. Hover Enter and Exit Cycle (with Visual Feedback):**

1.  **User Action (Hover Enter)**: User's controller pointer moves over an interactable GameObject.
2.  **XR Event (`hoverEntered`)**: Detected on the `XRBaseInteractable`.
3.  **`VRInteractionController` (`HandleHoverEntered`)**:
    *   Identifies entity ID.
    *   Calls `IVRInteractionService.ProcessHoverEnter(entityId, interactorName)`.
4.  **`VRInteractionService` (`ProcessHoverEnter`)**:
    *   Retrieves `VRInteractionEntity`.
    *   Calls `entity.HoverEnter(interactorName)`.
5.  **`VRInteractionEntity` (`HoverEnter`)**:
    *   Sets `IsHovered = true`.
    *   Logs "HOVER ENTER" message.
6.  **`VRInteractionService`**: Calls `_repository.Update(entity)`.
7.  **`UnityVRInteractionRepository` (`Update`)**:
    *   Detects `entity.IsHovered == true`.
    *   Changes the GameObject's material color to `hoverColor`.
8.  **User Action (Hover Exit)**: User's controller pointer moves off the GameObject.
9.  **XR Event (`hoverExited`)**: Detected on the `XRBaseInteractable`.
10. **`VRInteractionController` (`HandleHoverExited`)**:
    *   Identifies entity ID.
    *   Calls `IVRInteractionService.ProcessHoverExit(entityId, interactorName)`.
11. **`VRInteractionService` (`ProcessHoverExit`)**:
    *   Retrieves `VRInteractionEntity`.
    *   Calls `entity.HoverExit(interactorName)`.
12. **`VRInteractionEntity` (`HoverExit`)**:
    *   Sets `IsHovered = false`.
    *   Logs "HOVER EXIT" message.
13. **`VRInteractionService`**: Calls `_repository.Update(entity)`.
14. **`UnityVRInteractionRepository` (`Update`)**:
    *   Detects `entity.IsHovered == false`.
    *   Changes the GameObject's material color back to `originalColor`.

**C. Use Interaction Example:** Remains largely the same as previously described, but now uses `ProcessUseInteraction` and `entity.Use()`.

This detailed flow illustrates how specific user actions are mapped through the layers to specific domain logic and state changes, including visual feedback for hover states.