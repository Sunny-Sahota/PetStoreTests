using FluentAssertions;
using PetStoreTests.Clients;
using System.Net;
namespace PetStoreTests.Tests
{
    public class PetNegativeTests : IDisposable
    {
        private readonly PetClient _petClient = new();

        public PetNegativeTests() { }

        [Fact]
        [Trait("Category","Negative")]
        public void GetPet_NonexistentId_ShouldReturn404()
        {
            //  ARRANGE , Use hardcoded ID that is unlikely to exist 
            long nonExistentId = 999999999;
            //  ACT
            var response = _petClient.GetPetById(nonExistentId);
            //  ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void Dispose() => _petClient.Dispose();
    }
}