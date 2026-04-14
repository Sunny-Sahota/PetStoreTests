using PetStoreTests.Config;
using RestSharp;

namespace PetStoreTests.Clients
{
    public class PetClient
    {
        private readonly RestClient _restClient;

        public PetClient()
        {
            _restClient = new RestClient(ApiConfig.BaseUrl);
        }

        private RestResponse Execute(string resource,Method method,object? body = null)
        {
            var request = new RestRequest(resource, method);

            if (body != null)
                request.AddJsonBody(body);

            return _restClient.Execute(request);
        }

        public RestResponse PostPet(object body) => Execute("/pet", Method.Post, body);

        public RestResponse GetPetById(long id) => Execute($"/pet/{id}", Method.Get);

        public RestResponse PutPet(object body) => Execute("/pet", Method.Put, body);

        public RestResponse DeletePet(long id) => Execute($"/pet/{id}", Method.Delete);
    }
}
