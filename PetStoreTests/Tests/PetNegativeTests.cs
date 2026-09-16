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

        // Skipped: PetStore demo API accepts empty bodies and returns 200 OK.
        // The API lacks input validation — it creates a pet with defaults (id=0, name="").
        // Re-enable once WireMock.Net is in place (P5) to enforce strict body validation.
        [Fact(Skip = "Demo API lacks body validation — needs WireMock")]
        [Trait("Category", "Negative")]
        public void PostPet_InvalidBody_ShouldReturn404()
        {
            //  ARRANGE , send an empty object - The API expects at minimum name
            var invalidBody = new { };
            //  ACT
            var response = _petClient.PostPet(invalidBody);
            //  ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Skipped: PetStore demo API treats PUT as an upsert and accepts any valid pet object.
        // It does not reject mismatched IDs — it just creates/updates based on the body's id field.
        // Re-enable once WireMock.Net is in place (P5) to enforce strict PUT semantics.
        [Fact(Skip = "Demo API treats PUT as upsert — needs WireMock")]
        [Trait("Category","Negative")]
        public void PutPet_MismatchedId_ShouldFail()
        {
            //  ARRANGE 
            var pet = TestDataFactory.CreatePet();
            _petAction.CreatePet(pet);

            //  Attempt to put the second pet Id in the first one 
            var otherPet = TestDataFactory.CreatePet();
            otherPet.Id = pet.Id + 1;

            //  ACT
            var response = _petClient.PutPet(otherPet);

            //  ASSERT, API should reject or not find pet
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed);
        }

        // Skipped: PetStore demo API is idempotent and returns 200 OK for repeated deletes.
        // It does not track state or return 404 for an already-deleted pet.
        // Re-enable once WireMock.Net is in place (P5) to enforce stateful delete semantics.
        [Fact(Skip = "Demo API allows repeat deletes — needs WireMock")]
        [Trait("Category","Negative")]
        public void DeletePet_AlreadyDeleted_ShouldBeDeleted()
        {
            //  ARRANGE
            var pet = TestDataFactory.CreatePet();
            _petAction.CreatePet(pet);
            //  ACT
            var firstDelete = _petClient.DeletePet(pet.Id);
            firstDelete.StatusCode.Should().Be(HttpStatusCode.OK);

            var secondDelete = _petClient.DeletePet(pet.Id);
            //  ASSERT
            secondDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}