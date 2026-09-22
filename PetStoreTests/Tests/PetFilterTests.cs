using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Helpers;
using PetStoreTests.Models;
using PetStoreTests.Services;
using PetStoreTests.Utilities;

namespace PetStoreTests.Tests
{
    public class PetFilterTests : ApiTestBase
    {
        private readonly PetActions _petAction;
        private readonly PetService _petService;

        public PetFilterTests()
        {
            _petAction = new PetActions(PetClient);
            _petService = new PetService(PetClient);
        }

        [Fact]
        [Trait("Category","Filter")]
        public void FindByStatus_WithValidStatus_ShouldReturnPets()
        {
            //  ARRANGE
            var statuses = new[] { PetStatus.Available, PetStatus.Pending, PetStatus.Sold };

            foreach(var status in statuses)
            {
                var pet = TestDataFactory.CreatePet(status);
                _petAction.CreatePet(pet);
                TrackPet(pet.Id);

                //  ACT - wait for the created pet to appear in the search results
                var pets = _petService.WaitForPetInStatusSearch(pet.Id, status);

                //  ASSERT - self-seeded so we don't depend on the shared API's data
                pets.Should().Contain(p => p.Id == pet.Id);

                //  Every Returned pet should match the requested status
                pets.Should().OnlyContain(p => p.Status == status);
            }
        }
    }
}