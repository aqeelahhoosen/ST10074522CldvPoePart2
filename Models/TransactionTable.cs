namespace cldvPOE.Models
{
    public class TransactionTable
    {
        public int TransactionID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public DateTime Date {  get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
