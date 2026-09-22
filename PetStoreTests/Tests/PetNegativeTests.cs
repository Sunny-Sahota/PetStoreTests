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
            //  ARRANGE , use a random large ID that is effectively guaranteed not to exist on the shared demo API
            long nonExistentId = Random.Shared.NextInt64(1_000_000_000_000_000, long.MaxValue);
            //  ACT
            var response = _petClient.GetPetById(nonExistentId);
            //  ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void Dispose() => _petClient.Dispose();
    }
}