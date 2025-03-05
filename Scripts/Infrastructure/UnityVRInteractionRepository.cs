using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MetaQuestTest.Domain;

namespace MetaQuestTest.Infrastructure
{
    /// <summary>
    /// Unity-specific implementation of the IVRInteractionRepository
    /// This class bridges our domain model with Unity's GameObject system
    /// </summary>
    public class UnityVRInteractionRepository : MonoBehaviour, IVRInteractionRepository
    {
        private Dictionary<string, VRInteractionEntity> _interactionEntities = new Dictionary<string, VRInteractionEntity>();
        private Dictionary<string, GameObject> _gameObjectMap = new Dictionary<string, GameObject>();
        
        public void Add(VRInteractionEntity entity)
        {
            if (!_interactionEntities.ContainsKey(entity.Id))
            {
                _interactionEntities[entity.Id] = entity;
                
                // Create or link a Unity GameObject for this entity
                GameObject gameObject = new GameObject(entity.Name);
                gameObject.transform.position = entity.CurrentPosition.ToVector3();
                
                // Add appropriate Unity components based on entity properties
                if (entity.CanBeGrabbed())
                {
                    // In a real implementation, we would add XR grab interactable components
                    // For this simple test project, we'll just add a placeholder component
                    gameObject.AddComponent<BoxCollider>();
                    gameObject.AddComponent<Rigidbody>();
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
                    Destroy(gameObject);
                    _gameObjectMap.Remove(id);
                }
            }
        }
        
        public IEnumerable<VRInteractionEntity> GetAll()
        {
            return _interactionEntities.Values;
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