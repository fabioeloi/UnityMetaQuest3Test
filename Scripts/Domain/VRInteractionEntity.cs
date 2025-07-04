using System;
using UnityEngine;

namespace MetaQuestTest.Domain
{
    /// <summary>
    /// Core domain entity representing a VR interactable object
    /// This follows DDD principles by encapsulating domain logic related to VR interactions
    /// </summary>
    public class VRInteractionEntity
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public InteractionType Type { get; private set; }
        public bool IsGrabbable { get; private set; }
        public bool IsUsable { get; private set; }
        public bool WasInteracted { get; private set; } = false;
        public int InteractionCount { get; private set; } = 0;
        public bool IsGrabbed { get; private set; } = false; // Property for grab state
        public bool IsHovered { get; private set; } = false; // New property for hover state
        
        // Value object for position in 3D space
        public class Position
        {
            public float X { get; }
            public float Y { get; }
            public float Z { get; }
            
            public Position(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            
            public Vector3 ToVector3()
            {
                return new Vector3(X, Y, Z);
            }
            
            public static Position FromVector3(Vector3 vector)
            {
                return new Position(vector.x, vector.y, vector.z);
            }
        }
        
        public Position CurrentPosition { get; private set; }
        
        private VRInteractionEntity() { }
        
        public static VRInteractionEntity Create(string name, InteractionType type, bool isGrabbable, bool isUsable)
        {
            return new VRInteractionEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Type = type,
                IsGrabbable = isGrabbable,
                IsUsable = isUsable,
                CurrentPosition = new Position(0, 0, 0)
            };
        }
        
        public void UpdatePosition(Position newPosition)
        {
            CurrentPosition = newPosition;
        }
        
        // Removed the non-obsolete duplicate Interact() method. Only the obsolete one remains.

        [System.Obsolete("Use Grab() or Use() instead.")]
        public void Interact()
        {
            // Domain logic for basic interaction
            WasInteracted = true;
            InteractionCount++;
            Debug.Log($"Entity '{Name}' interacted with (old method). Total interactions: {InteractionCount}. Grabbable: {IsGrabbable}, Usable: {IsUsable}");
        }

        /// <summary>
        /// Handles the domain logic for a grab interaction.
        /// </summary>
        /// <param name="interactorId">Identifier for the interactor that performed the grab.</param>
        public void Grab(string interactorId)
        {
            WasInteracted = true;
            InteractionCount++;
            // TODO: Consider if IsGrabbable flag check is needed here or if service layer handles it sufficiently.
            // For now, directly setting state as per interaction.
            IsGrabbed = true;
            Debug.Log($"Entity '{Name}' GRABBED by interactor '{interactorId}'. Total interactions: {InteractionCount}. IsGrabbed: {IsGrabbed}");
        }

        /// <summary>
        /// Handles the domain logic for a use interaction.
        /// </summary>
        /// <param name="interactorId">Identifier for the interactor that performed the use action.</param>
        public void Use(string interactorId)
        {
            WasInteracted = true;
            InteractionCount++;
            // TODO: Consider if IsUsable flag check is needed here.
            Debug.Log($"Entity '{Name}' USED by interactor '{interactorId}'. Total interactions: {InteractionCount}.");
        }

        /// <summary>
        /// Handles the domain logic for a release interaction.
        /// </summary>
        /// <param name="interactorId">Identifier for the interactor that performed the release.</param>
        public void Release(string interactorId)
        {
            // TODO: Consider if !IsGrabbed check is needed here or if service layer handles it.
            IsGrabbed = false;
            Debug.Log($"Entity '{Name}' RELEASED by interactor '{interactorId}'. IsGrabbed: {IsGrabbed}");
        }

        /// <summary>
        /// Handles the domain logic for a hover enter event.
        /// </summary>
        /// <param name="interactorId">Identifier for the interactor that started hovering.</param>
        public void HoverEnter(string interactorId)
        {
            IsHovered = true;
            Debug.Log($"Entity '{Name}' HOVER ENTER by interactor '{interactorId}'. IsHovered: {IsHovered}");
        }

        /// <summary>
        /// Handles the domain logic for a hover exit event.
        /// </summary>
        /// <param name="interactorId">Identifier for the interactor that stopped hovering.</param>
        public void HoverExit(string interactorId)
        {
            IsHovered = false;
            Debug.Log($"Entity '{Name}' HOVER EXIT by interactor '{interactorId}'. IsHovered: {IsHovered}");
        }
        
        public bool CanBeGrabbed()
        {
            return IsGrabbable;
        }
        
        public bool CanBeUsed()
        {
            return IsUsable;
        }
    }
    
    public enum InteractionType
    {
        Grab,
        Touch,
        Point,
        Voice
    }
}