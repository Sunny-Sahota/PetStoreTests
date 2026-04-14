using Newtonsoft.Json;
using PetStoreTests.Clients;
using PetStoreTests.Models;
using PetStoreTests.Utilities;

namespace PetStoreTests.Services
{
    public class PetService
    {
        private readonly PetClient _petClient;

        public PetService(PetClient petClient)
        {
            _petClient = petClient ?? throw new ArgumentNullException(nameof(petClient));
        }

        public Pet WaitForPetNameToBe(long id, string expectedName)
        {
            return RetryHelper.RetryUntil(
                action: () =>
                {
                    var response = _petClient.GetPetById(id);

                    if (string.IsNullOrEmpty(response.Content))
                        return null;

                    var pet = JsonConvert.DeserializeObject<Pet>(response.Content);

                    return pet?.Name == expectedName ? pet : null;
                },
                retries: 10,
                delayMs: 500
            ) ?? throw new Exception($"Pet with id {id} was not updated to '{expectedName}'");
        }
    }
}
