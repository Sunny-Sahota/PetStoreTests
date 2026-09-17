using PetStoreTests.Models;

namespace PetStoreTests.Helpers
{
    public class TestDataFactory
    {
        public const string DefaultPetName = "TestPet";
        public const string UpdatedPetName = "UpdatedPet";
        public const string DefaultCategoryName = "Dogs";
        public const string DefaultTagName = "friendly";
        public const string DefaultStatus = PetStatus.Available;

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

        public static Pet CreateCompletePet()
        {
            return new Pet
            {
                Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000 + Random.Shared.Next(0, 999),
                Name = "TestPet",
                Status = "available",
                Category = new Category { Id = 1, Name = "Dogs" },
                PhotoUrls = ["http://example.com/pic1.jpg"],
                Tags = [new Tag { Id = 1, Name = "friendly" }]
            };
        }
    }
}
