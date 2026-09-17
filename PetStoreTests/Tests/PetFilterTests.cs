using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;

namespace PetStoreTests.Tests
{
    public class PetFilterTests
    {
        private readonly PetClient _petClient = new();
        private readonly PetActions _petAction;

        public PetFilterTests()
        {
            _petAction = new PetActions(_petClient);
        }

        [Fact]
        [Trait("Category","Filter")]
        public void FindByStatus_WithValidStatus_ShouldReturnPets()
        {
            //  ARRANGE
            var statuses = new[] {"available","pending","sold"};

            foreach(var status in statuses)
            {
                //  ACT
                var pets = _petAction.FindByStatus(status);

                //  ASSERT - loose for shared API
                pets.Should().NotBeNull();
                pets.Should().NotBeEmpty();

                //  Every Returned pet should match the requested status
                pets.Should().OnlyContain(p => p.Status == status);
            }
        }
    }
}