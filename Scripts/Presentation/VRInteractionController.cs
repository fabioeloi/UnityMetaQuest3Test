using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using MetaQuestTest.Application;
using MetaQuestTest.Domain;
using MetaQuestTest.Infrastructure;
using System.Linq; // Added for LINQ operations

namespace MetaQuestTest.Presentation
{
    /// <summary>
    /// Main controller for VR interactions in the Unity scene
    /// Acts as a mediator between Unity's XR system and our domain model
    /// </summary>
    public class VRInteractionController : MonoBehaviour
    {
        private IVRInteractionService _interactionService;
        private UnityVRInteractionRepository _repository;
        private XRInteractionManager _interactionManager;

        // It's better to use XROrigin or a similar class that contains the XRRig and XRInteractionManager
        // For simplicity, assuming _xrRig might be the XROrigin or has the manager as a component.
        [SerializeField]
        private XRBaseControllerInteractor _xrRig; // Changed to XRBaseControllerInteractor for interactor name
        
        // Sample interactable objects to create on start
        [SerializeField]
        private int _numberOfSampleObjects = 5;
        
        // Distance range to distribute sample objects
        [SerializeField]
        private float _distributionRadius = 2.0f;

        void Awake()
        {
            // Setup the repository and service
            _repository = FindObjectOfType<UnityVRInteractionRepository>();
            if (_repository == null)
            {
                _repository = gameObject.AddComponent<UnityVRInteractionRepository>();
            }
            _interactionService = new VRInteractionService(_repository);

            // Get the XRInteractionManager
            _interactionManager = FindObjectOfType<XRInteractionManager>();
            if (_interactionManager == null)
            {
                // Fallback if not found on XRRig, try to find it in the scene
                _interactionManager = FindObjectOfType<XRInteractionManager>();
                if (_interactionManager == null)
                {
                    Debug.LogError("XRInteractionManager not found in the scene.");
                    return;
                }
            }
        }
        
        void Start()
        {
            // Create some test objects in the scene
            CreateSampleInteractables();
            
            // Hook into XR Interaction events
            SetupXRInteractionEvents();
        }
        
        private void CreateSampleInteractables()
        {
            for (int i = 0; i < _numberOfSampleObjects; i++)
            {
                InteractionType type = (InteractionType)(i % System.Enum.GetValues(typeof(InteractionType)).Length);
                bool isGrabbable = (i % 2 == 0);
                bool isUsable = (i % 3 == 0);
                
                float angle = i * (360f / _numberOfSampleObjects);
                float x = Mathf.Sin(angle * Mathf.Deg2Rad) * _distributionRadius;
                float z = Mathf.Cos(angle * Mathf.Deg2Rad) * _distributionRadius;
                
                var entity = _interactionService.RegisterInteractable(
                    $"Interactable_{type}_{i}", 
                    type, 
                    isGrabbable, 
                    isUsable
                );
                
                var position = new VRInteractionEntity.Position(x, 1.0f, z);
                _interactionService.UpdateInteractablePosition(entity.Id, position);

                // Component addition logic is now handled by UnityVRInteractionRepository.Add()
                // No need to add XRGrabInteractable or BoxCollider here.
            }
        }
        
        private void SetupXRInteractionEvents()
        {
            if (_interactionManager == null)
            {
                Debug.LogError("XRInteractionManager is not set. Cannot setup XR events.");
                return;
            }

            if (_repository == null || _repository._gameObjectMap == null)
            {
                 Debug.LogError("Repository or GameObjectMap is not initialized.");
                 return;
            }

            // Iterate through GameObjects managed by the repository
            foreach (var go in _repository._gameObjectMap.Values.ToList()) // ToList() to avoid modification issues if any
            {
                if (go == null) continue;

                var interactable = go.GetComponent<XRBaseInteractable>();
                if (interactable != null)
                {
                    interactable.selectEntered.AddListener(HandleSelectEntered);
                    interactable.activated.AddListener(HandleActivated);
                }
            }
            Debug.Log("VR Interaction Controller initialized and XR events setup.");
        }

        private void HandleSelectEntered(SelectEnterEventArgs args)
        {
            var interactableObject = args.interactableObject as IXRSelectInteractable;
            if (interactableObject != null && interactableObject.transform != null)
            {
                string entityId = _repository.GetEntityIdByGameObject(interactableObject.transform.gameObject);
                if (!string.IsNullOrEmpty(entityId))
                {
                    string interactorName = args.interactorObject.transform.name; // Get interactor name
                    _interactionService.ProcessInteraction(entityId, interactorName, InteractionEvent.Selected);
                    Debug.Log($"Select Entered: Entity {entityId} by {interactorName}");
                }
            }
        }

        private void HandleActivated(ActivateEventArgs args)
        {
            var interactableObject = args.interactableObject as IXRActivateInteractable;
            if (interactableObject != null && interactableObject.transform != null)
            {
                string entityId = _repository.GetEntityIdByGameObject(interactableObject.transform.gameObject);
                if (!string.IsNullOrEmpty(entityId))
                {
                    string interactorName = args.interactorObject.transform.name; // Get interactor name
                    _interactionService.ProcessInteraction(entityId, interactorName, InteractionEvent.Activated);
                    Debug.Log($"Activated: Entity {entityId} by {interactorName}");
                }
            }
        }
        
        void Update()
        {
            // Per-frame updates can go here
        }

        void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (_repository != null && _repository._gameObjectMap != null)
            {
                foreach (var go in _repository._gameObjectMap.Values.ToList())
                {
                    if (go == null) continue;
                    var interactable = go.GetComponent<XRBaseInteractable>();
                    if (interactable != null)
                    {
                        interactable.selectEntered.RemoveListener(HandleSelectEntered);
                        interactable.activated.RemoveListener(HandleActivated);
                    }
                }
            }
        }
    }
}