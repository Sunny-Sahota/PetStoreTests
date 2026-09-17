using PetStoreTests.Clients;
using PetStoreTests.Models;
using Xunit;

namespace PetStoreTests.Utilities
{
    public class ApiTestBase : IAsyncLifetime
    {
        //  Centralized Breakdown - every pet that a test creates is deleted when the test ends
        protected readonly PetClient PetClient = new();
        private readonly List<long> _trackedPetIds = [];

        protected void TrackPet(long id) => _trackedPetIds.Add(id);

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            foreach(var id in _trackedPetIds)
            {
                PetClient.DeletePet(id);
            }
            return Task.CompletedTask;
        }
    }
}