using Hanssens.Net;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TGFPIZZAHUB.Models;
using WinPizzaData;

namespace TGFPIZZAHUB.Hubs
{
    public class ChatHub : Hub
    {
        public async Task<string> PrintTicket(string orderNumber)
        {
            return "Success";
        }

        public async Task SendData(string postData)
        {
            ChangeOrderStatusModel? orderData = JsonConvert.DeserializeObject<ChangeOrderStatusModel>(postData);
            if (orderData != null)
            {
                if (orderData.status == "confirmed")
                {
                    string curPath = Directory.GetCurrentDirectory();
                    string target = Path.Combine(curPath, "Orders");
                    File.Delete(Path.Combine(target, orderData.order_id + ".data"));
                    await Clients.Client(Context.ConnectionId).SendAsync("ChangeOrderStatusOK", orderData.status, orderData.order_id);
                } else
                {
                    var baseUrl = string.Format("https://api.hubrise.com/v1/location/orders/{0}", orderData.order_id);
                    var options = new RestClientOptions(baseUrl)
                    {
                        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    };

                    var client = new RestClient(options);
                    var request = new RestRequest();
                    request.AddHeader("Access-Control-Allow-Origin", "*");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("X-Access-Token", "02117ecb2c2fb78e4da8c33d1d031bd1");
                    JObject bodyObject = new JObject();
                    bodyObject.Add("status", orderData.status);
                    if (orderData.status == "accepted")
                        bodyObject.Add("confirmed_time", orderData.confirmed_time);
                    request.AddParameter("application/json", bodyObject.ToString(), ParameterType.RequestBody);
                    RestResponse response = await client.PatchAsync(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string curPath = Directory.GetCurrentDirectory();
                        string target = Path.Combine(curPath, "Orders");

                        if (Directory.Exists(target))
                        {
                            if (orderData.status == "rejected")
                            {
                                File.Delete(Path.Combine(target, orderData.order_id + ".data"));
                            }
                            else
                            {
                                string orderString = File.ReadAllText(Path.Combine(target, orderData.order_id + ".data"));
                                if (orderString != null)
                                {
                                    var orderObj = JsonConvert.DeserializeObject<Order>(orderString);
                                    if (orderObj != null)
                                    {
                                        orderObj.IsAccepted = true;
                                        using StreamWriter file = new(Path.Combine(target, orderData.order_id + ".data"));
                                        orderString = JsonConvert.SerializeObject(orderObj);
                                        await file.WriteAsync(orderString);
                                    }
                                }
                            }
                        }
                        await Clients.Client(Context.ConnectionId).SendAsync("ChangeOrderStatusOK", orderData.status, orderData.order_id);
                    }
                    else
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("ChangeOrderStatusFail", response.Content, orderData.order_id);
                    }
                }
            }
            Console.WriteLine(postData);
        }

        public void sendOrderToTGFPIZZA(Order order)
        {
            Basket basket = convertOrderToBasket(order);

        }

        public decimal convert2Decimal(string strPrice)
        {
            if (strPrice == null)
                return 0;
            string strRealPrice = strPrice.Split(" ")[0];
            if (strRealPrice == null)
                return 0;
            return Convert.ToDecimal(strRealPrice);
        }

        public int convert2Int(string str)
        {
            if(str == null) return 0;
            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }
            return 0;
        }

        public Basket? convertOrderToBasket(Order? order)
        {
            if (order == null)
                return null;
            Basket basket = new Basket();
            HubRiseModel? hubriseOrder = JsonConvert.DeserializeObject<HubRiseModel>(order.Json);
            if (hubriseOrder != null && hubriseOrder.NewStateObj != null)
            {
                basket.DeOrderHeader.ID = hubriseOrder.Id;
                basket.DeOrderHeader.Total = convert2Decimal(hubriseOrder.NewStateObj.Total);
                basket.DeOrderHeader.DeOrdTimeInfo.TimedOrd = hubriseOrder.NewStateObj.CreatedAt;
                basket.DeOrderHeader.DeOrdTimeInfo.TExpectedOrdReady = hubriseOrder.NewStateObj.ExpectedTime;
                basket.DeOrderHeader.IsOpen = true;
                basket.DeOrderHeader.AddressStr = hubriseOrder.NewStateObj.Customer.Address1.ToString() + hubriseOrder.NewStateObj.Customer.Address2.ToString();
                basket.DeOrderHeader.DePeople.ID = hubriseOrder.NewStateObj.Customer.Id.ToString();
                basket.DeOrderHeader.DePeople.Phone = hubriseOrder.NewStateObj.Customer.Phone;
                basket.DeOrderHeader.DePeople.Name = hubriseOrder.NewStateObj.Customer.FirstName + hubriseOrder.NewStateObj.Customer.LastName;
                basket.DeOrderHeader.DePeople.Email = hubriseOrder.NewStateObj.Customer.Email.ToString();

                basket.DeOrderLines = new List<OrderLine>(hubriseOrder.NewStateObj.Items.Count);
                foreach(HubRiseModel.Item item in hubriseOrder.NewStateObj.Items)
                {
                    OrderLine orderLine = new OrderLine();
                    orderLine.Name = item.ProductName;
                    orderLine.Price = convert2Decimal(item.Subtotal);
                    orderLine.Qty = convert2Int(item.Quantity);

                }
            }
            
            return null;
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
                List<Order>? orderArray = new List<Order>();
                foreach (var file in d.GetFiles("*.data"))
                {
                    string orderString = File.ReadAllText(file.FullName);
                    if (orderString != null)
                    {
                        var orderObj = JsonConvert.DeserializeObject<Order>(orderString);
                        if (orderObj != null) {
                            orderArray.Add(orderObj);
                        }
                    }
                }

                var qry = from p in orderArray
                          orderby p.ExpectTime
                          select p;

                await Clients.Client(connectionId).SendAsync("CurrentOrders", JsonConvert.SerializeObject(qry));
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
