using PetStoreTests.Config;
using RestSharp;

namespace PetStoreTests.Clients
{
    public class PetClient
    {
        // Raw HTTP communication
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

        public RestResponse FindByStatus(string status) => Execute($"/pet/findByStatus?status={status}", Method.Get);

        public RestResponse UploadImage(long Id, string additionalMetaData, byte[] file)
        {
            var request = new RestRequest($"/pet/{Id}/uploadImage",Method.Post);
            request.AddParameter("additionalMetadata",additionalMetaData);
            request.AddFile("file",file,"test-image.png");
            return _restClient.Execute(request);   
        }
    }
}
