namespace SoapApi.Infrastructure.Entities;

public class BookEntity {
    public Guid Id {get; set;}
    public string Title {get; set;} = null;
    public string Author {get; set;} = null;
    public string Publisher {get; set;} = null;
    public DateTime PublishedDate {get; set;}
}