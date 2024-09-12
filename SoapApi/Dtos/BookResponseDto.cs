using System.Runtime.Serialization;

namespace SoapApi.Dtos;

[DataContract]
public class BookResponseDto {
    [DataMember]
    public Guid BookId {get; set;}
    [DataMember]
    public string Title {get; set;} = null;
    [DataMember]
    public string Author {get; set;} = null;
    [DataMember]
    public string Publisher {get; set;} = null;
    [DataMember]
    public DateTime PublishedDate {get; set;}
}