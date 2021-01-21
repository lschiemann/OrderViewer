using OrderClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OrderClient
{
  public interface IOrderAPIService
  {
    Task<IEnumerable<Order>> GetOpenOrders(DateTime receiveDate);
    Task<bool> DeliverOrder(int orderId);
    Task<bool> TransferOrder(int orderId);
    Task<Stream> GetFile(Guid id);
  }
}