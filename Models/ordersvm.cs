namespace ECommerce.Models
{
    public class ordersvm
    {
        public ordersvm()
        {
            orders = new List<Order>(); 
            ordersdetail = new List<OrderDetail>(); 
        }
        public List<Order> orders { get; set; } 
        public List<OrderDetail> ordersdetail { get; set; } 
    }
}
