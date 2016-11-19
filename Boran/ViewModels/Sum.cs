namespace Boran.ViewModels
{
    public class Sum
    {
        public int Date { get; set;}
        //public int OrderQuantity { get; set; }
        //public double Discount { get; set; }
        //public double UnitPrice { get; set; }
        //public Double UnitShippingCost { get; set; }
        public int _count { get; set; }
        public string Count { get; set; }
        public string ProductCategory { get; set; }
        public double? _totalPriceToCategory { get; set; }
        public string TotalPriceToCategory { get; set; }
        //   public double price  { get { return price; } set { price = getPrice(); } }
        // private double getPrice()
        // {
        //   return (OrderQuantity * (Convert.ToDouble(ShippingCost) - (Convert.ToDouble(ShippingCost) * Discount)));
        // }
    }
     

   
}