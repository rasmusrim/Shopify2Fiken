using System.Globalization;
using System.IO;
using System.Net.Http;
using CsvHelper;
using Shopify2Fiken.Domain.Types;

namespace GUI.Helpers;

public class OrdersCsvReader
{
    public Dictionary<string, ParsedCsvOrder> Read(string filename)
    {
        using var reader = new StreamReader(filename);

        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var httpClient = new HttpClient();

        var records = csv.GetRecords<CsvOrder>();
        
        var orders = records.Aggregate(new Dictionary<string, ParsedCsvOrder>(), (acc, current) =>
        {
            var key = current.Name;
            if (!acc.ContainsKey(key))
            {
                var parsedOrder = current.ToParsedCsvOrder();
                acc[key] = parsedOrder;
            }
            else
            {
                
                acc[key].AddLineItem(current);
            }
            return acc;
        });        
        
        return orders;

    }

}
