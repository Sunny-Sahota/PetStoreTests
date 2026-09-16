namespace PetStoreTests.Models
{
    public class Pet
    {
        // Domain models
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        public Category? Category { get; set; }
        public List<string> PhotoUrls { get; set; } = [];
        public List<Tag> Tags { get; set; } = [];
    }
}
