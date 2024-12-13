namespace Web.Models
{
    public class CartItemModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Picture {  get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice
        {
            get { return Quantity * Price; }
        }
        public CartItemModel() //Khi không thêm giỏ hàng thì giỏ hàng vẫn được tạo nhưng rỗng
        {

        }
        public CartItemModel(ProductModel product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Picture = product.Picture;
            Price = product.Price;
            Quantity = 1;
        }
    }
}
