using PetStoreTests.Clients;
using PetStoreTests.Config;
using Xunit.Abstractions;

namespace PetStoreTests.Utilities
{
    public class ApiTestBase : IAsyncLifetime
    {
        //  Centralized Breakdown - every pet that a test creates is deleted when the test ends
        protected readonly PetClient PetClient;
        private readonly List<long> _trackedPetIds = [];

        // xUnit injects ITestOutputHelper; each HTTP exchange is logged to it and
        // only surfaces in output if the test fails.
        protected ApiTestBase(ITestOutputHelper output)
        {
            PetClient = new PetClient(ApiConfig.BaseUrl, line => output.WriteLine(line));
        }

        protected void TrackPet(long id) => _trackedPetIds.Add(id);

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            foreach(var id in _trackedPetIds)
            {
                PetClient.DeletePet(id);
            }
            PetClient.Dispose();
            return Task.CompletedTask;
        }
    }
}