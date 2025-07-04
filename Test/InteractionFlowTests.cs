using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI; // For EventSystem if needed
using MetaQuestTest.Application;
using MetaQuestTest.Domain;
using MetaQuestTest.Infrastructure;
using MetaQuestTest.Presentation;
using System.Collections;

public class InteractionFlowTests
{
    private XRInteractionManager _interactionManager;
    private VRInteractionController _vrController;
    private IVRInteractionService _interactionService;
    private UnityVRInteractionRepository _interactionRepository;
    private XRDirectInteractor _dummyInteractor; // Using XRDirectInteractor for simplicity

    // IEnumerator used for Setup to allow yielding a frame, ensuring Awake/Start are called.
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // 1. EventSystem
        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<XRUIInputModule>();
        }

        // 2. XRInteractionManager
        var interactionManagerGo = new GameObject("XRInteractionManager");
        _interactionManager = interactionManagerGo.AddComponent<XRInteractionManager>();

        // 3. XROrigin (simplified setup)
        var xrOriginGo = new GameObject("XROrigin");
        var cameraGo = new GameObject("Main Camera");
        cameraGo.transform.SetParent(xrOriginGo.transform);
        cameraGo.AddComponent<Camera>();
        if (Camera.main == null) cameraGo.tag = "MainCamera";

        // 4. Dummy Interactor
        var interactorGo = new GameObject("DummyDirectInteractor");
        interactorGo.transform.SetParent(xrOriginGo.transform);
        _dummyInteractor = interactorGo.AddComponent<XRDirectInteractor>();
        var controller = interactorGo.AddComponent<XRController>();
        _dummyInteractor.xrController = controller;

        // 5. VRInteractionController
        var vrControllerGo = new GameObject("VRInteractionController");
        _vrController = vrControllerGo.AddComponent<VRInteractionController>();

        // Yield a frame to ensure Awake and Start methods of VRInteractionController (and its components) are called.
        yield return null;

        // Retrieve the repository and service instances that VRInteractionController created/uses.
        // VRInteractionController adds UnityVRInteractionRepository on its own GameObject in Awake.
        _interactionRepository = _vrController.GetComponent<UnityVRInteractionRepository>();
        if (_interactionRepository == null)
        {
            Debug.LogError("UnityVRInteractionRepository not found on VRInteractionController GameObject after setup.");
            // Fallback if it was added elsewhere or FindObjectOfType is preferred (though GetComponent is more specific here)
            _interactionRepository = GameObject.FindObjectOfType<UnityVRInteractionRepository>();
        }
        Assert.IsNotNull(_interactionRepository, "UnityVRInteractionRepository must be initialized by VRInteractionController.");

        // VRInteractionController creates its own IVRInteractionService instance using the repository.
        // We need to use this specific service instance or replicate its creation.
        // For simplicity, we create a new service here using the repository instance VRInteractionController uses.
        // This assumes VRInteractionService is stateless or its state is managed via the repository.
        _interactionService = new VRInteractionService(_interactionRepository);
        Assert.IsNotNull(_interactionService, "IVRInteractionService must be initializable for tests.");
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up GameObjects created for the test
        GameObject.Destroy(_dummyInteractor.gameObject.GetComponent<XRController>().gameObject); // Destroy interactor
        GameObject.Destroy(_interactionManager.gameObject);
        GameObject.Destroy(_vrController.gameObject); // This should also destroy repository if it's a component

        // Destroy XROrigin and EventSystem if they were created by setup
        var xrOrigin = GameObject.Find("XROrigin");
        if (xrOrigin != null) GameObject.Destroy(xrOrigin);
        var eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem != null) GameObject.Destroy(eventSystem.gameObject);

        // Other cleanup if necessary
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator TestGrabAndReleaseInteraction_UpdatesStateAndLogs() // Renamed
    {
        // Arrange
        string entityName = "TestGrabbableEntity";
        var entity = _interactionService.RegisterInteractable(entityName, InteractionType.Grab, true, false);
        Assert.IsNotNull(entity, "Entity should be registered.");
        _interactionService.UpdateInteractablePosition(entity.Id, new VRInteractionEntity.Position(0,1,2));
        yield return null;

        GameObject entityGo = _interactionRepository._gameObjectMap[entity.Id];
        Assert.IsNotNull(entityGo, $"GameObject for entity {entity.Id} should exist in repository.");
        var grabInteractable = entityGo.GetComponent<XRGrabInteractable>();
        Assert.IsNotNull(grabInteractable, "XRGrabInteractable component should be on the GameObject.");

        // Act (Grab)
        LogAssert.Expect(LogType.Log, $"Entity '{entity.Name}' GRABBED by interactor '{_dummyInteractor.name}'. Total interactions: 1. IsGrabbed: True");
        Assert.IsTrue(_dummyInteractor.gameObject.activeInHierarchy && _dummyInteractor.enabled, "Interactor should be active and enabled for grab.");
        Assert.IsTrue(grabInteractable.gameObject.activeInHierarchy && grabInteractable.enabled, "Interactable should be active and enabled for grab.");
        _interactionManager.SelectEnter(_dummyInteractor, grabInteractable);
        yield return null;

        // Assert (Grab)
        var grabbedEntity = _interactionService.GetInteractableById(entity.Id);
        Assert.IsNotNull(grabbedEntity, "Grabbed entity should be retrievable.");
        Assert.IsTrue(grabbedEntity.WasInteracted, "Entity's WasInteracted should be true after grab.");
        Assert.AreEqual(1, grabbedEntity.InteractionCount, "Entity's InteractionCount should be 1 after grab.");
        Assert.IsTrue(grabbedEntity.IsGrabbed, "Entity's IsGrabbed should be true after grab.");

        // Act (Release)
        LogAssert.Expect(LogType.Log, $"Entity '{entity.Name}' RELEASED by interactor '{_dummyInteractor.name}'. IsGrabbed: False");
        _interactionManager.SelectExit(_dummyInteractor, grabInteractable);
        yield return null;

        // Assert (Release)
        var releasedEntity = _interactionService.GetInteractableById(entity.Id);
        Assert.IsNotNull(releasedEntity, "Released entity should be retrievable.");
        Assert.IsFalse(releasedEntity.IsGrabbed, "Entity's IsGrabbed should be false after release.");
        // InteractionCount might or might not change on release depending on domain logic, current Release doesn't change it.
        Assert.AreEqual(1, releasedEntity.InteractionCount, "InteractionCount should remain 1 after release if Release doesn't modify it.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUseInteraction_CallsEntityInteractMethod()
    {
        // Arrange
        string entityName = "TestUsableEntity";
        // Register as Usable, not Grabbable, with a distinct InteractionType if desired (e.g., Touch)
        var entity = _interactionService.RegisterInteractable(entityName, InteractionType.Touch, false, true);
        Assert.IsNotNull(entity, "Entity should be registered.");
        _interactionService.UpdateInteractablePosition(entity.Id, new VRInteractionEntity.Position(0,1.5f,2));
        yield return null; // Allow repository to process Add and create/update GameObject

        GameObject entityGo = _interactionRepository._gameObjectMap[entity.Id];
        Assert.IsNotNull(entityGo, $"GameObject for entity {entity.Id} should exist in repository.");

        // For Usable (but not Grabbable) entities, UnityVRInteractionRepository adds XRSimpleInteractable
        var simpleInteractable = entityGo.GetComponent<XRSimpleInteractable>();
        Assert.IsNotNull(simpleInteractable, "XRSimpleInteractable component should be on the GameObject for a usable entity.");

        // Act
        // Updated log message to match VRInteractionEntity.Use()
        LogAssert.Expect(LogType.Log, $"Entity '{entity.Name}' USED by interactor '{_dummyInteractor.name}'. Total interactions: 1.");

        Assert.IsTrue(_dummyInteractor.gameObject.activeInHierarchy && _dummyInteractor.enabled, "Interactor should be active and enabled.");
        Assert.IsTrue(simpleInteractable.gameObject.activeInHierarchy && simpleInteractable.enabled, "Interactable should be active and enabled.");

        // Simulate the activate event for XRSimpleInteractable
        // Note: XRDirectInteractor might not directly "activate" an XRSimpleInteractable in the same way it "selects" a grabbable.
        // Activation is often triggered by a controller button press while hovering or selecting.
        // We need to ensure the VRInteractionController's HandleActivated is connected to simpleInteractable.activated event.
        // The VRInteractionController.SetupXRInteractionEvents should handle this.

        // Direct simulation of activate:
        // The interactor needs to be selecting the interactable first for activate to typically work.
        // For XRSimpleInteractable, it might respond to hover and activate.
        // Let's simulate selection first, then activation.
        _interactionManager.SelectEnter(_dummyInteractor, simpleInteractable); // Select it first
        yield return null; // Process select

        // Now activate
        _interactionManager.Activate(_dummyInteractor, simpleInteractable);

        yield return null; // Wait a frame for event processing by VRInteractionController

        // Assert
        var updatedEntity = _interactionService.GetInteractableById(entity.Id);
        Assert.IsNotNull(updatedEntity, "Updated entity should be retrievable.");
        Assert.IsTrue(updatedEntity.WasInteracted, "Entity's WasInteracted should be true after use.");
        Assert.AreEqual(1, updatedEntity.InteractionCount, "Entity's InteractionCount should be 1 after use.");

        // Clean up selection for this test item
        _interactionManager.SelectExit(_dummyInteractor, simpleInteractable);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestHoverInteraction_UpdatesStateAndLogs()
    {
        // Arrange
        string entityName = "TestHoverableEntity";
        // For hover, it can be any type of interactable (grabbable, usable, or just hoverable)
        var entity = _interactionService.RegisterInteractable(entityName, InteractionType.Point, false, false); // Example: Not grabbable/usable
        Assert.IsNotNull(entity, "Entity should be registered for hover test.");
        _interactionService.UpdateInteractablePosition(entity.Id, new VRInteractionEntity.Position(0,1,3));
        yield return null;

        GameObject entityGo = _interactionRepository._gameObjectMap[entity.Id];
        Assert.IsNotNull(entityGo, $"GameObject for entity {entity.Id} should exist in repository for hover test.");
        // Any XRBaseInteractable can be hovered. If specific components were added for non-grabbable/non-usable,
        // get that. For now, UnityVRInteractionRepository adds BoxCollider. XRBaseInteractable events are on it.
        var interactable = entityGo.GetComponent<XRBaseInteractable>();
        if (interactable == null) // If only BoxCollider was added and no specific XR...Interactable
        {
            // In our current UnityVRInteractionRepository, if not grabbable or usable, no XR component is added.
            // XRInteractionManager.HoverEnter/Exit requires an IXRHoverInteractable.
            // So, for this test to work with current repo, entity must be grabbable or usable.
            // Let's make it usable so XRSimpleInteractable is added.
            entity = _interactionService.RegisterInteractable(entityName, InteractionType.Point, false, true); // Make it usable
            _interactionService.UpdateInteractablePosition(entity.Id, new VRInteractionEntity.Position(0,1,3));
             yield return null; // Re-yield after re-registration
            entityGo = _interactionRepository._gameObjectMap[entity.Id]; // Re-fetch
            interactable = entityGo.GetComponent<XRBaseInteractable>(); // Should now have XRSimpleInteractable
        }
        Assert.IsNotNull(interactable, "An XRBaseInteractable component (e.g., XRSimpleInteractable or XRGrabInteractable) must be on the GameObject for hover events.");

        // Act (Hover Enter)
        LogAssert.Expect(LogType.Log, $"Entity '{entity.Name}' HOVER ENTER by interactor '{_dummyInteractor.name}'. IsHovered: True");
        Assert.IsTrue(_dummyInteractor.gameObject.activeInHierarchy && _dummyInteractor.enabled, "Interactor should be active for hover enter.");
        Assert.IsTrue(interactable.gameObject.activeInHierarchy && interactable.enabled, "Interactable should be active for hover enter.");
        _interactionManager.HoverEnter(_dummyInteractor, interactable);
        yield return null;

        // Assert (Hover Enter)
        var hoveredEntity = _interactionService.GetInteractableById(entity.Id);
        Assert.IsNotNull(hoveredEntity, "Hovered entity should be retrievable.");
        Assert.IsTrue(hoveredEntity.IsHovered, "Entity's IsHovered should be true after hover enter.");
        // Note: InteractionCount is not expected to change on hover by current domain logic.
        // Assert.AreEqual(0, hoveredEntity.InteractionCount, "InteractionCount should be 0 for hover if it's a new entity.");


        // Act (Hover Exit)
        LogAssert.Expect(LogType.Log, $"Entity '{entity.Name}' HOVER EXIT by interactor '{_dummyInteractor.name}'. IsHovered: False");
        _interactionManager.HoverExit(_dummyInteractor, interactable);
        yield return null;

        // Assert (Hover Exit)
        var unhoveredEntity = _interactionService.GetInteractableById(entity.Id);
        Assert.IsNotNull(unhoveredEntity, "Unhovered entity should be retrievable.");
        Assert.IsFalse(unhoveredEntity.IsHovered, "Entity's IsHovered should be false after hover exit.");

        yield return null;
    }
}
