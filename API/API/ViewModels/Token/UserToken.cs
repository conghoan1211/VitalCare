namespace API.ViewModels.Token
{
    public class UserToken
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
       // public string Email { get; set; } = string.Empty;
      //  public string PhoneNumber { get; set; } = string.Empty;
        public string RoleID { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

    }
}
