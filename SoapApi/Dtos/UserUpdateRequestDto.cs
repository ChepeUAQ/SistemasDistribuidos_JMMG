using System.Runtime.Serialization;

namespace SoapApi.Dtos;

[DataContract]
public class UserUpdateRequestDto {
    [DataMember]
    public Guid UserId {get; set;}
    [DataMember]
    public string FirstName {get; set;} = null;
    [DataMember]
    public string LastName {get; set;} = null;
    [DataMember]
    public DateTime BirthDate {get; set;}
}