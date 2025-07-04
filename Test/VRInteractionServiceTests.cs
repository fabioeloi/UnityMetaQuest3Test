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

        // --- Tests for ProcessGrabInteraction ---
        [Test]
        public void ProcessGrabInteraction_WhenEntityExistsAndIsGrabbable_GrabsEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("GrabbableObject", InteractionType.Grab, true, false);
            string interactorId = "TestInteractor";

            // Act
            _service.ProcessGrabInteraction(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsTrue(updatedEntity.IsGrabbed);
            Assert.IsTrue(updatedEntity.WasInteracted);
            Assert.AreEqual(1, updatedEntity.InteractionCount);
        }

        [Test]
        public void ProcessGrabInteraction_WhenEntityExistsAndIsNotGrabbable_DoesNotGrabEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("NonGrabbableObject", InteractionType.Touch, false, true);
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {entity.Id} is not grabbable. Grab interaction aborted.");

            // Act
            _service.ProcessGrabInteraction(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsFalse(updatedEntity.IsGrabbed);
            Assert.IsFalse(updatedEntity.WasInteracted); // Should not count as an interaction if aborted
            Assert.AreEqual(0, updatedEntity.InteractionCount);
        }

        [Test]
        public void ProcessGrabInteraction_WhenEntityDoesNotExist_LogsWarning()
        {
            // Arrange
            string nonExistentId = "NonExistentId";
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {nonExistentId} not found. Cannot process grab interaction.");

            // Act
            _service.ProcessGrabInteraction(nonExistentId, interactorId);

            // Assert (Log is checked by LogAssert)
        }

        // --- Tests for ProcessReleaseInteraction ---
        [Test]
        public void ProcessReleaseInteraction_WhenEntityExistsAndIsGrabbed_ReleasesEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("GrabbableObject", InteractionType.Grab, true, false);
            string interactorId = "TestInteractor";
            _service.ProcessGrabInteraction(entity.Id, interactorId); // Grab it first
            var grabbedEntity = _service.GetInteractableById(entity.Id);
            Assert.IsTrue(grabbedEntity.IsGrabbed, "Entity should be grabbed before testing release.");

            // Act
            _service.ProcessReleaseInteraction(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsFalse(updatedEntity.IsGrabbed);
        }

        [Test]
        public void ProcessReleaseInteraction_WhenEntityDoesNotExist_LogsWarning()
        {
            // Arrange
            string nonExistentId = "NonExistentId";
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {nonExistentId} not found. Cannot process release interaction.");

            // Act
            _service.ProcessReleaseInteraction(nonExistentId, interactorId);

            // Assert (Log is checked by LogAssert)
        }

        // --- Tests for ProcessUseInteraction ---
        [Test]
        public void ProcessUseInteraction_WhenEntityExistsAndIsUsable_UsesEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("UsableObject", InteractionType.Touch, false, true);
            string interactorId = "TestInteractor";

            // Act
            _service.ProcessUseInteraction(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsTrue(updatedEntity.WasInteracted);
            Assert.AreEqual(1, updatedEntity.InteractionCount);
        }

        [Test]
        public void ProcessUseInteraction_WhenEntityExistsAndIsNotUsable_DoesNotUseEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("NonUsableObject", InteractionType.Grab, true, false);
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {entity.Id} is not usable. Use interaction aborted.");

            // Act
            _service.ProcessUseInteraction(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsFalse(updatedEntity.WasInteracted);
            Assert.AreEqual(0, updatedEntity.InteractionCount);
        }

        [Test]
        public void ProcessUseInteraction_WhenEntityDoesNotExist_LogsWarning()
        {
            // Arrange
            string nonExistentId = "NonExistentId";
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {nonExistentId} not found. Cannot process use interaction.");

            // Act
            _service.ProcessUseInteraction(nonExistentId, interactorId);

            // Assert (Log is checked by LogAssert)
        }

        // --- Tests for ProcessHoverEnter ---
        [Test]
        public void ProcessHoverEnter_WhenEntityExists_HoversEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("HoverableObject", InteractionType.Point, false, false);
            string interactorId = "TestInteractor";

            // Act
            _service.ProcessHoverEnter(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsTrue(updatedEntity.IsHovered);
        }

        [Test]
        public void ProcessHoverEnter_WhenEntityDoesNotExist_LogsWarning()
        {
            // Arrange
            string nonExistentId = "NonExistentId";
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {nonExistentId} not found. Cannot process hover enter.");

            // Act
            _service.ProcessHoverEnter(nonExistentId, interactorId);

            // Assert (Log is checked by LogAssert)
        }

        // --- Tests for ProcessHoverExit ---
        [Test]
        public void ProcessHoverExit_WhenEntityExists_UnhoversEntity()
        {
            // Arrange
            var entity = _service.RegisterInteractable("HoverableObject", InteractionType.Point, false, false);
            string interactorId = "TestInteractor";
            _service.ProcessHoverEnter(entity.Id, interactorId); // Hover it first
            var hoveredEntity = _service.GetInteractableById(entity.Id);
            Assert.IsTrue(hoveredEntity.IsHovered, "Entity should be hovered before testing hover exit.");

            // Act
            _service.ProcessHoverExit(entity.Id, interactorId);
            var updatedEntity = _service.GetInteractableById(entity.Id);

            // Assert
            Assert.IsFalse(updatedEntity.IsHovered);
        }

        [Test]
        public void ProcessHoverExit_WhenEntityDoesNotExist_LogsWarning()
        {
            // Arrange
            string nonExistentId = "NonExistentId";
            string interactorId = "TestInteractor";
            LogAssert.Expect(LogType.Warning, $"Entity with ID {nonExistentId} not found. Cannot process hover exit.");

            // Act
            _service.ProcessHoverExit(nonExistentId, interactorId);

            // Assert (Log is checked by LogAssert)
        }

        // --- Edge Case Tests ---
        [Test]
        public void GetInteractableById_WhenIdNotFound_ReturnsNull()
        {
            // Arrange
            string nonExistentId = "NonExistentId";

            // Act
            var result = _service.GetInteractableById(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateInteractablePosition_WhenIdNotFound_DoesNotThrow()
        {
            // Arrange
            string nonExistentId = "NonExistentId";
            var newPosition = new VRInteractionEntity.Position(10f, 20f, 30f);

            // Act & Assert
            Assert.DoesNotThrow(() => _service.UpdateInteractablePosition(nonExistentId, newPosition));
            // Optionally, check that no entity was added or modified in the mock repo if it had such tracking.
            // For now, ensuring no exception is sufficient for this case.
        }
    }
}