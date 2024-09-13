using System.Runtime.Serialization;

namespace RestApi.Infrastructure.Soap;

[DataContract]
public class UserResponseDto {
    [DataMember]
    public Guid UserId {get; set;}
    [DataMember]
    public string Email {get; set;} = null;
    [DataMember]
    public string FirstName {get; set;} = null;
    [DataMember]
    public string LastName {get; set;} = null;
    [DataMember]
    public DateTime BirthDate {get; set;}
}