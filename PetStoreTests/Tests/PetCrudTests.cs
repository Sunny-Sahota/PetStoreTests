using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Helpers;
using PetStoreTests.Services;
using PetStoreTests.Utilities;
using Xunit.Abstractions;

namespace PetStoreTests.Tests
{
    public class PetCrudTests : ApiTestBase
    {
        private readonly PetService _petService;
        private readonly PetActions _petAction;

        public PetCrudTests(ITestOutputHelper output) : base(output)
        {
            _petService = new PetService(PetClient);
            _petAction = new PetActions(PetClient);
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
            TrackPet(pet.Id);
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
            _petAction.CreatePet(pet);
            TrackPet(pet.Id);

            // READ (with retry - still fine via helper)
            var fetchedPet = _petAction.GetPet(pet.Id);
            fetchedPet.Id.Should().Be(pet.Id);

            // UPDATE
            pet.Name = TestDataFactory.UpdatedPetName;
            ApiAssertions.ShouldBeOk(PetClient.PutPet(pet));

            // VERIFY UPDATE (via service layer)
            var updatedPet = _petService.WaitForPetNameToBe(pet.Id, TestDataFactory.UpdatedPetName);
            updatedPet.Name.Should().Be(TestDataFactory.UpdatedPetName);

            // DELETE
            ApiAssertions.ShouldBeDeletedOrNotFound(PetClient.DeletePet(pet.Id));

            // VERIFY DELETE
            ApiAssertions.ShouldBeOkOrNotFound(PetClient.GetPetById(pet.Id));
        }

        [Fact]
        [Trait("Category","CRUD")]
        public void CreateAndGetPet_FullSchema_ShouldBeRoundTrip()
        {
            //  ARRANGE
            var pet = TestDataFactory.CreateCompletePet();

            //  ACT
            var createdPet = _petAction.CreatePet(pet);
            TrackPet(pet.Id);
            var fetchedPet = _petAction.GetPet(pet.Id);

            //  ASSERT
            createdPet.Id.Should().Be(pet.Id);
            fetchedPet.Id.Should().Be(pet.Id);
            fetchedPet.Name.Should().Be(pet.Name);

            //  ASSERT
            fetchedPet.Category.Should().NotBeNull();
            fetchedPet.Category!.Id.Should().Be(1);
            fetchedPet.Category!.Name.Should().Be(TestDataFactory.DefaultCategoryName);

            //  ASSERT
            fetchedPet.PhotoUrls.Should().BeEquivalentTo(["http://example.com/pic1.jpg"]);

            //  ASSERT
            fetchedPet.Tags.Should().HaveCount(1);
            fetchedPet.Tags[0].Id.Should().Be(1);
            fetchedPet.Tags[0].Name.Should().Be(TestDataFactory.DefaultTagName);
        }
    }
}
