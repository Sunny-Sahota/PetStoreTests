using PetStoreTests.Clients;
using PetStoreTests.Models;
using PetStoreTests.Utilities;

namespace PetStoreTests.Actions
{
    public class PetActions
    {
        // Business Action Layer
        private readonly PetClient _petClient;

        public PetActions(PetClient petClient)
        {
            _petClient = petClient ?? throw new ArgumentNullException(nameof(petClient));
        }

        public Pet CreatePet(Pet pet)
        {
            var response = RetryHelper.RetryUntilSuccess(() => _petClient.PostPet(pet));
            return JsonHelper.DeserializeOrThrow<Pet>(response.Content);
        }

        public Pet GetPet(long id)
        {
            var response = RetryHelper.RetryUntilSuccess(() => _petClient.GetPetById(id));
            return JsonHelper.DeserializeOrThrow<Pet>(response.Content);
        }
    }
}
