using PetStoreTests.Clients;
using PetStoreTests.Models;
using PetStoreTests.Utilities;
using RestSharp;
using System.Net;
using WireMock.ResponseBuilders;

namespace PetStoreTests.Actions
{
    public class PetActions
    {
        // Business Action Layer 
        private readonly PetClient _petClient;

        private static bool hasOkayResponseWithBody(RestResponse r) => r.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(r.Content);

        public PetActions(PetClient petClient)
        {
            _petClient = petClient ?? throw new ArgumentNullException(nameof(petClient));
        }

        public Pet CreatePet(Pet pet)
        {
            var response = RetryHelper.RetryUntil(
                action: () => _petClient.PostPet(pet),
                condition: hasOkayResponseWithBody,
                context: $"POST /pet (id: {pet.Id})"
            );
            return JsonHelper.DeserializeOrThrow<Pet>(response.Content);
        }

        public Pet GetPet(long id)
        {
            var response = RetryHelper.RetryUntil(
                action: () => _petClient.GetPetById(id),
                condition: hasOkayResponseWithBody,
                context: $"GET/pet/{id}"
            );
            return JsonHelper.DeserializeOrThrow<Pet>(response.Content);
        }

        public List<Pet> FindByStatus(string status)
        {
            var response = _petClient.FindByStatus(status);
            return JsonHelper.DeserializeOrThrow<List<Pet>>(response.Content);
        }

        public ApiResponse UploadImage(long id, string additionalMetadata, byte[] file)
        {
            var response = _petClient.UploadImage(id, additionalMetadata, file);
            return JsonHelper.DeserializeOrThrow<ApiResponse>(response.Content);
        }
    }
}
