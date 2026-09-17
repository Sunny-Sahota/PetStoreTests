using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Models;

namespace PetStoreTests.Tests
{
    public class PetFilterTests : IDisposable
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
            var statuses = new[] { PetStatus.Available, PetStatus.Pending, PetStatus.Sold };

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

        public void Dispose() => _petClient.Dispose();
    }
}