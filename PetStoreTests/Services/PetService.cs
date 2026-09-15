using Newtonsoft.Json;
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
            // Added null forgiving '!' for compiler since im checking null in condition that passes into retry helper
            return RetryHelper.RetryUntil(
                action: () =>
                {
                    var response = _petClient.GetPetById(id);
                    return JsonConvert.DeserializeObject<Pet>(response.Content!);
                },
                condition: pet => pet != null && pet.Name == expectedName,
                retries: 10,
                delayMs: 500
            )!;
        }
    }
}
