using PetStoreTests.Models;

namespace PetStoreTests.Helpers
{
    public class TestDataFactory
    {
        // Test data creation
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
