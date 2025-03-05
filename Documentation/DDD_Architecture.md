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
- `UnityVRInteractionRepository`: Unity-specific implementation of repository
- Setup scripts for development environment and deployment

**Key Characteristics:**
- Implements persistence, framework integration, etc.
- Contains Unity-specific code and integration
- Depends on the domain and application layers
- Handles technical concerns like GameObject management

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

1. User interacts with a VR object via controller
2. Unity's XR system detects the interaction
3. `VRInteractionController` receives the event
4. Controller calls the application service
5. Service performs operations using domain entities
6. Repository persists any changes
7. Results flow back through the layers to update the Unity scene