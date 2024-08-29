namespace SoapApi.Infrastructure.Entities;

public class UserEntity {
    public Guid Id {get; set;}
    public string Email {get; set;} = null;
    public string FirstName {get; set;} = null;
    public string LastName {get; set;} = null;
    public DateTime Birthday {get; set;}
}