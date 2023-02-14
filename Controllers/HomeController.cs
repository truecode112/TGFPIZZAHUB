using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using TGFPIZZAHUB.Hubs;
using TGFPIZZAHUB.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TGFPIZZAHUB.Controllers
{
    public class HomeController : Controller
    {
        public static string ACCOUNT_ACCESS_TOKEN = "463ddd4b60ada35d4a52055d55a85f53";

        private readonly ILogger<HomeController> _logger;

        private IHubContext<ChatHub> HubContext { get; set; }

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            HubContext = hubContext;
        }

        /*public IActionResult Index(HubRiseModel model)
        {
            ViewBag.Data = model.Message;
            return View(model);
        }*/

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View("Index");
        }

        [HttpPost("/hubrise_callback")]
        public async Task<IActionResult> HubRiseCallback([FromBody] HubRiseModel model)
        {
            var jsonString = JsonConvert.SerializeObject(model);
            
            //string prettyStr;
            //prettyStr = JToken.Parse(jsonString).ToString(Formatting.Indented);

            //await this.HubContext.Clients.All.SendAsync("ReceiveMessage", prettyStr);
            if (model != null) {

                string formatted = FormatOrder(model);
                await this.HubContext.Clients.All.SendAsync("ReceiveMessage", formatted, jsonString);
                return Ok("Received an order");
            }

            return Ok("Received an order");
        }

        private string FormatOrder(HubRiseModel model)
        {
            string formatted = "";
            formatted += string.Format("<div><b>Order No: {0}</b></div>", model.OrderId);
            formatted += string.Format("<div>{0}</div>", model.CreatedAt);
            formatted += string.Format("<div style=\"font-size: small;\"><b>Name: {0} {1}</b></div>", model.NewStateObj.Customer.FirstName, model.NewStateObj.Customer.LastName);
            formatted += "<br/>";
            if (model.NewStateObj.Customer.Phone != null && model.NewStateObj.Customer.Phone != "") {
                formatted += string.Format("<div style=\"text-align: right;\">Tel: {0}</div>", model.NewStateObj.Customer.Phone);
            } else {
                formatted += string.Format("<div style=\"text-align: right;\">Tel: No number</div>");
            }

            formatted += string.Format("<div><b>{0}</b><br/></div>", model.NewStateObj.Customer.Address1);
            formatted += string.Format("<div><b>{0}</b><br/></div>", model.NewStateObj.Customer.Address2);

            Dictionary<string, string> optionDict = new Dictionary<string, string>();

            foreach(var item in model.NewStateObj.Items) {
                optionDict.Clear();
                if (item != null) {
                    formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div>{0} {1}</div><div>{2}</div></div>",
                        (int)(float.Parse(item.Quantity)), item.ProductName, item.Price);
                
                    if (item.Options.Count > 0)
                    {
                        foreach(var option in item.Options)
                        {
                            if (optionDict.ContainsKey(option.OptionListName)) {
                                string? value;
                                if (optionDict.TryGetValue(option.OptionListName, out value)) {
                                    value += "," + option.Name;
                                    optionDict.Remove(option.OptionListName);
                                    optionDict.Add(option.OptionListName, value);
                                }
                            } else {
                                optionDict.Add(option.OptionListName, option.Name);
                            }
                        }

                        foreach(KeyValuePair<string,string> entry in optionDict) {
                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0}: {1}</div></div>",
                                entry.Key, entry.Value);
                        }
                    }
                }
            }

            float totalDiscount = 0.0f;
            foreach(var discount in model.NewStateObj.Discounts) {
                if (discount != null) {
                    formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div>{0} {1}</div><div>-{2}</div></div>",
                            1, discount.Name, discount.PricingValue);
                    
                    string price = discount.PricingValue.Replace("GBP", "");
                    price = price.Replace(" ", "");

                    float discountPrice = float.Parse(price);
                    totalDiscount += discountPrice;
                }
            }

            formatted += string.Format("<br/><div><b>*** DEALS ***</b></div>");

            if (model.NewStateObj.Deals.Name != null && model.NewStateObj.Deals.Name != "") {
                formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div>{0} {1}</div></div>",
                            1, model.NewStateObj.Deals.Name);
            }

            // foreach(var deal in model.NewStateObj.Deals) {                
            // }
            
            formatted += string.Format("<hr>");
            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left: 50px;\"><b>Total</b></div><div>{0}</div></div>", model.NewStateObj.Total);
            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div>You have saved:</div><div>{0} GBP</div></div>", totalDiscount);

            return formatted;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        { 
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}