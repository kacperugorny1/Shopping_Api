namespace Shopping.Dtos{

    public partial class PaymentForm{
        public int WhoId  {get; set;}
        public int ToWhoId {get; set;}
        public decimal Cost {get; set;}
    }
}