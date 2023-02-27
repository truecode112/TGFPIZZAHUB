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
                var baseUrl = string.Format("https://api.hubrise.com/v1/location/orders/{0}", orderData.order_id);
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
                    string curPath = Directory.GetCurrentDirectory();
                    string target = Path.Combine(curPath, "Orders");
                    if (Directory.Exists(target))
                    {
                        File.Delete(Path.Combine(target, orderData.order_id + ".data"));
                    }
                        /*
                        string? orderSavedData = null;
                        try
                        {
                            orderSavedData = (string)localStorage.Get("Orders");
                        }
                        catch (Exception e)
                        {
                        }

                        if (orderSavedData != null)
                        {
                            List<Order>? orderArray;
                            orderArray = JsonConvert.DeserializeObject<List<Order>>(orderSavedData);
                            if (orderArray != null)
                            {
                                Order? findOrder = orderArray.Find(order => (order.OrderId == orderData.order_id && order.IsOrdered == false));
                                if (findOrder != null)
                                {
                                    findOrder.IsOrdered = true;
                                    var orderArrayString = JsonConvert.SerializeObject(orderArray);
                                    localStorage.Store("Orders", orderArrayString);
                                    localStorage.Persist();
                                }
                            }
                        }*/
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveOrderAccept", "OK", orderData.order_id);
                } else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveOrderAccept", response.Content, orderData.order_id);
                }
            }
            Console.WriteLine(postData);
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            //string? orderSavedData = null;
            await base.OnConnectedAsync();

            string curPath = Directory.GetCurrentDirectory();
            string target = Path.Combine(curPath, "Orders");
            if (Directory.Exists(target))
            {
                DirectoryInfo d = new DirectoryInfo(target);
                List<JObject>? orderArray = new List<JObject>();
                foreach (var file in d.GetFiles("*.data"))
                {
                    string orderString = File.ReadAllText(file.FullName);
                    if (orderString != null)
                    {
                        var orderObj = JsonConvert.DeserializeObject<JObject>(orderString);
                        if (orderObj != null) {
                            orderArray.Add(orderObj);
                        }
                    }
                }
                await Clients.Client(connectionId).SendAsync("OldMessages", JsonConvert.SerializeObject(orderArray));
            }

            /*
            try
            {
                orderSavedData = (string)localStorage.Get("Orders");
            }
            catch (Exception e)
            {
            }

            if (orderSavedData != null)
            {
                List<Order>? orderArray;
                List<Order>? unAcceptArray;
                orderArray = JsonConvert.DeserializeObject<List<Order>>(orderSavedData);
                if (orderArray != null)
                {
                    unAcceptArray = orderArray.FindAll(order => (order.IsOrdered == false || order.IsOrdered == null));
                    if (unAcceptArray != null)
                    {
                        await Clients.Client(connectionId).SendAsync("OldMessages", JsonConvert.SerializeObject(unAcceptArray));
                    }
                }
            }*/
        }
    }
}
