using PetStoreTests.Models;

namespace PetStoreTests.Helpers
{
    public class TestDataFactory
    {
        // Create Test data objects
        public static Pet CreatePet()
        {
            return new Pet
            {
                Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Name = "TestPet",
                Status = "available"
            };
        }
    }
}
