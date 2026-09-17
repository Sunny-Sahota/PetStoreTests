using System.ComponentModel;
using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Helpers;
using PetStoreTests.Services;
using PetStoreTests.Utilities;

namespace PetStoreTests.Tests
{
    public class PetCrudTests
    {
        private readonly PetClient _petClient = new();
        private readonly PetService _petService;
        private readonly PetActions _petAction;

        public PetCrudTests()
        {
            _petService = new PetService(_petClient);
            _petAction = new PetActions(_petClient);
        }

        [Fact]
        [Trait("Category","CRUD")]
        public void CreatePet_And_GetPet_ValidResponse()
        {
            // ARRANGE
            // Creates pet with id, name, status
            var pet = TestDataFactory.CreatePet();

            // ACT
            var createdPet = _petAction.CreatePet(pet);
            var fetchedPet = _petAction.GetPet(pet.Id);
             
            // ASSERT
            createdPet.Id.Should().Be(pet.Id);

            fetchedPet.Id.Should().Be(pet.Id);
            fetchedPet.Name.Should().Be(pet.Name);
        }

        [Fact]
        [Trait("Category","CRUD")]
        public void FullCrud_Pet_ShouldWork()
        {
            // ARRANGE
            var pet = TestDataFactory.CreatePet();

            // CREATE
            var createdPet = _petAction.CreatePet(pet);

            // READ (with retry - still fine via helper)
            var fetchedPet = _petAction.GetPet(pet.Id);
            fetchedPet.Id.Should().Be(pet.Id);

            // UPDATE
            pet.Name = "UpdatedPet";
            ApiAssertions.ShouldBeOk(_petClient.PutPet(pet));

            // VERIFY UPDATE (via service layer)
            var updatedPet = _petService.WaitForPetNameToBe(pet.Id, "UpdatedPet");
            updatedPet.Name.Should().Be("UpdatedPet");

            // DELETE
            ApiAssertions.ShouldBeDeletedOrNotFound(_petClient.DeletePet(pet.Id));

            // VERIFY DELETE
            ApiAssertions.ShouldBeOkOrNotFound(_petClient.GetPetById(pet.Id));
        }

        [Fact]
        [Trait("Category","CRUD")]
        public void CreateAndGetPet_FullSchema_ShouldBeRoundTrip()
        {
            //  ARRANGE
            var pet = TestDataFactory.CreateCompletePet();

            //  ACT
            var createdPet = _petAction.CreatePet(pet);
            var fetchedPet = _petAction.GetPet(pet.Id);

            //  ASSERT
            createdPet.Id.Should().Be(pet.Id);
            fetchedPet.Id.Should().Be(pet.Id);
            fetchedPet.Name.Should().Be(pet.Name);

            //  ASSERT
            fetchedPet.Category.Should().NotBeNull();
            fetchedPet.Category!.Id.Should().Be(1);
            fetchedPet.Category!.Name.Should().Be("Dogs");

            //  ASSERT
            fetchedPet.PhotoUrls.Should().BeEquivalentTo(["http://example.com/pic1.jpg"]);

            //  ASSERT
            fetchedPet.Tags.Should().HaveCount(1);
            fetchedPet.Tags[0].Id.Should().Be(1);
            fetchedPet.Tags[0].Name.Should().Be("friendly");

            //  CLEANUP
            _petClient.DeletePet(pet.Id);
        }
    }
}
