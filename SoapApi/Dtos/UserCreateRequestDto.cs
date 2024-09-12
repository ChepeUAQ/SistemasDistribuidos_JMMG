using System.Runtime.Serialization;

namespace SoapApi.Dtos;

[DataContract]
public class UserCreateRequestDto {
    [DataMember]
    public string Email {get; set;} = null;
    [DataMember]
    public string FirstName {get; set;} = null;
    [DataMember]
    public string LastName {get; set;} = null;
    [DataMember]
    public DateTime BirthDate {get; set;}
}