namespace SoapApi.Models;

public class UserModel {
    public Guid Id {get; set;}
    public string Email {get; set;} = null;
    public string FirstName {get; set;} = null;
    public string LastName {get; set;} = null;
    public DateTime BirthDate {get; set;}
}