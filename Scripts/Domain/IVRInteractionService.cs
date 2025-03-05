using System.Collections.Generic;

namespace MetaQuestTest.Domain
{
    /// <summary>
    /// Domain service interface defining operations for VR interactions
    /// Following DDD principles by defining domain operations at a higher level
    /// </summary>
    public interface IVRInteractionService
    {
        /// <summary>
        /// Registers a new interactable entity in the VR environment
        /// </summary>
        VRInteractionEntity RegisterInteractable(string name, InteractionType type, bool isGrabbable, bool isUsable);
        
        /// <summary>
        /// Gets an interactable entity by its unique ID
        /// </summary>
        VRInteractionEntity GetInteractableById(string id);
        
        /// <summary>
        /// Gets all interactable entities of a specific type
        /// </summary>
        IEnumerable<VRInteractionEntity> GetInteractablesByType(InteractionType type);
        
        /// <summary>
        /// Updates the position of an interactable entity
        /// </summary>
        void UpdateInteractablePosition(string id, VRInteractionEntity.Position newPosition);
        
        /// <summary>
        /// Processes an interaction with an entity
        /// </summary>
        void ProcessInteraction(string interactableId, string interactorId);
    }
}