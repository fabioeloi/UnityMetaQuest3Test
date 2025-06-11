# Domain-Driven Design Architecture

This document explains the Domain-Driven Design (DDD) architecture implemented in the Meta Quest 3 VR test project.

## Key Concepts

DDD is a software design approach that focuses on modeling the software to match the business domain. In our VR context, we've applied these principles to create a clean, maintainable architecture for VR interactions.

## Layers

### Domain Layer

The core of our application containing the business logic with no dependencies on other layers.

**Components:**
- `VRInteractionEntity`: Core domain entity representing an interactable object in VR. It now includes specific interaction methods:
    - `Grab(string interactorId)`: Handles the logic when the entity is grabbed.
    - `Use(string interactorId)`: Handles the logic when the entity is used (e.g., a button is pressed).
    - The generic `Interact()` method is now marked as `[System.Obsolete("Use Grab() or Use() instead.")]` to guide developers towards the more explicit interaction methods.
- `IVRInteractionRepository`: Repository interface for persistence operations of `VRInteractionEntity` instances.
- `IVRInteractionService`: Domain service interface defining high-level operations for VR interactions.

**Key Characteristics:**
- Contains business rules and logic, encapsulated within entities like `VRInteractionEntity`.
- Uses value objects (e.g., `Position`)
- Has no dependencies on external frameworks or libraries
- Implements domain events for state changes

### Application Layer

Coordinates the domain objects to perform specific application tasks.

**Components:**
- `VRInteractionService`: Implementation of `IVRInteractionService`. This service now includes:
    - `ProcessGrabInteraction(string interactableId, string interactorId)`: Coordinates the process of a grab interaction, retrieving the entity and calling its `Grab()` method.
    - `ProcessUseInteraction(string interactableId, string interactorId)`: Coordinates the process of a use interaction, retrieving the entity and calling its `Use()` method.
    - The generic `ProcessInteraction(string interactableId, string interactorId)` method is marked as `[System.Obsolete("Use ProcessGrabInteraction() or ProcessUseInteraction() instead.")]`.
- Use cases for VR interactions are orchestrated by this service.

**Key Characteristics:**
- Depends on the domain layer (specifically `IVRInteractionRepository` and `VRInteractionEntity`).
- Orchestrates domain objects to perform tasks based on specific interaction types.
- Doesn't contain business rules itself but delegates to domain entities.
- Translates between the presentation layer's requests and domain layer actions.

### Infrastructure Layer

Provides implementations for interfaces defined in the domain layer.

**Components:**
- `UnityVRInteractionRepository`: Unity-specific implementation of `IVRInteractionRepository`. It manages the lifecycle of GameObjects corresponding to `VRInteractionEntity` instances. This includes:
    - Creating GameObjects.
    - Attaching the `EntityIdentifier` component to link the GameObject to its domain entity ID.
    - Dynamically adding necessary XR components (e.g., `XRGrabInteractable`, `XRSimpleInteractable`, `Rigidbody`, `Collider`) to the GameObject based on the properties of the `VRInteractionEntity` (e.g., `IsGrabbable`, `IsUsable`).
- `EntityIdentifier`: A MonoBehaviour component that holds the ID of the domain `VRInteractionEntity` it represents. This allows the system to retrieve the domain entity associated with a given Unity GameObject.
- Setup scripts for development environment and deployment.

**Key Characteristics:**
- Implements persistence, framework integration, etc.
- Contains Unity-specific code and integration for XR components and GameObject management.
- Depends on the domain and application layers.
- Handles technical concerns like bridging Unity's GameObject world with the domain model.

### Presentation Layer

Handles the user interface and interactions with the system.

**Components:**
- `VRInteractionController`: Connects Unity's XR Interaction Toolkit events with the application's domain logic.
    - It subscribes to `selectEntered` events (typically from `XRGrabInteractable`) and routes these to the `ProcessGrabInteraction` method of the `IVRInteractionService`.
    - It subscribes to `activated` events (from `XRBaseInteractable`, which includes `XRGrabInteractable` and `XRSimpleInteractable`) and routes these to the `ProcessUseInteraction` method of the `IVRInteractionService`.
    - This allows a clear distinction between grabbing an object and using an object (which might be separate actions even for the same grabbable object).
- `VRSceneSetup`: (If exists) Sets up the Unity scene for VR, potentially placing initial interactable objects.

**Key Characteristics:**
- Contains Unity MonoBehaviour components.
- Handles user input events from the XR system.
- Uses application layer services (`IVRInteractionService`) to perform domain-specific operations based on the type of XR event detected.
- Maps between Unity concepts (GameObjects, XR events) and domain concepts (entity IDs, interaction types).

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

The following steps describe how specific XR interactions (Grab and Use) are processed:

**A. Grab Interaction Example:**

1.  **User Action**: The user aims their controller at a grabbable GameObject and presses the grab button.
2.  **XR Event (Select Entered)**: Unity's XR Interaction Toolkit detects this and triggers a `selectEntered` event on the `XRGrabInteractable` component of the GameObject.
3.  **`VRInteractionController` Handles `selectEntered`**:
    *   The `HandleSelectEntered` method in `VRInteractionController` is invoked.
    *   It retrieves the `GameObject` from the event arguments.
4.  **Entity Identification**:
    *   `VRInteractionController` calls `UnityVRInteractionRepository.GetEntityIdByGameObject()` to get the domain entity's ID using the `EntityIdentifier` component on the GameObject.
5.  **Specific Service Call (`ProcessGrabInteraction`)**:
    *   `VRInteractionController` calls `IVRInteractionService.ProcessGrabInteraction(entityId, interactorName)`.
6.  **Application Service Logic (`ProcessGrabInteraction`)**:
    *   `VRInteractionService` retrieves the `VRInteractionEntity` from the repository.
    *   It checks if the entity `IsGrabbable`.
    *   If so, it calls `entity.Grab(interactorName)`.
7.  **Domain Entity Logic (`Grab`)**:
    *   The `VRInteractionEntity.Grab()` method executes its specific logic (e.g., updates `WasInteracted`, `InteractionCount`, logs "GRABBED" message).
8.  **Persistence**: `VRInteractionService` calls `_repository.Update(entity)` to save changes if any.

**B. Use Interaction Example (e.g., pressing a button on an object):**

1.  **User Action**: While an object is selected/hovered, or if it's a static usable object, the user presses an "activate" or "use" button on their controller.
2.  **XR Event (Activated)**: Unity's XR Interaction Toolkit triggers an `activated` event on the `XRBaseInteractable` component (could be `XRGrabInteractable` or `XRSimpleInteractable`).
3.  **`VRInteractionController` Handles `activated`**:
    *   The `HandleActivated` method in `VRInteractionController` is invoked.
    *   It retrieves the `GameObject`.
4.  **Entity Identification**:
    *   Same as in the grab flow, `VRInteractionController` gets the `entityId`.
5.  **Specific Service Call (`ProcessUseInteraction`)**:
    *   `VRInteractionController` calls `IVRInteractionService.ProcessUseInteraction(entityId, interactorName)`.
6.  **Application Service Logic (`ProcessUseInteraction`)**:
    *   `VRInteractionService` retrieves the `VRInteractionEntity`.
    *   It checks if the entity `IsUsable`.
    *   If so, it calls `entity.Use(interactorName)`.
7.  **Domain Entity Logic (`Use`)**:
    *   The `VRInteractionEntity.Use()` method executes its specific logic (e.g., updates state, logs "USED" message).
8.  **Persistence**: `VRInteractionService` calls `_repository.Update(entity)`.

This refactored flow provides a clearer mapping from specific XR controller events to specific domain actions, enhancing the expressiveness and maintainability of the interaction logic.