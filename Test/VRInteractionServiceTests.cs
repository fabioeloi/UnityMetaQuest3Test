using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MetaQuestTest.Domain;
using MetaQuestTest.Application;

namespace MetaQuestTest.Tests
{
    public class VRInteractionServiceTests
    {
        private IVRInteractionRepository _mockRepository;
        private IVRInteractionService _service;
        private Dictionary<string, VRInteractionEntity> _testEntities;
        
        [SetUp]
        public void SetUp()
        {
            // Setup a mock repository for testing
            _testEntities = new Dictionary<string, VRInteractionEntity>();
            _mockRepository = new MockVRInteractionRepository(_testEntities);
            _service = new VRInteractionService(_mockRepository);
        }
        
        [Test]
        public void RegisterInteractable_CreatesNewEntity()
        {
            // Arrange
            string name = "TestObject";
            InteractionType type = InteractionType.Grab;
            
            // Act
            var entity = _service.RegisterInteractable(name, type, true, false);
            
            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(type, entity.Type);
            Assert.IsTrue(entity.CanBeGrabbed());
            Assert.IsFalse(entity.CanBeUsed());
            Assert.IsTrue(_testEntities.ContainsKey(entity.Id));
        }
        
        [Test]
        public void GetInteractableById_ReturnsCorrectEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("TestObject", InteractionType.Grab, true, false);
            
            // Act
            var retrievedEntity = _service.GetInteractableById(entity.Id);
            
            // Assert
            Assert.IsNotNull(retrievedEntity);
            Assert.AreEqual(entity.Id, retrievedEntity.Id);
            Assert.AreEqual(entity.Name, retrievedEntity.Name);
        }
        
        [Test]
        public void GetInteractablesByType_ReturnsOnlyMatchingType()
        {
            // Arrange
            _service.RegisterInteractable("GrabObject", InteractionType.Grab, true, false);
            _service.RegisterInteractable("TouchObject", InteractionType.Touch, false, true);
            _service.RegisterInteractable("PointObject", InteractionType.Point, false, false);
            _service.RegisterInteractable("AnotherGrabObject", InteractionType.Grab, true, true);
            
            // Act
            var grabObjects = _service.GetInteractablesByType(InteractionType.Grab).ToList();
            
            // Assert
            Assert.AreEqual(2, grabObjects.Count);
            Assert.IsTrue(grabObjects.All(e => e.Type == InteractionType.Grab));
        }
        
        [Test]
        public void UpdateInteractablePosition_ChangesPosition()
        {
            // Arrange
            var entity = _service.RegisterInteractable("TestObject", InteractionType.Grab, true, false);
            var newPosition = new VRInteractionEntity.Position(1.0f, 2.0f, 3.0f);
            
            // Act
            _service.UpdateInteractablePosition(entity.Id, newPosition);
            var updatedEntity = _service.GetInteractableById(entity.Id);
            
            // Assert
            Assert.AreEqual(1.0f, updatedEntity.CurrentPosition.X);
            Assert.AreEqual(2.0f, updatedEntity.CurrentPosition.Y);
            Assert.AreEqual(3.0f, updatedEntity.CurrentPosition.Z);
        }
        
        // Helper mock class for testing
        private class MockVRInteractionRepository : IVRInteractionRepository
        {
            private readonly Dictionary<string, VRInteractionEntity> _entities;
            
            public MockVRInteractionRepository(Dictionary<string, VRInteractionEntity> entities)
            {
                _entities = entities;
            }
            
            public void Add(VRInteractionEntity entity)
            {
                _entities[entity.Id] = entity;
            }
            
            public VRInteractionEntity GetById(string id)
            {
                return _entities.TryGetValue(id, out var entity) ? entity : null;
            }
            
            public IEnumerable<VRInteractionEntity> FindByType(InteractionType type)
            {
                return _entities.Values.Where(e => e.Type == type);
            }
            
            public void Update(VRInteractionEntity entity)
            {
                _entities[entity.Id] = entity;
            }
            
            public void Remove(string id)
            {
                _entities.Remove(id);
            }
            
            public IEnumerable<VRInteractionEntity> GetAll()
            {
                return _entities.Values;
            }
        }
    }
}