using System.ServiceModel;
using SoapApi.Dtos;

namespace SoapApi.Contracts;

[ServiceContract]
public interface IBookContract {
 
    [OperationContract]
    public Task<bool> DeleteBookById(Guid bookId, CancellationToken cancellationToken);
}