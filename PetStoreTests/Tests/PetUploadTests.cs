using FluentAssertions;
using PetStoreTests.Actions;
using PetStoreTests.Clients;
using PetStoreTests.Helpers;
using PetStoreTests.Utilities;

namespace PetStoreTests.Tests
{
    public class PetUploadTests : ApiTestBase
    {
        private readonly PetActions _petAction;

        public PetUploadTests()
        {
            _petAction = new PetActions(PetClient);
        }

        [Fact]
        [Trait("Category","Upload")]
        public void UploadImage_ToExistingPet_ShouldReturnSuccess()
        {
            //  ARRANGE
            var pet = TestDataFactory.CreatePet();
            _petAction.CreatePet(pet);
            TrackPet(pet.Id);

            //  FAKE PNG Bytes - API doesn't validate content
            byte[] image = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
            const string metadata = "Integration Test Image";

            //  ACT
            var response = _petAction.UploadImage(pet.Id, metadata, image);

            //  ASSERT
            response.Code.Should().Be(200);
            response.Message.Should().NotBeNullOrEmpty();
        }
    }
}