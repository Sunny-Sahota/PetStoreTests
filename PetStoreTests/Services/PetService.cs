using PetStoreTests.Clients;
using PetStoreTests.Models;
using PetStoreTests.Utilities;

namespace PetStoreTests.Services
{
    public class PetService
    {
        // Business logic / polling
        private readonly PetClient _petClient;

        public PetService(PetClient petClient)
        {
            _petClient = petClient ?? throw new ArgumentNullException(nameof(petClient));
        }

        public Pet WaitForPetNameToBe(long id, string expectedName)
        {
            // Polls GET /pet/{id} until the returned pet's name matches (eventual consistency)
            return RetryHelper.RetryUntil(
                action: () => JsonHelper.DeserializeOrThrow<Pet>(_petClient.GetPetById(id).Content),
                condition: pet => pet != null && pet.Name == expectedName,
                retries: 10,
                delayMs: 500,
                context: $"GET /pet/{id} - waiting for pet name to be '{expectedName}'"
            )!;
        }

        public List<Pet> WaitForPetInStatusSearch(long id, string status)
        {
            return RetryHelper.RetryUntil(
                action: () => JsonHelper.DeserializeOrThrow<List<Pet>>(_petClient.FindByStatus(status).Content),
                condition: pets => pets != null && pets.Any(p => p.Id == id),
                retries: 10,
                delayMs: 500,
                context: $"GET /pet/findByStatus?status={status} - waiting for pet {id}"
            )!;
        }
    }
}
