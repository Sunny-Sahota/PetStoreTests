using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Helpers;
using PetStoreTests.Models;
using PetStoreTests.Utilities;
using System.Net;

namespace PetStoreTests.Tests
{
    public class PetTheoryTests : ApiTestBase
    {
        private readonly PetActions _petAction;

        public PetTheoryTests()
        {
            _petAction = new PetActions(PetClient);
        }

        //  Status parameterized tests
        //  The API accepts any status string; test that each one
        //  round-trips correctly (create → read).
        [Theory]
        [InlineData(PetStatus.Available)]
        [InlineData(PetStatus.Pending)]
        [InlineData(PetStatus.Sold)]
        [Trait("Category","CRUD")]
        public void CreatePet_WithStatus_ShouldBeRoundTrip(string status)
        {
            // ARRANGE
            var pet = TestDataFactory.CreatePet();
            pet.Status = status;
            // ACT
            var createdPet = _petAction.CreatePet(pet);
            TrackPet(pet.Id);
            var fetchedPet = _petAction.GetPet(pet.Id);
            // ASSERT
            createdPet.Status.Should().Be(status);
            fetchedPet.Status.Should().Be(status);
        }

        //  Edge-case name tests
        //  The demo API is permissive — it accepts any string.
        //  Assert SUCCESS (200 OK + correct round-trip).
        //  Rejection of invalid names is deferred to P1B (WireMock).
        [Theory]
        [InlineData("")]                                          // empty name
        [InlineData("A very long name that exceeds normal length and is designed to test boundary handling in the API and should be at least a couple hundred characters to stress any name-length assumptions the server might make and still be accepted by the demo endpoint")] // long name
        [InlineData("!@#$%^&*()_+={}[]|\\:\";<>?,./`~")]           // special chars
        [InlineData("日本語テスト")]                                  // unicode
        [Trait("Category", "EdgeCase")]
        public void CreatePet_WithEdgeCaseName_ShouldSucceed(string name)
        {
            //  ARRANGE
            var pet = TestDataFactory.CreatePet();
            pet.Name = name;
            //  ACT
            var createdPet = _petAction.CreatePet(pet);
            TrackPet(pet.Id);
            var fetchedPet = _petAction.GetPet(pet.Id);
            //  ASSERT
            createdPet.Name.Should().Be(name);
            fetchedPet.Name.Should().Be(name);
        }
    }
}