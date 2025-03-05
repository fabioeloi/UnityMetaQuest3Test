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
        
        public void Interact()
        {
            // Domain logic for basic interaction
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