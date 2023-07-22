namespace Shopping.Dtos{

    public partial class RegistrationForm{
        public string Email {get;set;} = "";
        public string Password {get;set;} = "";
        public string PasswordConfirm {get;set;} = "";
        public string Name {get;set;} = "";
    }
}