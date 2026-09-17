using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Helpers;
using PetStoreTests.Models;
using System.Net;
namespace PetStoreTests.Tests
{
    public class PetNegativeTests
    {
        private readonly PetClient _petClient = new();
        private readonly PetActions _petAction = new(new PetClient());

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
    }
}