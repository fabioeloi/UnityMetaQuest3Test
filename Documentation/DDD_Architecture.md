# Domain-Driven Design Architecture

This document explains the Domain-Driven Design (DDD) architecture implemented in the Meta Quest 3 VR test project.

## Key Concepts

DDD is a software design approach that focuses on modeling the software to match the business domain. In our VR context, we've applied these principles to create a clean, maintainable architecture for VR interactions.

## Layers

### Domain Layer

The core of our application containing the business logic with no dependencies on other layers.

**Components:**
- `VRInteractionEntity`: Core domain entity representing an interactable object in VR
- `IVRInteractionRepository`: Repository interface for persistence operations
- `IVRInteractionService`: Domain service interface for VR interactions

**Key Characteristics:**
- Contains business rules and logic
- Uses value objects (e.g., `Position`)
- Has no dependencies on external frameworks or libraries
- Implements domain events for state changes

### Application Layer

Coordinates the domain objects to perform specific application tasks.

**Components:**
- `VRInteractionService`: Implementation of domain services
- Use cases for VR interactions

**Key Characteristics:**
- Depends on the domain layer
- Orchestrates domain objects to perform tasks
- Doesn't contain business rules
- Translates between the domain and external layers

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
- `VRInteractionController`: Connects Unity's XR system with our domain model
- `VRSceneSetup`: Sets up the Unity scene for VR

**Key Characteristics:**
- Contains Unity MonoBehaviour components
- Handles user input and display
- Uses application layer services to perform operations
- Maps between Unity concepts and domain concepts

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

The following steps describe a typical XR interaction flow:

1.  **User Interaction**: The user performs an action (e.g., grabbing or using) on a GameObject in the VR scene using their controller.
2.  **XR Event Detection**: Unity's XR Interaction Toolkit (part of the underlying XR system) detects this physical interaction and raises an event (e.g., `selectEntered`, `activated`) on the corresponding `XRBaseInteractable` component of the GameObject.
3.  **Event Handling by `VRInteractionController`**:
    *   The `VRInteractionController` (Presentation Layer) has previously subscribed its event handler methods to these XR events from the `XRBaseInteractable` components of managed GameObjects.
    *   When an event is triggered, the appropriate handler in `VRInteractionController` is invoked.
4.  **Entity Identification**:
    *   The handler method receives event arguments containing the interacted `GameObject`.
    *   The `VRInteractionController` uses the `UnityVRInteractionRepository.GetEntityIdByGameObject()` method (Infrastructure Layer) to retrieve the domain entity's ID. This method looks for an `EntityIdentifier` component on the `GameObject` to get the ID.
5.  **Application Service Call**:
    *   The `VRInteractionController` calls the `IVRInteractionService.ProcessInteraction(entityId, interactorName, interactionEvent)` method (Application Layer), passing the identified entity ID, the name of the interactor, and the type of interaction event (e.g., `Selected`, `Activated`).
6.  **Domain Logic Execution**:
    *   The `VRInteractionService` retrieves the `VRInteractionEntity` from the `UnityVRInteractionRepository` using the entity ID.
    *   It then calls the `Interact()` method on the retrieved `VRInteractionEntity` instance.
7.  **Entity State Update**:
    *   The `VRInteractionEntity.Interact()` method (Domain Layer) executes its specific business logic. This typically involves updating its state (e.g., setting `WasInteracted = true`, incrementing `InteractionCount`) and logging the interaction.
8.  **Feedback (Optional)**: If the interaction results in changes to the entity's state that need to be reflected in the Unity scene (e.g., position, appearance), this would typically involve the Application Layer notifying the Presentation Layer, or the Presentation Layer observing domain events. For this project, the primary feedback is logging.

This flow demonstrates how user actions in the VR environment are translated into domain-specific operations, maintaining a separation of concerns between the XR hardware/SDK specifics and the core application logic. The `UnityVRInteractionRepository` plays a crucial role in creating and configuring the GameObjects with appropriate XR components based on domain entity properties, and the `EntityIdentifier` component links these GameObjects back to their domain counterparts.