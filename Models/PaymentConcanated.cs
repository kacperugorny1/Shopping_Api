
namespace Shopping.Models{
    public partial class PaymentConcanated{
        public int PayId {get; set;}
        public int WhoId  {get; set;}
        public int ToWhoId {get; set;}
        public decimal Cost {get; set;}
    }
}