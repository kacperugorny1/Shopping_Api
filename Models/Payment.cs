
namespace Shopping.Models{
    public partial class Payment{
        public int PayId {get; set;}
        public int WhoId  {get; set;}
        public int ToWhoId {get; set;}
        public decimal Cost {get; set;}
        public DateTime DateAdded {get; set;}
    }
}