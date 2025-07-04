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
        [System.Obsolete("Use ProcessGrabInteraction() or ProcessUseInteraction() instead.")]
        void ProcessInteraction(string interactableId, string interactorId);

        /// <summary>
        /// Processes a grab interaction with a specific entity.
        /// </summary>
        /// <param name="interactableId">The ID of the interactable entity being grabbed.</param>
        /// <param name="interactorId">The ID of the interactor performing the grab.</param>
        void ProcessGrabInteraction(string interactableId, string interactorId);

        /// <summary>
        /// Processes a use interaction with a specific entity.
        /// </summary>
        /// <param name="interactableId">The ID of the interactable entity being used.</param>
        /// <param name="interactorId">The ID of the interactor performing the use action.</param>
        void ProcessUseInteraction(string interactableId, string interactorId);

        /// <summary>
        /// Processes a release interaction with a specific entity.
        /// </summary>
        /// <param name="interactableId">The ID of the interactable entity being released.</param>
        /// <param name="interactorId">The ID of the interactor performing the release.</param>
        void ProcessReleaseInteraction(string interactableId, string interactorId);

        /// <summary>
        /// Processes a hover enter event for a specific entity.
        /// </summary>
        /// <param name="interactableId">The ID of the interactable entity being hovered over.</param>
        /// <param name="interactorId">The ID of the interactor performing the hover.</param>
        void ProcessHoverEnter(string interactableId, string interactorId);

        /// <summary>
        /// Processes a hover exit event for a specific entity.
        /// </summary>
        /// <param name="interactableId">The ID of the interactable entity that was being hovered over.</param>
        /// <param name="interactorId">The ID of the interactor that stopped hovering.</param>
        void ProcessHoverExit(string interactableId, string interactorId);
    }
}