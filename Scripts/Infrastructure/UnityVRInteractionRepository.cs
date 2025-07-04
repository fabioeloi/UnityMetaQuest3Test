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
        // Public fields for color configuration, can be set in Inspector or use defaults.
        public Color originalColor = Color.white;
        public Color hoverColor = Color.yellow;

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

                // Add a MeshFilter and MeshRenderer to make the object visible and colorable.
                // Use a default cube mesh for simplicity.
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = GetPrimitiveMesh(PrimitiveType.Cube); // Simple cube mesh

                MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

                // Apply a default material that can be colored.
                // Using a standard shader material.
                if (meshRenderer.sharedMaterial == null)
                {
                    // Create a new material instance if one doesn't exist to avoid modifying shared assets.
                    // However, for simplicity in this context, directly setting color on a new default material.
                    // In a real project, use pre-made materials or a more robust material management.
                    var material = new Material(Shader.Find("Standard")); // Or "Legacy Shaders/Diffuse" for simpler unlit color
                    material.color = originalColor;
                    meshRenderer.material = material;
                }
                else
                {
                    // If a material already exists (e.g. from a prefab), make sure we have an instance of it
                    // to avoid changing the shared material asset. Then set its color.
                    // For newly created GameObjects as here, the above block is more likely.
                    meshRenderer.material.color = originalColor;
                }

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

                    // Update visual feedback based on hover state
                    Renderer renderer = gameObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        // Ensure the material is an instance, not a shared asset, before changing color.
                        // This is important if objects might share materials initially.
                        // If Add() always creates a new material instance, this might not be strictly needed here
                        // but is good practice.
                        if (renderer.material == null) { // Should not happen if Add() sets it up
                             var material = new Material(Shader.Find("Standard"));
                             renderer.material = material;
                        }

                        renderer.material.color = entity.IsHovered ? hoverColor : originalColor;
                    }
                }
            }
        }
        
        // Helper method to get a primitive mesh (e.g., Cube)
        private static Mesh GetPrimitiveMesh(PrimitiveType primitiveType)
        {
            GameObject tempObject = GameObject.CreatePrimitive(primitiveType);
            Mesh mesh = tempObject.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tempObject);
            return mesh;
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