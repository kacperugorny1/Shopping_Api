
namespace Shopping.Models{
    public partial class Payment{
        int PayId {get; set;}
        int WhoId  {get; set;}
        int ToWhoId {get; set;}
        decimal Cost {get; set;}
        DateTime DateAdded {get; set;}
    }
}