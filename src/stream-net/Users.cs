using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stream.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stream
{
    public class User
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "data")]
        public IDictionary<string, object> Data { get; set; }
    };

    public class Users
    {
        readonly StreamClient _client;

        internal Users(StreamClient client)
        {
            _client = client;
        }

        public async Task<User> Add(string userID, IDictionary<string, object> data = null, bool getOrCreate = false)
        {
            var u = new User()
            {
                ID = userID,
                Data = data,
            };
            var request = this._client.BuildJWTAppRequest("user/", HttpMethod.POST);

            request.SetJsonBody(JsonConvert.SerializeObject(u));
            request.AddQueryParameter("get_or_create", getOrCreate.ToString());

            var response = await this._client.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return JsonConvert.DeserializeObject<User>(response.Content);

            throw StreamException.FromResponse(response);
        }

        public async Task<User> Get(string userID)
        {
            var request = this._client.BuildJWTAppRequest($"user/{userID}/", HttpMethod.GET);

            var response = await this._client.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<User>(response.Content);

            throw StreamException.FromResponse(response);
        }

        public async Task<User> Update(string userID, IDictionary<string, object> data)
        {
            var u = new User()
            {
                Data = data,
            };
            var request = this._client.BuildJWTAppRequest($"user/{userID}/", HttpMethod.PUT);
            request.SetJsonBody(JsonConvert.SerializeObject(u));

            var response = await this._client.MakeRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return JsonConvert.DeserializeObject<User>(response.Content);

            throw StreamException.FromResponse(response);
        }

        public async Task Delete(string userID)
        {
            var request = this._client.BuildJWTAppRequest($"user/{userID}/", HttpMethod.DELETE);

            var response = await this._client.MakeRequest(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw StreamException.FromResponse(response);
        }
    };
}
