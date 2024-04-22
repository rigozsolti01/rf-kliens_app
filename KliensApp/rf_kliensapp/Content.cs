using System;
using System.Collections.Generic;

namespace rf_kliensapp
{
    public class ProductListItem
    {
        public string ProductName { get; set; }
        public string Bvin { get; set; }
        public double Price { get; set; }
    }
    public class Root
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public List<Product> Products { get; set; }
    }
    public class ProductResponse // Consider a more suitable name
    {
        public Product Content { get; set; }
        public List<object> Errors { get; set; } // Or define a specific Error type
    }
    public class Product
    {
        public string Bvin { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public string ProductTypeId { get; set; }
        public List<CustomProperty> CustomProperties { get; set; }
        public double ListPrice { get; set; }
        public double SitePrice { get; set; }
        public string SitePriceOverrideText { get; set; }
        public double SiteCost { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public bool TaxExempt { get; set; }
        public int TaxSchedule { get; set; }
        public ShippingDetails ShippingDetails { get; set; }
        public int ShippingMode { get; set; }
        public int Status { get; set; }
        public string ImageFileSmall { get; set; }
        public string ImageFileSmallAlternateText { get; set; }
        public string ImageFileMedium { get; set; }
        public string ImageFileMediumAlternateText { get; set; }
        public DateTime CreationDateUtc { get; set; }
        public int MinimumQty { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ManufacturerId { get; set; }
        public string VendorId { get; set; }
        public bool GiftWrapAllowed { get; set; }
        public double GiftWrapPrice { get; set; }
        public string Keywords { get; set; }
        public string PreContentColumnId { get; set; }
        public string PostContentColumnId { get; set; }
        public string UrlSlug { get; set; }
        public int InventoryMode { get; set; }
        public bool IsAvailableForSale { get; set; }
        public bool Featured { get; set; }
        public List<Tab> Tabs { get; set; }
        public int StoreId { get; set; }
        public bool IsSearchable { get; set; }
        public int ShippingCharge{ get; set; }
    }

    public class CustomProperty
    {
        public string DeveloperId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ShippingDetails
    {
        public bool IsNonShipping { get; set; }
        public double ExtraShipFee { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ShippingSource { get; set; }
        public string ShippingSourceId { get; set; }
        public bool ShipSeparately { get; set; }
    }

    public class Tab
    {
        public string Bvin { get; set; }
        public string TabTitle { get; set; }
        public string HtmlData { get; set; }
        public int SortOrder { get; set; }
    }
}