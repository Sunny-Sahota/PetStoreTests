using FluentAssertions;
using System.Net;
using RestSharp;

namespace PetStoreTests.Utilities
{
    public static class ApiAssertions
    {
        // Shared helpers (Assertions)
        public static void ShouldBeOk(RestResponse response)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNullOrEmpty();
        }

        public static void ShouldBeOkOrNotFound(RestResponse response)
        {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }

        public static void ShouldBeSuccess(RestResponse response)
        {
            ((int)response.StatusCode).Should().BeInRange(200, 299);
        }

        public static void ShouldBeDeletedOrNotFound(RestResponse response)
        {
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.OK,          // Deleted Successfully
                HttpStatusCode.NotFound,    // Already deleted / never existed
                HttpStatusCode.BadRequest   // Invalid ID supplied
                );
        }
    }
}
