using System.Collections.Generic;

namespace MetaQuestTest.Domain
{
    /// <summary>
    /// Repository interface for VR interaction entities
    /// Following DDD principles of persistence ignorance
    /// </summary>
    public interface IVRInteractionRepository
    {
        /// <summary>
        /// Adds a new VR interaction entity to the repository
        /// </summary>
        void Add(VRInteractionEntity entity);
        
        /// <summary>
        /// Gets a VR interaction entity by its id
        /// </summary>
        VRInteractionEntity GetById(string id);
        
        /// <summary>
        /// Gets all VR interaction entities matching the specified type
        /// </summary>
        IEnumerable<VRInteractionEntity> FindByType(InteractionType type);
        
        /// <summary>
        /// Updates an existing VR interaction entity
        /// </summary>
        void Update(VRInteractionEntity entity);
        
        /// <summary>
        /// Removes a VR interaction entity from the repository
        /// </summary>
        void Remove(string id);
        
        /// <summary>
        /// Gets all VR interaction entities
        /// </summary>
        IEnumerable<VRInteractionEntity> GetAll();
    }
}