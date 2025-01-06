namespace Web.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public string CouponCode { get; set; }
        //public decimal Discount { get; set; } 
    }
}
