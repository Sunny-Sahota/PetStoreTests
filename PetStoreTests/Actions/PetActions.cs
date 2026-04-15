using PetStoreTests.Clients;
using PetStoreTests.Models;
using PetStoreTests.Utilities;
using System.Net;

namespace PetStoreTests.Actions
{
    public class PetActions
    {
        // Business Action Layer 
        private readonly PetClient _petClient;

        public PetActions(PetClient petClient)
        {
            _petClient = petClient;
        }

        public Pet CreatePet(Pet pet)
        {
            var response = RetryHelper.RetryUntil(
                action: () => _petClient.PostPet(pet),
                condition: r => r.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(r.Content)
            );
            return JsonHelper.DeserializeOrThrow<Pet>(response.Content);
        }

        public Pet GetPet(long id)
        {
            var response = RetryHelper.RetryUntil(
                action: () => _petClient.GetPetById(id),
                condition: r => r.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(r.Content)
            );
            return JsonHelper.DeserializeOrThrow<Pet>(response.Content);
        }
    }
}
