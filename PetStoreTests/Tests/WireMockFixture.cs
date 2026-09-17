using DevLab.JmesPath.Functions;
using PetStoreTests.Models;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PetStoreTests.Tests
{
    // One fake server shared by every test in the class; disposed when the class finishes.
    public class WireMockFixture : IDisposable
    {
        public WireMockServer Server { get; }

        public WireMockFixture()
        {
            Server = WireMockServer.Start();    //  Grabs free Local Host port
            ConfigureStubs();
        }

        private void ConfigureStubs()
        {
            // 1) POST /pet with an EMPTY body {} -> 400 (invalid body rejected)
            Server
                .Given(Request.Create().WithPath("/pet").UsingPost().WithBody(new ExactMatcher("{}")))
                .RespondWith(Response.Create().WithStatusCode("400"));
            
            // 2) Any other POST /pet -> 200 + a valid pet (so CreatePet works in tests)
            Server
                .Given(Request.Create().WithPath("/pet").UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode("200")
                    .WithBodyAsJson(new Pet { Id = 1, Name = "Mock", Status = "available" }));
            
            // 3) PUT /pet -> 404 (pet with a mismatched id "does not exist")
            Server
                .Given(Request.Create().WithPath("/pet").UsingPut())
                .RespondWith(Response.Create().WithStatusCode("404"));

            // 4) DELETE /pet/{id} -> 200 the first time, then 404 (stateful scenario)
            Server
                .Given(Request.Create().WithPath("/pet/*").UsingDelete())
                .InScenario("DeletePet")
                .WillSetStateTo("Deleted")
                .RespondWith(Response.Create().WithStatusCode("200"));

            Server
                .Given(Request.Create().WithPath("/pet/*").UsingDelete())
                .InScenario("DeletePet")
                .WhenStateIs("Deleted")
                .RespondWith(Response.Create().WithStatusCode("404"));
        }
        public void Dispose() => Server.Stop();
    }
}