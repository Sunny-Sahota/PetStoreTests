using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Helpers;
using System.Net;


namespace PetStoreTests.Tests
{
    public class PetMockedNegativeTests : IClassFixture<WireMockFixture>
    {
        private readonly PetClient _petClient;
        private readonly PetActions _petAction;

        public PetMockedNegativeTests(WireMockFixture fixture) 
        {
            _petClient = new PetClient(fixture.Server.Url!);
            _petAction = new PetActions(_petClient);
        }

        // Proves the test runs hermetically(closed/sealed): the GET is answered by the mock, never the internet.
        [Fact]
        [Trait("Category", "Negative")]
        public void PetClient_ShouldHitMock_NotTheInternet()
        {
            var response = _petClient.GetPetById(1);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category","Negative")]
        public void PostPet_InvalidBody_ShouldReturn404()
        {
            var invalidBody = new { };

            var response = _petClient.PostPet(invalidBody);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category","Negative")]
        public void PutPet_MismatchedId_ShouldFail()
        {
            var pet = TestDataFactory.CreatePet();
            _petAction.CreatePet(pet);

            var otherPet = TestDataFactory.CreatePet();
            otherPet.Id = pet.Id + 1;

            var response = _petClient.PutPet(otherPet);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category","Negative")]
        public void DeletePet_AlreadyDeleted_ShouldBeDeleted()
        {
            var pet = TestDataFactory.CreatePet();
            _petAction.CreatePet(pet);

            var firstDelete = _petClient.DeletePet(pet.Id);
            var secondDelete = _petClient.DeletePet(pet.Id);

            firstDelete.StatusCode.Should().Be(HttpStatusCode.OK);
            secondDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}