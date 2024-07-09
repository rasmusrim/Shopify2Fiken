using CsvHelper.Configuration.Attributes;
using Fiken.Sdk;

namespace Shopify2Fiken.Domain.Types;

public class CsvOrder
{
     [Name("Name")]
    public string Name { get; set; }

    [Name("Email")]
    public string Email { get; set; }

    [Name("Financial Status")]
    public string FinancialStatus { get; set; }

    [Name("Paid at")]
    public DateTime? PaidAt { get; set; }

    [Name("Fulfillment Status")]
    public string FulfillmentStatus { get; set; }

    [Name("Fulfilled at")]
    public DateTime? FulfilledAt { get; set; }

    [Name("Accepts Marketing")]
    [BooleanTrueValues("yes")]
    [BooleanFalseValues("no", "")]
    public bool AcceptsMarketing { get; set; }

    [Name("Currency")]
    public string Currency { get; set; }

    [Name("Subtotal")]
    public decimal? Subtotal { get; set; }

    [Name("Shipping")]
    public decimal? Shipping { get; set; }

    [Name("Taxes")]
    public decimal? Taxes { get; set; }

    [Name("Total")]
    public decimal? Total { get; set; }

    [Name("Discount Code")]
    public string DiscountCode { get; set; }

    [Name("Discount Amount")]
    public decimal? DiscountAmount { get; set; }

    [Name("Shipping Method")]
    public string ShippingMethod { get; set; }

    [Name("Created at")]
    public DateTime CreatedAt { get; set; }

    [Name("Lineitem quantity")]
    public int LineitemQuantity { get; set; }

    [Name("Lineitem name")]
    public string LineitemName { get; set; }

    [Name("Lineitem price")]
    public decimal LineitemPrice { get; set; }

    [Name("Lineitem compare at price")]
    public decimal? LineitemCompareAtPrice { get; set; }

    [Name("Lineitem sku")]
    public string LineitemSku { get; set; }

    [Name("Lineitem requires shipping")]
    public bool LineitemRequiresShipping { get; set; }

    [Name("Lineitem taxable")]
    public bool LineitemTaxable { get; set; }

    [Name("Lineitem fulfillment status")]
    public string LineitemFulfillmentStatus { get; set; }

    [Name("Billing Name")]
    public string BillingName { get; set; }

    [Name("Billing Street")]
    public string BillingStreet { get; set; }

    [Name("Billing Address1")]
    public string BillingAddress1 { get; set; }

    [Name("Billing Address2")]
    public string BillingAddress2 { get; set; }

    [Name("Billing Company")]
    public string BillingCompany { get; set; }

    [Name("Billing City")]
    public string BillingCity { get; set; }

    [Name("Billing Zip")]
    public string BillingZip { get; set; }

    [Name("Billing Province")]
    public string BillingProvince { get; set; }

    [Name("Billing Country")]
    public string BillingCountry { get; set; }

    [Name("Billing Phone")]
    public string BillingPhone { get; set; }

    [Name("Shipping Name")]
    public string ShippingName { get; set; }

    [Name("Shipping Street")]
    public string ShippingStreet { get; set; }

    [Name("Shipping Address1")]
    public string ShippingAddress1 { get; set; }

    [Name("Shipping Address2")]
    public string ShippingAddress2 { get; set; }

    [Name("Shipping Company")]
    public string ShippingCompany { get; set; }

    [Name("Shipping City")]
    public string ShippingCity { get; set; }

    [Name("Shipping Zip")]
    public string ShippingZip { get; set; }

    [Name("Shipping Province")]
    public string ShippingProvince { get; set; }

    [Name("Shipping Country")]
    public string ShippingCountry { get; set; }

    [Name("Shipping Phone")]
    public string ShippingPhone { get; set; }

    [Name("Notes")]
    public string Notes { get; set; }

    [Name("Note Attributes")]
    public string NoteAttributes { get; set; }

    [Name("Cancelled at")]
    public DateTime? CancelledAt { get; set; }

    [Name("Payment Method")]
    public string PaymentMethod { get; set; }

    [Name("Payment Reference")]
    public string PaymentReference { get; set; }

    [Name("Refunded Amount")]
    public decimal? RefundedAmount { get; set; }

    [Name("Vendor")]
    public string Vendor { get; set; }

    [Name("Outstanding Balance")]
    public decimal? OutstandingBalance { get; set; }

    [Name("Employee")]
    public string Employee { get; set; }

    [Name("Location")]
    public string Location { get; set; }

    [Name("Device ID")]
    public string DeviceId { get; set; }

    [Name("Id")]
    public string Id { get; set; }

    [Name("Tags")]
    public string Tags { get; set; }

    [Name("Risk Level")]
    public string RiskLevel { get; set; }

    [Name("Source")]
    public string Source { get; set; }

    [Name("Lineitem discount")]
    public decimal LineitemDiscount { get; set; }

    [Name("Tax 1 Name")]
    public string Tax1Name { get; set; }

    [Name("Tax 1 Value")]
    public decimal? Tax1Value { get; set; }

    [Name("Tax 2 Name")]
    public string Tax2Name { get; set; }

    [Name("Tax 2 Value")]
    public decimal? Tax2Value { get; set; }

    [Name("Tax 3 Name")]
    public string Tax3Name { get; set; }

    [Name("Tax 3 Value")]
    public decimal? Tax3Value { get; set; }

    [Name("Tax 4 Name")]
    public string Tax4Name { get; set; }

    [Name("Tax 4 Value")]
    public decimal? Tax4Value { get; set; }

    [Name("Tax 5 Name")]
    public string Tax5Name { get; set; }

    [Name("Tax 5 Value")]
    public decimal? Tax5Value { get; set; }

    [Name("Phone")]
    public string Phone { get; set; }

    [Name("Receipt Number")]
    public string ReceiptNumber { get; set; }

    [Name("Duties")]
    public decimal? Duties { get; set; }

    [Name("Billing Province Name")]
    public string BillingProvinceName { get; set; }

    [Name("Shipping Province Name")]
    public string ShippingProvinceName { get; set; }

    [Name("Payment ID")]
    public string PaymentId { get; set; }

    [Name("Payment Terms Name")]
    public string PaymentTermsName { get; set; }

    [Name("Next Payment Due At")]
    public DateTime? NextPaymentDueAt { get; set; }

    [Name("Payment References")]
    public string PaymentReferences { get; set; }

    public ParsedCsvOrder ToParsedCsvOrder()
    {

        var parsedCsvOrder = new ParsedCsvOrder();

        foreach (var propertyInfo in GetType().GetProperties())
        {
            var propertyInParsedCsv = parsedCsvOrder.GetType().GetProperty(propertyInfo.Name);
            propertyInParsedCsv.SetValue(parsedCsvOrder, propertyInfo.GetValue(this));
            
        }

        parsedCsvOrder.AddLineItem(this);
        
        return parsedCsvOrder;


    }

    public Customer ToCustomer()
    {
        return new Customer
        {
            Name = BillingName,
            Email = Email,
            Street = BillingStreet,
            Address1 = BillingAddress1,
            Address2 = BillingAddress2,
            Company = BillingCompany,
            City = BillingCity,
            Zip = BillingZip,
            Province = BillingProvince,
            Country = BillingCountry,
            Phone = BillingPhone

        };
    } 
}