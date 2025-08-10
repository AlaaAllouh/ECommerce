public class OrderViewModel
{
    // معلومات المستخدم
    public string Name { get; set; }
    public string Email { get; set; }
    public string Aderss { get; set; }
    public string Mobile { get; set; }

    // معلومات الدفع
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string Expiry { get; set; }
    public string CVV { get; set; }
}
