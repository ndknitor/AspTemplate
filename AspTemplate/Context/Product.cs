using System;
using System.Collections.Generic;

namespace AspTemplate.Context;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductDescription { get; set; }

    public decimal? ProductPrice { get; set; }

    public bool? InStock { get; set; }

    public string Sku { get; set; }

    public string Category { get; set; }

    public string Brand { get; set; }

    public string ProductWeight { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? ProductAvailabilityDate { get; set; }

    public decimal? TaxRate { get; set; }
}
