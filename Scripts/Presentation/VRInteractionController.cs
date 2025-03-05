using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using MetaQuestTest.Application;
using MetaQuestTest.Domain;
using MetaQuestTest.Infrastructure;

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

        [SerializeField]
        private XRRig _xrRig;
        
        // Sample interactable objects to create on start
        [SerializeField]
        private int _numberOfSampleObjects = 5;
        
        // Distance range to distribute sample objects
        [SerializeField]
        private float _distributionRadius = 2.0f;

        void Awake()
        {
            // Setup the repository and service
            _repository = gameObject.AddComponent<UnityVRInteractionRepository>();
            _interactionService = new VRInteractionService(_repository);
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
                // Create different types of interactable objects
                InteractionType type = (InteractionType)(i % 4); // Cycle through the enum values
                bool isGrabbable = (i % 2 == 0);
                bool isUsable = (i % 3 == 0);
                
                // Position objects in a circle around the player
                float angle = i * (360f / _numberOfSampleObjects);
                float x = Mathf.Sin(angle * Mathf.Deg2Rad) * _distributionRadius;
                float z = Mathf.Cos(angle * Mathf.Deg2Rad) * _distributionRadius;
                
                // Create the domain entity
                var entity = _interactionService.RegisterInteractable(
                    $"Interactable_{type}_{i}", 
                    type, 
                    isGrabbable, 
                    isUsable
                );
                
                // Update its position
                var position = new VRInteractionEntity.Position(x, 1.0f, z);
                _interactionService.UpdateInteractablePosition(entity.Id, position);
            }
        }
        
        private void SetupXRInteractionEvents()
        {
            // In a real implementation, we would hook into XR Interaction events here
            // For example:
            // var interactionManager = _xrRig.GetComponent<XRInteractionManager>();
            // interactionManager.interactionGroups.GetRegisteredInteractionGroups()...
            
            // For this simple test project, we'll just log that we're ready
            Debug.Log("VR Interaction Controller initialized and ready");
        }
        
        // Update is called once per frame
        void Update()
        {
            // Any per-frame updates would go here
            // This could include checking for controller inputs, 
            // updating positions of tracked objects, etc.
        }
    }
}