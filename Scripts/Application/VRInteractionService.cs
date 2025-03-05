using System.Collections.Generic;
using MetaQuestTest.Domain;

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

        public void ProcessInteraction(string interactableId, string interactorId)
        {
            var entity = _repository.GetById(interactableId);
            if (entity != null)
            {
                entity.Interact();
                _repository.Update(entity);
            }
        }
    }
}