using GUI.Helpers;
using Shopify2Fiken.Domain.Types;

namespace Fiken.Sdk;

public class FikenCommunicator
{
    private readonly FikenConfig _config;
    private readonly FikenClient _api;
    private ICollection<Product>? _products;

    public FikenCommunicator(FikenConfig config)
    {
        _config = config;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + config.FikenApiKey);

        _api = new FikenClient(httpClient);
    }

    private Task CreateCustomer(Customer customer)
    {
        var contact = new Contact
        {
            Name = customer.Name,
            Email = customer.Email,
            Address = new Address
            {
                StreetAddress = customer.Address1,
                StreetAddressLine2 = customer.Address2,
                City = customer.City,
                Country = customer.Country,
                PostCode = customer.Zip,
            },
            PhoneNumber = customer.Phone,
            Customer = true,
            Currency = "NOK",
        };

        return _api.CreateContactAsync(contact, _config.CompanySlug);
    }

    public Task<ICollection<InvoiceResult>> GetSalesForContact(Contact contact)
    {
        return _api.GetInvoicesAsync(
            null,
            999999,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            contact.ContactId,
            null,
            null,
            null,
            null,
            _config.CompanySlug
        );
    }

    public Task<ICollection<Contact>> GetCustomer(string email)
    {
        return _api.GetContactsAsync(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            email,
            null,
            null,
            null,
            null,
            null,
            null,
            _config.CompanySlug
        );
    }

    public async Task AddOrders(List<ParsedCsvOrder> orders)
    {
        foreach (var order in orders)
        {
            var customers = await GetCustomer(order.Email);

            if (customers.Count == 0)
            {
                await CreateCustomer(order.ToCustomer());
                customers = await GetCustomer(order.Email);
            }

            var customer = customers.First();

            var existingSales = await GetSalesForContact(customer);

            if (!OrderAlreadyExists(existingSales, order))
            {
                await CreateOrder(customer, order);
            }
        }
    }

    private async Task SetSaleToPaid(InvoiceResult invoice, string account)
    {
        var sale = await _api.GetSaleAsync(_config.CompanySlug, invoice.Sale.SaleId);

        await _api.CreateSalePaymentAsync(
            new Payment
            {
                Account = account,
                Amount = invoice.Gross,
                Date = invoice.CreatedDate,
                Currency = "NOK",
                Fee = 0
            },
            _config.CompanySlug,
            sale.SaleId
        );
    }

    private async Task CreateOrder(Contact customer, ParsedCsvOrder order)
    {
        var invoiceRequest = await CreateInvoiceRequest(order, customer);

        await _api.CreateInvoiceAsync(invoiceRequest, _config.CompanySlug);

        var existingSales = (await GetSalesForContact(customer))
            .ToList()
            .Where(s => s.AssociatedCreditNotes.Count == 0);
        var sale = existingSales.ToList().Find(s => s.OurReference == order.Hash);

        await SetSaleToPaid(sale, invoiceRequest.BankAccountCode);
    }

    private bool OrderAlreadyExists(ICollection<InvoiceResult> existingSales, ParsedCsvOrder order)
    {
        return existingSales
            .Where(s => s.AssociatedCreditNotes.Count == 0)
            .Any(s => s.OurReference == order.Hash);
    }

    public async Task<InvoiceRequest> CreateInvoiceRequest(ParsedCsvOrder order, Contact customer)
    {
        var invoice = new InvoiceRequest
        {
            Uuid = Guid.NewGuid().ToString(),
            IssueDate = order.CreatedAt,
            DueDate = order.CreatedAt.AddDays(1),

            BankAccountCode = GetBankAccountCode(order.PaymentMethod),
            CustomerId = customer.ContactId.Value,
            ContactPersonId = null,
            Currency = "NOK",
            Cash = false,
            OurReference = order.Hash
        };

        invoice.Lines = await CovertToInvoiceRequestLines(order);

        return invoice;
    }

    private async Task<ICollection<InvoiceLineRequest>> CovertToInvoiceRequestLines(
        ParsedCsvOrder order
    )
    {
        var lines = new List<InvoiceLineRequest>();

        foreach (var li in order.LineItems)
        {
            var product = (await GetProduct(li.LineitemName));

            lines.Add(
                new InvoiceLineRequest
                {
                    Quantity = li.LineitemQuantity,
                    ProductId = product.ProductId,
                    VatType = product.VatType
                }
            );
        }

        var shippingProduct = await GetProduct("Frakt");
        if (order.Shipping > 0)
        {
            lines.Add(
                new InvoiceLineRequest
                {
                    ProductId = shippingProduct.ProductId,
                    Quantity = 1,
                    VatType = shippingProduct.VatType,
                    Gross = (long)Math.Round((order.Shipping * 100 ?? 0))
                }
            );
        }

        return lines;
    }

    private static string GetBankAccountCode(string bankAccountCode)
    {
        const string stripeAccount = "1960:10004";
        const string vippsAccount = "1960:10003";

        string normalizedCode = bankAccountCode.ToLower().Trim();

        if (normalizedCode.Contains("stripe"))
        {
            return stripeAccount;
        }

        if (normalizedCode.Contains("vipps"))
        {
            return vippsAccount;
        }

        throw new ArgumentException("Unknown payment method: " + bankAccountCode);
    }

    private async Task<Product> GetProduct(string productName)
    {
        var products = await GetProducts();

        var product = products.MinBy(p => LevenshteinDistance(p.Name, productName));

        if (product == null)
        {
            throw new InvalidOperationException("Product not found: " + productName);
        }

        return product;
    }

    private int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a))
        {
            return string.IsNullOrEmpty(b) ? 0 : b.Length;
        }

        if (string.IsNullOrEmpty(b))
        {
            return a.Length;
        }

        int n = a.Length;
        int m = b.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[n, m];
    }

    private async Task<ICollection<Product>> GetProducts()
    {
        if (_products == null)
        {
            _products = await _api.GetProductsAsync(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                true,
                _config.CompanySlug
            );
        }

        return _products;
    }
}
