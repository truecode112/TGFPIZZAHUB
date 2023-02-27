using Hanssens.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Pipelines;
using TGFPIZZAHUB.Hubs;
using TGFPIZZAHUB.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TGFPIZZAHUB.Models.HubRiseModel;
using System.IO;

namespace TGFPIZZAHUB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IHubContext<ChatHub> HubContext { get; set; }

        LocalStorage localStorage = new LocalStorage();

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

                saveOrderToFile(model.OrderId, formatted, jsonString);
                /*
                string orderSavedData;

                try
                {
                    orderSavedData = (string)localStorage.Get("Orders");
                } catch (Exception e)
                {
                    orderSavedData = null;
                }

                List<Order> orderArray;

                if (orderSavedData!= null)
                {
                    orderArray = JsonConvert.DeserializeObject<List<Order>>(orderSavedData);
                    if (orderArray == null)
                    {
                        orderArray = new List<Order>();
                    }
                }
                else
                {
                    orderArray = new List<Order>();
                }

                Order newOrder = new Order
                {
                    Formated = formatted,
                    Json = jsonString,
                    IsOrdered = false,
                    OrderId = model.OrderId
                };
                orderArray.Add(newOrder);
                var orderArrayString = JsonConvert.SerializeObject(orderArray);
                localStorage.Store("Orders", orderArrayString);
                localStorage.Persist();*/

                await this.HubContext.Clients.All.SendAsync("ReceiveMessage", formatted, jsonString);
                return Ok("Received an order");
            }

            return Ok("Received an order");
        }

        public async void saveOrderToFile(string orderId, string formattedString, string jsonString)
        {
            try
            {
                string curPath = Directory.GetCurrentDirectory();
                string target = Path.Combine(curPath, "Orders");
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                JObject jsonObj = new JObject();
                jsonObj.Add("formatted", formattedString);
                jsonObj.Add("json", jsonString);
                string orderString = JsonConvert.SerializeObject(jsonObj);
                using StreamWriter file = new(Path.Combine(target, orderId + ".data"));
                await file.WriteAsync(orderString);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
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
            Dictionary<string, int> optionQuantity = new Dictionary<string, int>();
            Dictionary<string, float> optionprice = new Dictionary<string, float>();
            string currency = "GBP";

            foreach (var item in model.NewStateObj.Items) {
                optionDict.Clear();
                foreach (var option in item.Options)
                {
                    if (option.Price != null)
                    {
                        option.Price = option.Price.Split(" ")[0];  //Remove currency
                    }
                }

                if (item != null) {
                    if (item.SkuRef.Contains("OFFER", StringComparison.OrdinalIgnoreCase))
                    {
                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div>{0} {1}</div><div>{2}</div></div>",
                            (int)(float.Parse(item.Quantity)) + " X", item.ProductName, item.Subtotal.Split(" ")[0]);

                        var choiceStart = false;
                        var choiceNames = "";
                        var choiceTitle = "";
                        int choiceQuantity = 0;
                        var choicePrice = "";
                        var choiceOPs = "";
                        foreach (var option in item.Options)
                        {
                            if (option != null && option.OptionListName.StartsWith("Choice", StringComparison.OrdinalIgnoreCase))
                            {
                                if (choiceStart)
                                {
                                    // Print
                                    if (choiceNames.Length > 0)
                                    {
                                        choiceNames = choiceNames.Remove(choiceNames.Length - 1);
                                    }

                                    if (choiceNames.Trim().Length > 0)
                                    {
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                            choiceQuantity + " X", choiceTitle + "(" + choiceNames + ")", /*choicePrice*/"");
                                    } else
                                    {
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                            choiceQuantity + " X", choiceTitle, /*choicePrice*/"");
                                    }

                                    formatted += choiceOPs;

                                    choiceQuantity = 0;
                                    choiceTitle = option.Name;
                                    choiceQuantity = option.Quantity;
                                    choicePrice = option.Price;
                                    choiceNames = "";
                                    choiceOPs = "";
                                } else
                                {
                                    choiceStart = true;
                                    choiceTitle = option.Name;
                                    choiceQuantity = option.Quantity;
                                    choicePrice = option.Price;
                                    choiceNames = "";
                                    choiceOPs = "";
                                }
                            }
                            else
                            {
                                if (choiceStart)
                                {
                                    if (option.OptionListName.StartsWith("Remove", StringComparison.OrdinalIgnoreCase) && option.Ref != null && option.Ref.Contains("TOP", StringComparison.OrdinalIgnoreCase))
                                    {
                                        choiceOPs += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:40px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                            "-", option.Name, ""/*option.Price*/);
                                    } else if (option != null && option.Ref != null && option.Ref.StartsWith("OP", StringComparison.OrdinalIgnoreCase))
                                    {
                                        choiceOPs += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:40px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                                    option.Ref.Split("-")[1].Trim() + ":", option.Name, /*option.Price*/"");

                                    } else
                                    {
                                        choiceNames += option.Name + ",";
                                    }
                                } else
                                {
                                    formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                            option.Quantity, option.Name, ""/*option.Price*/);
                                }
                            }
                        }

                        if (choiceNames.Length > 0)
                        {
                            choiceNames = choiceNames.Remove(choiceNames.Length - 1);
                        }

                        if (choiceNames.Trim().Length > 0)
                        {
                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                choiceQuantity + " X", choiceTitle + "(" + choiceNames + ")", /*choicePrice*/"");
                        }
                        else
                        {
                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                choiceQuantity + " X", choiceTitle, /*choicePrice*/"");
                        }
                        formatted += choiceOPs;
                        continue;
                    }

                    var OPRef = "";
                    if (item.Options.Count > 0)
                    {
                        foreach (var option in item.Options)
                        {
                            if (option != null && option.Ref != null && option.Ref.StartsWith("OP", StringComparison.OrdinalIgnoreCase))
                            {
                                OPRef += option.Name + ",";
                            }
                        }
                    }

                    if (OPRef.Length > 0)
                    {
                        OPRef = OPRef.Remove(OPRef.Length - 1);
                    }

                    formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div>{0} {1}</div><div>{2}</div></div>",
                        (int)(float.Parse(item.Quantity)), item.ProductName, item.Subtotal.Split(" ")[0]);

                    if (item.Options.Count > 0)
                    {
                        var plusOptions = "";
                        var lastSideName = "";
                        var sideStrings = "";
                        foreach (var option in item.Options)
                        {
                            if (option != null && option.Ref != null && option.Ref.StartsWith("OP", StringComparison.OrdinalIgnoreCase))
                            {
                                formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                    option.Ref.Split("-")[1].Trim() + ":", option.Name, ""/*option.Price*/);
                                continue;
                            }

                            if (option.OptionListName != null && option.OptionListName.Contains("-Topping", StringComparison.OrdinalIgnoreCase) && !option.OptionListName.StartsWith("Remove", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            if (option != null && option.Ref != null && !option.Ref.StartsWith("OP", StringComparison.OrdinalIgnoreCase))
                            {
                                
                                if (option.OptionListName != null && option.OptionListName.StartsWith("Side", StringComparison.OrdinalIgnoreCase)) {
                                    if (plusOptions.Length > 0)
                                    {
                                        plusOptions = plusOptions.Remove(plusOptions.Length - 1);
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                                        plusOptions, "", "");
                                        plusOptions = "";
                                    }
                                    var sideName = option.OptionListName.Split("-")[0];
                                    if (option.Ref != null && option.Ref.Contains("TOP", StringComparison.OrdinalIgnoreCase)) {
                                        if (lastSideName == sideName)
                                        {
                                            sideStrings += "+" + option.Name + ",";
                                        } else
                                        {
                                            if (lastSideName == "")
                                            {
                                                lastSideName = sideName;
                                                sideStrings = "+" + option.Name + " ,";
                                            } else
                                            {
                                                if (sideStrings.Length > 0)
                                                {
                                                    sideStrings = sideStrings.Remove(sideStrings.Length - 1);
                                                    formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div></div>",
                                                        lastSideName + ":", sideStrings);
                                                }

                                                lastSideName = sideName;
                                                sideStrings = "+" + option.Name + " ,";
                                            }
                                        }
                                    } else
                                    {
                                        if (sideStrings.Length > 0)
                                        {
                                            sideStrings = sideStrings.Remove(sideStrings.Length - 1);
                                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div></div>",
                                                lastSideName + ":", sideStrings);
                                        }
                                        
                                        lastSideName = "";
                                        sideStrings = "";
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div></div>",
                                            sideName + ":", option.Name);
                                    }
                                } else
                                {
                                    if (sideStrings.Length > 0)
                                    {
                                        sideStrings = sideStrings.Remove(sideStrings.Length - 1);
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div></div>",
                                            lastSideName + ":", sideStrings);
                                    }
                                    lastSideName = "";
                                    sideStrings = "";

                                    if ((option.OptionListName.StartsWith("Extra Topping", StringComparison.OrdinalIgnoreCase)) || (option.OptionListName.StartsWith("Topping", StringComparison.OrdinalIgnoreCase) && option.OptionListName.EndsWith("HNH", StringComparison.OrdinalIgnoreCase))
                                        || (option.Ref != null && option.Ref.Contains("TOP", StringComparison.OrdinalIgnoreCase) && !option.OptionListName.StartsWith("Remove", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        plusOptions += "+" + option.Name + ",";
                                    } else if (option.OptionListName.StartsWith("Remove", StringComparison.OrdinalIgnoreCase) && option.Ref.Contains("TOP", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (plusOptions.Length > 0)
                                        {
                                            plusOptions = plusOptions.Remove(plusOptions.Length - 1);
                                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                                            plusOptions, "", "");
                                            plusOptions = "";
                                        }
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                            "-", option.Name, ""/*option.Price*/);
                                    } else 
                                    {
                                        if (plusOptions.Length > 0)
                                        {
                                            plusOptions = plusOptions.Remove(plusOptions.Length - 1);
                                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                                            plusOptions, "", "");
                                            plusOptions = "";
                                        }
                                        formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                            option.Quantity, option.Name, ""/*option.Price*/);
                                    }
                                    
                                }
                            }

                            /*if (optionDict.ContainsKey(option.OptionListName)) {
                                string? value;
                                if (optionDict.TryGetValue(option.OptionListName, out value)) {
                                    value += "," + option.Name;
                                    optionDict.Remove(option.OptionListName);
                                    optionDict.Add(option.OptionListName, value);
                                }

                                
                            } else {
                                optionDict.Add(option.OptionListName, option.Name);
                            }

                            if (optionprice.ContainsKey(option.OptionListName))
                            {
                                float fPrice;
                                if (optionprice.TryGetValue(option.OptionListName, out fPrice))
                                {
                                    float fNewPrice;
                                    if (float.TryParse(option.Price.Split(" ")[0], out fNewPrice))
                                    {
                                        fPrice += fNewPrice;
                                        optionprice.Remove(option.OptionListName);
                                        optionprice.Add(option.OptionListName, fPrice);
                                    }
                                } else
                                {
                                    string price = option.Price;
                                    if (price != null)
                                    {
                                        if (float.TryParse(price.Split(" ")[0], out fPrice))
                                        {
                                            optionprice.Remove(option.OptionListName);
                                            optionprice.Add(option.OptionListName, fPrice);
                                        }

                                        string tmpCurrency = price.Split(" ")[1];
                                        if (tmpCurrency != null) {
                                            currency = tmpCurrency;
                                        }
                                    }
                                }
                            } else
                            {
                                string price = option.Price;
                                if (price != null)
                                {
                                    float fPrice;
                                    if (float.TryParse(price.Split(" ")[0], out fPrice))
                                    {
                                        optionprice.Add(option.OptionListName, fPrice);
                                    }
                                }
                            }

                            if (optionQuantity.ContainsKey(option.OptionListName))
                            {
                                int curQuantity;
                                if (optionQuantity.TryGetValue(option.OptionListName, out curQuantity))
                                {
                                    curQuantity += option.Quantity;
                                    optionQuantity.Remove(option.OptionListName);
                                    optionQuantity.Add(option.OptionListName, curQuantity);
                                }
                            } else
                            {
                                optionQuantity.Add(option.OptionListName, option.Quantity);
                            }*/
                        }
                        
                        /*foreach(KeyValuePair<string,string> entry in optionDict) {
                            int quantity = optionQuantity[entry.Key];
                            float price = optionprice[entry.Key];

                            formatted += string.Format("<div style=\"display: flex; justify-content: space-between;\"><div style=\"margin-left:20px;\">{0} {1}</div><div style=\"white-space:nowrap;\">{2}</div></div>",
                                quantity, entry.Key + " : " + entry.Value, price + " " + currency);
                        }*/
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

            if (model.NewStateObj.Deals.Name != null && model.NewStateObj.Deals.Name != "") {
                formatted += string.Format("<br/><div><b>*** DEALS ***</b></div>");
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