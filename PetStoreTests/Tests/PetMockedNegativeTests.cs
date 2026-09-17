using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Helpers;
using PetStoreTests.Models;
using System.Net;


namespace PetStoreTests.Tests
{
    public class PetMockedNegativeTests : IClassFixture<WireMockFixture>, IDisposable
    {
        private const string InvalidStatus = "bogus";

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

        [Fact]
        [Trait("Category","Negative")]
        public void FindByStatus_InvalidStatus_ShouldReturn400()
        {
            var response = _petClient.FindByStatus(InvalidStatus);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category","Negative")]
        public void FindByStatus_ValidStatus_ShouldReturn200()
        {
            var response = _petClient.FindByStatus(PetStatus.Available);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A very long name that exceeds normal length and is designed to test boundary handling in the API and should be at least a couple hundred characters to stress any name-length assumptions the server might make and still be accepted by the demo endpoint")]
        [InlineData("!@#$%^&*()_+={}[]|\\:\";<>?,./`~")]
        [InlineData("日本語テスト")]
        [Trait("Category", "EdgeCase")]
        public void CreatePet_EdgeCaseName_ShouldBeRejected(string name)
        {
            var pet = TestDataFactory.CreatePet();
            pet.Name = name;

            var response = _petClient.PostPet(pet);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void Dispose() => _petClient.Dispose();
    }
}