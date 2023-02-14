using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TGFPIZZAHUB.Models
{
    public class HubRiseModel
    {
        public class Customer
        {
            [JsonProperty("id")]
            [JsonPropertyName("id")]
            public object Id { get; set; }

            [JsonProperty("customer_list_id")]
            [JsonPropertyName("customer_list_id")]
            public object CustomerListId { get; set; }

            [JsonProperty("anonymised")]
            [JsonPropertyName("anonymised")]
            public bool Anonymised { get; set; }

            [JsonProperty("private_ref")]
            [JsonPropertyName("private_ref")]
            public object PrivateRef { get; set; }

            [JsonProperty("email")]
            [JsonPropertyName("email")]
            public object Email { get; set; }

            [JsonProperty("first_name")]
            [JsonPropertyName("first_name")]
            public string FirstName { get; set; }

            [JsonProperty("last_name")]
            [JsonPropertyName("last_name")]
            public string LastName { get; set; }

            [JsonProperty("gender")]
            [JsonPropertyName("gender")]
            public object Gender { get; set; }

            [JsonProperty("birth_date")]
            [JsonPropertyName("birth_date")]
            public object BirthDate { get; set; }

            [JsonProperty("company_name")]
            [JsonPropertyName("company_name")]
            public object CompanyName { get; set; }

            [JsonProperty("phone")]
            [JsonPropertyName("phone")]
            public string? Phone { get; set; }

            [JsonProperty("address_1")]
            [JsonPropertyName("address_1")]
            public object Address1 { get; set; }

            [JsonProperty("address_2")]
            [JsonPropertyName("address_2")]
            public object Address2 { get; set; }

            [JsonProperty("postal_code")]
            [JsonPropertyName("postal_code")]
            public object PostalCode { get; set; }

            [JsonProperty("city")]
            [JsonPropertyName("city")]
            public object City { get; set; }

            [JsonProperty("state")]
            [JsonPropertyName("state")]
            public object State { get; set; }

            [JsonProperty("country")]
            [JsonPropertyName("country")]
            public object Country { get; set; }

            [JsonProperty("latitude")]
            [JsonPropertyName("latitude")]
            public object Latitude { get; set; }

            [JsonProperty("longitude")]
            [JsonPropertyName("longitude")]
            public object Longitude { get; set; }

            [JsonProperty("delivery_notes")]
            [JsonPropertyName("delivery_notes")]
            public object DeliveryNotes { get; set; }

            [JsonProperty("sms_marketing")]
            [JsonPropertyName("sms_marketing")]
            public bool SmsMarketing { get; set; }

            [JsonProperty("email_marketing")]
            [JsonPropertyName("email_marketing")]
            public bool EmailMarketing { get; set; }

            [JsonProperty("nb_orders")]
            [JsonPropertyName("nb_orders")]
            public int NbOrders { get; set; }

            [JsonProperty("order_total")]
            [JsonPropertyName("order_total")]
            public string OrderTotal { get; set; }

            [JsonProperty("first_order_date")]
            [JsonPropertyName("first_order_date")]
            public DateTime FirstOrderDate { get; set; }

            [JsonProperty("last_order_date")]
            [JsonPropertyName("last_order_date")]
            public DateTime LastOrderDate { get; set; }

            [JsonProperty("loyalty_cards")]
            [JsonPropertyName("loyalty_cards")]
            public List<object> LoyaltyCards { get; set; }

            [JsonProperty("custom_fields")]
            [JsonPropertyName("custom_fields")]
            public CustomFields CustomFields { get; set; }
        }

        public class CustomFields
        {
        }

        public class OrderDeals
        {
            [JsonProperty("name")]
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonProperty("ref")]
            [JsonPropertyName("ref")]
            public string? Ref { get; set; }
        }

        public class Discount
        {
            [JsonProperty("id")]
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonProperty("private_ref")]
            [JsonPropertyName("private_ref")]
            public object PrivateRef { get; set; }

            [JsonProperty("name")]
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonProperty("ref")]
            [JsonPropertyName("ref")]
            public string Ref { get; set; }

            [JsonProperty("pricing_effect")]
            [JsonPropertyName("pricing_effect")]
            public string PricingEffect { get; set; }

            [JsonProperty("pricing_value")]
            [JsonPropertyName("pricing_value")]
            public string PricingValue { get; set; }

            [JsonProperty("price_off")]
            [JsonPropertyName("price_off")]
            public string PriceOff { get; set; }

            [JsonProperty("deleted")]
            [JsonPropertyName("deleted")]
            public bool Deleted { get; set; }
        }

        public class Item
        {
            [JsonProperty("id")]
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonProperty("private_ref")]
            [JsonPropertyName("private_ref")]
            public object PrivateRef { get; set; }

            [JsonProperty("product_name")]
            [JsonPropertyName("product_name")]
            public string ProductName { get; set; }

            [JsonProperty("sku_name")]
            [JsonPropertyName("sku_name")]
            public object SkuName { get; set; }

            [JsonProperty("sku_ref")]
            [JsonPropertyName("sku_ref")]
            public string SkuRef { get; set; }

            [JsonProperty("price")]
            [JsonPropertyName("price")]
            public string Price { get; set; }

            [JsonProperty("quantity")]
            [JsonPropertyName("quantity")]
            public string Quantity { get; set; }

            [JsonProperty("subtotal")]
            [JsonPropertyName("subtotal")]
            public string Subtotal { get; set; }

            [JsonProperty("tax_rate")]
            [JsonPropertyName("tax_rate")]
            public object TaxRate { get; set; }

            [JsonProperty("customer_notes")]
            [JsonPropertyName("customer_notes")]
            public object CustomerNotes { get; set; }

            [JsonProperty("points_earned")]
            [JsonPropertyName("points_earned")]
            public object PointsEarned { get; set; }

            [JsonProperty("points_used")]
            [JsonPropertyName("points_used")]
            public object PointsUsed { get; set; }

            [JsonProperty("options")]
            [JsonPropertyName("options")]
            public List<Option> Options { get; set; }

            [JsonProperty("deleted")]
            [JsonPropertyName("deleted")]
            public bool Deleted { get; set; }
        }

        public class NewState
        {
            [JsonProperty("id")]
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonProperty("location_id")]
            [JsonPropertyName("location_id")]
            public string LocationId { get; set; }

            [JsonProperty("private_ref")]
            [JsonPropertyName("private_ref")]
            public object PrivateRef { get; set; }

            [JsonProperty("status")]
            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonProperty("service_type")]
            [JsonPropertyName("service_type")]
            public string ServiceType { get; set; }

            [JsonProperty("service_type_ref")]
            [JsonPropertyName("service_type_ref")]
            public string ServiceTypeRef { get; set; }

            [JsonProperty("created_at")]
            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("created_by")]
            [JsonPropertyName("created_by")]
            public string CreatedBy { get; set; }

            [JsonProperty("expected_time")]
            [JsonPropertyName("expected_time")]
            public DateTime ExpectedTime { get; set; }

            [JsonProperty("confirmed_time")]
            [JsonPropertyName("confirmed_time")]
            public object ConfirmedTime { get; set; }

            [JsonProperty("customer_notes")]
            [JsonPropertyName("customer_notes")]
            public string CustomerNotes { get; set; }

            [JsonProperty("seller_notes")]
            [JsonPropertyName("seller_notes")]
            public object SellerNotes { get; set; }

            [JsonProperty("collection_code")]
            [JsonPropertyName("collection_code")]
            public string CollectionCode { get; set; }

            [JsonProperty("coupon_codes")]
            [JsonPropertyName("coupon_codes")]
            public List<object> CouponCodes { get; set; }

            [JsonProperty("total")]
            [JsonPropertyName("total")]
            public string Total { get; set; }

            [JsonProperty("total_discrepancy")]
            [JsonPropertyName("total_discrepancy")]
            public object TotalDiscrepancy { get; set; }

            [JsonProperty("payment_discrepancy")]
            [JsonPropertyName("payment_discrepancy")]
            public string PaymentDiscrepancy { get; set; }

            [JsonProperty("items")]
            [JsonPropertyName("items")]
            public List<Item> Items { get; set; }

            [JsonProperty("deals")]
            [JsonPropertyName("deals")]
            public OrderDeals Deals { get; set; }

            [JsonProperty("discounts")]
            [JsonPropertyName("discounts")]
            public List<Discount> Discounts { get; set; }

            [JsonProperty("charges")]
            [JsonPropertyName("charges")]
            public List<object> Charges { get; set; }

            [JsonProperty("payments")]
            [JsonPropertyName("payments")]
            public List<Payment> Payments { get; set; }

            [JsonProperty("customer")]
            [JsonPropertyName("customer")]
            public Customer Customer { get; set; }

            [JsonProperty("loyalty_operations")]
            [JsonPropertyName("loyalty_operations")]
            public List<object> LoyaltyOperations { get; set; }

            [JsonProperty("custom_fields")]
            [JsonPropertyName("custom_fields")]
            public CustomFields CustomFields { get; set; }
        }

        public class Option
        {
            [JsonProperty("option_list_name")]
            [JsonPropertyName("option_list_name")]
            public string OptionListName { get; set; }

            [JsonProperty("name")]
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonProperty("ref")]
            [JsonPropertyName("ref")]
            public string Ref { get; set; }

            [JsonProperty("price")]
            [JsonPropertyName("price")]
            public string Price { get; set; }

            [JsonProperty("quantity")]
            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }
        }

        public class Payment
        {
            [JsonProperty("id")]
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonProperty("private_ref")]
            [JsonPropertyName("private_ref")]
            public object PrivateRef { get; set; }

            [JsonProperty("type")]
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonProperty("ref")]
            [JsonPropertyName("ref")]
            public string Ref { get; set; }

            [JsonProperty("amount")]
            [JsonPropertyName("amount")]
            public string Amount { get; set; }

            [JsonProperty("info")]
            [JsonPropertyName("info")]
            public object Info { get; set; }

            [JsonProperty("deleted")]
            [JsonPropertyName("deleted")]
            public bool Deleted { get; set; }
        }

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("resource_type")]
        [JsonPropertyName("resource_type")]
        public string ResourceType { get; set; }

        [JsonProperty("event_type")]
        [JsonPropertyName("event_type")]
        public string EventType { get; set; }

        [JsonProperty("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("order_id")]
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("resource_id")]
        [JsonPropertyName("resource_id")]
        public string ResourceId { get; set; }

        [JsonProperty("location_id")]
        [JsonPropertyName("location_id")]
        public string LocationId { get; set; }

        [JsonProperty("new_state")]
        [JsonPropertyName("new_state")]
        public NewState NewStateObj { get; set; }

        [JsonProperty("user_id")]
        [JsonPropertyName("user_id")]
        public object UserId { get; set; }

        [JsonProperty("account_id")]
        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("app_instance_id")]
        [JsonPropertyName("app_instance_id")]
        public object AppInstanceId { get; set; }
    }
}
