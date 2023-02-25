using Hanssens.Net;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TGFPIZZAHUB.Models;

namespace TGFPIZZAHUB.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendData(string postData)
        {
            

            AcceptOrderModel? orderData = JsonConvert.DeserializeObject<AcceptOrderModel>(postData);
            if (orderData != null)
            {
                var baseUrl = string.Format("https://api.hubrise.com/v1/location/{0}/orders/{1}", orderData.location_id, orderData.order_id);
                var options = new RestClientOptions(baseUrl) {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                };

                var client = new RestClient(options);
                var request = new RestRequest();
                request.AddHeader("Access-Control-Allow-Origin", "*");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-Access-Token", "02117ecb2c2fb78e4da8c33d1d031bd1");
                JObject bodyObject = new JObject();
                bodyObject.Add("status", "accepted");
                bodyObject.Add("confirmed_time", orderData.confirmed_time);
                request.AddParameter("application/json", bodyObject.ToString(), ParameterType.RequestBody);
                RestResponse response = await client.PatchAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveOrderAccept", "OK");
                } else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveOrderAccept", "Fail");
                }
            }
            Console.WriteLine(postData);
        }

        LocalStorage localStorage = new LocalStorage();

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string? orderSavedData = null;
            await base.OnConnectedAsync();

            try
            {
                orderSavedData = (string)localStorage.Get("Orders");
            }
            catch (Exception e)
            {
            }

            if (orderSavedData != null)
            {
                await Clients.Client(connectionId).SendAsync("OldMessages", orderSavedData);
            }
        }
    }
}
