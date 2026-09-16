using PetStoreTests.Models;

namespace PetStoreTests.Helpers
{
    public class TestDataFactory
    {
        //  Test data creation
        //  Note: I had to make Id more unique because the tests run in parallel so the same id was being used based on Date time
        public static Pet CreatePet()
        {
            return new Pet
            {
                Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000 + Random.Shared.Next(0, 999),
                Name = "TestPet",
                Status = "available"
            };
        }
    }
}
