namespace Shopify2Fiken.Domain.Types;

public class LineItem
{
    public DateTime CreatedAt { get; set; }
    public int LineitemQuantity { get; set; }
    public string LineitemName { get; set; }
    public decimal LineitemPrice { get; set; }
    public decimal? LineitemCompareAtPrice { get; set; }
    public string LineitemSku { get; set; }
    public bool LineitemRequiresShipping { get; set; }
    public bool LineitemTaxable { get; set; }
    public string LineitemFulfillmentStatus { get; set; }

}
