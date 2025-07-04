using System.Collections.Generic;
using MetaQuestTest.Domain;
using UnityEngine; // Required for Debug.LogWarning

namespace MetaQuestTest.Application
{
    /// <summary>
    /// Implementation of the IVRInteractionService domain service
    /// Part of the application layer in our DDD architecture
    /// </summary>
    public class VRInteractionService : IVRInteractionService
    {
        private readonly IVRInteractionRepository _repository;

        // Constructor injection for dependencies
        public VRInteractionService(IVRInteractionRepository repository)
        {
            _repository = repository;
        }

        public VRInteractionEntity RegisterInteractable(string name, InteractionType type, bool isGrabbable, bool isUsable)
        {
            var entity = VRInteractionEntity.Create(name, type, isGrabbable, isUsable);
            _repository.Add(entity);
            return entity;
        }

        public VRInteractionEntity GetInteractableById(string id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<VRInteractionEntity> GetInteractablesByType(InteractionType type)
        {
            return _repository.FindByType(type);
        }

        public void UpdateInteractablePosition(string id, VRInteractionEntity.Position newPosition)
        {
            var entity = _repository.GetById(id);
            if (entity != null)
            {
                entity.UpdatePosition(newPosition);
                _repository.Update(entity);
            }
        }

        [System.Obsolete("Use ProcessGrabInteraction() or ProcessUseInteraction() instead.")]
        public void ProcessInteraction(string interactableId, string interactorId)
        {
            Debug.LogWarning($"ProcessInteraction (obsolete) called for entity {interactableId} by {interactorId}. Consider updating to specific interaction methods.");
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                // Calling the obsolete Interact() method on the entity as per old behavior
                #pragma warning disable CS0618 // Type or member is obsolete
                entity.Interact();
                #pragma warning restore CS0618 // Type or member is obsolete
                _repository.Update(entity);
            }
            else
            {
                Debug.LogWarning($"Entity with ID {interactableId} not found. Obsolete ProcessInteraction call failed.");
            }
        }

        public void ProcessGrabInteraction(string interactableId, string interactorId)
        {
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                if (entity.IsGrabbable)
                {
                    entity.Grab(interactorId);
                    _repository.Update(entity); // Persist changes
                }
                else
                {
                    Debug.LogWarning($"Entity with ID {interactableId} is not grabbable. Grab interaction aborted.");
                }
            }
            else
            {
                Debug.LogWarning($"Entity with ID {interactableId} not found. Cannot process grab interaction.");
            }
        }

        public void ProcessUseInteraction(string interactableId, string interactorId)
        {
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                if (entity.IsUsable)
                {
                    entity.Use(interactorId);
                    _repository.Update(entity); // Persist changes
                }
                else
                {
                    Debug.LogWarning($"Entity with ID {interactableId} is not usable. Use interaction aborted.");
                }
            }
            else
            {
                Debug.LogWarning($"Entity with ID {interactableId} not found. Cannot process use interaction.");
            }
        }

        public void ProcessReleaseInteraction(string interactableId, string interactorId)
        {
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                // Call Release on the entity. The entity itself handles the IsGrabbed state.
                // We could add a check here: if (entity.IsGrabbed), but it's arguably the entity's job
                // to correctly handle a Release call regardless of its current IsGrabbed state.
                // For now, we align with how Grab/Use are handled (entity method called if entity exists).
                entity.Release(interactorId);
                _repository.Update(entity); // Persist changes to IsGrabbed state
            }
            else
            {
                Debug.LogWarning($"Entity with ID {interactableId} not found. Cannot process release interaction.");
            }
        }

        public void ProcessHoverEnter(string interactableId, string interactorId)
        {
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                entity.HoverEnter(interactorId);
                _repository.Update(entity); // Persist IsHovered state and trigger visual update
            }
            else
            {
                Debug.LogWarning($"Entity with ID {interactableId} not found. Cannot process hover enter.");
            }
        }

        public void ProcessHoverExit(string interactableId, string interactorId)
        {
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                entity.HoverExit(interactorId);
                _repository.Update(entity); // Persist IsHovered state and trigger visual update
            }
            else
            {
                Debug.LogWarning($"Entity with ID {interactableId} not found. Cannot process hover exit.");
            }
        }
    }
}