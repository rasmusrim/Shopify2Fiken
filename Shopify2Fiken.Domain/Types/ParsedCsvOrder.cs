using System.Security.Cryptography;
using System.Text;
using Fiken.Sdk;

namespace Shopify2Fiken.Domain.Types;

public class ParsedCsvOrder : CsvOrder
{
    public List<LineItem> LineItems { get; } = new List<LineItem>();

    public void AddLineItem(CsvOrder orderWithLineItem)
    {
        LineItems.Add(
            new LineItem
            {
                CreatedAt = orderWithLineItem.CreatedAt,
                LineitemQuantity = orderWithLineItem.LineitemQuantity,
                LineitemName = orderWithLineItem.LineitemName,
                LineitemPrice = orderWithLineItem.LineitemPrice,
                LineitemCompareAtPrice = orderWithLineItem.LineitemCompareAtPrice,
                LineitemSku = orderWithLineItem.LineitemSku,
                LineitemRequiresShipping = orderWithLineItem.LineitemRequiresShipping,
                LineitemTaxable = orderWithLineItem.LineitemTaxable,
                LineitemFulfillmentStatus = orderWithLineItem.LineitemFulfillmentStatus
            }
        );
    }

    public string LineItemsFormatted
    {
        get
        {
            if (LineItems == null || !LineItems.Any())
                return string.Empty;

            return string.Join(
                "\n",
                LineItems.Select(li =>
                    $"{li.LineitemName} x{li.LineitemQuantity} @ {li.LineitemPrice:C}"
                )
            );
        }
    }

    public string Hash
    {
        get
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hash.ComputeHash(
                    Encoding.UTF8.GetBytes($"{CreatedAt} {Total:C} {LineItemsFormatted}")
                );

                // Create a new StringBuilder to collect the bytes and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
