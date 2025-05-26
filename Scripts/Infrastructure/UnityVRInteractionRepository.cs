using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Required for XR components
using MetaQuestTest.Domain;
using VRApplication.Infrastructure; // Added for EntityIdentifier

namespace MetaQuestTest.Infrastructure
{
    /// <summary>
    /// Unity-specific implementation of the IVRInteractionRepository
    /// This class bridges our domain model with Unity's GameObject system
    /// </summary>
    public class UnityVRInteractionRepository : MonoBehaviour, IVRInteractionRepository
    {
        private Dictionary<string, VRInteractionEntity> _interactionEntities = new Dictionary<string, VRInteractionEntity>();
        public Dictionary<string, GameObject> _gameObjectMap = new Dictionary<string, GameObject>(); // Made public for VRInteractionController
        
        public void Add(VRInteractionEntity entity)
        {
            if (!_interactionEntities.ContainsKey(entity.Id))
            {
                _interactionEntities[entity.Id] = entity;
                
                // Create or link a Unity GameObject for this entity
                GameObject gameObject = new GameObject(entity.Name);
                gameObject.transform.position = entity.CurrentPosition.ToVector3();
                
                // Add EntityIdentifier component
                var entityIdentifier = gameObject.AddComponent<EntityIdentifier>();
                entityIdentifier.EntityId = entity.Id;
                
                // Always add a collider
                gameObject.AddComponent<BoxCollider>();
                
                // Add appropriate XR components based on entity properties
                if (entity.IsGrabbable) // Using direct property access
                {
                    gameObject.AddComponent<XRGrabInteractable>();
                    var rb = gameObject.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = gameObject.AddComponent<Rigidbody>();
                    }
                    rb.useGravity = false;
                    rb.isKinematic = true; 
                }
                else if (entity.IsUsable) // Using direct property access
                {
                    gameObject.AddComponent<XRSimpleInteractable>();
                }
                
                _gameObjectMap[entity.Id] = gameObject;
            }
        }
        
        public VRInteractionEntity GetById(string id)
        {
            return _interactionEntities.TryGetValue(id, out var entity) ? entity : null;
        }
        
        public IEnumerable<VRInteractionEntity> FindByType(InteractionType type)
        {
            return _interactionEntities.Values.Where(e => e.Type == type);
        }
        
        public void Update(VRInteractionEntity entity)
        {
            if (_interactionEntities.ContainsKey(entity.Id))
            {
                _interactionEntities[entity.Id] = entity;
                
                // Update the corresponding Unity GameObject
                if (_gameObjectMap.TryGetValue(entity.Id, out var gameObject))
                {
                    gameObject.transform.position = entity.CurrentPosition.ToVector3();
                }
            }
        }
        
        public void Remove(string id)
        {
            if (_interactionEntities.ContainsKey(id))
            {
                _interactionEntities.Remove(id);
                
                // Destroy the corresponding Unity GameObject
                if (_gameObjectMap.TryGetValue(id, out var gameObject))
                {
                    // Remove EntityIdentifier if it exists (though Destroy will handle it)
                    var entityIdentifier = gameObject.GetComponent<EntityIdentifier>();
                    if (entityIdentifier != null)
                    {
                        Destroy(entityIdentifier);
                    }
                    Destroy(gameObject);
                    _gameObjectMap.Remove(id);
                }
            }
        }
        
        public IEnumerable<VRInteractionEntity> GetAll()
        {
            return _interactionEntities.Values;
        }

        public string GetEntityIdByGameObject(GameObject go)
        {
            if (go != null)
            {
                var entityIdentifier = go.GetComponent<EntityIdentifier>();
                if (entityIdentifier != null)
                {
                    return entityIdentifier.EntityId;
                }
            }
            return null;
        }
        
        // Unity lifecycle methods
        private void OnDestroy()
        {
            foreach (var gameObject in _gameObjectMap.Values)
            {
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
            
            _interactionEntities.Clear();
            _gameObjectMap.Clear();
        }
    }
}