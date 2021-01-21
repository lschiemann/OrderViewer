using OrderClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrderClient
{
  public class OrderAPIService : IOrderAPIService
  {
    private const string _baseAddress = "https://localhost:44359";

    public async Task<bool> DeliverOrder(int orderId)
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(_baseAddress);

        //client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

        var response = await client.PostAsync("order/ship", new StringContent(orderId.ToString(), System.Text.Encoding.UTF8, "application/json")).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
      }
    }

    public async Task<bool> TransferOrder(int orderId)
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(_baseAddress);

        //client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

        var response = await client.PostAsync("order/transfer", new StringContent(orderId.ToString(), System.Text.Encoding.UTF8, "application/json")).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
      }
    }

    public async Task<IEnumerable<Order>> GetOpenOrders()
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(_baseAddress);

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

        var response = await client.GetAsync("order/openOrders").ConfigureAwait(false);

        var result = response.Content.ReadAsStringAsync().Result;
        var suceeded = response.IsSuccessStatusCode;
        response.Dispose();
        if (suceeded)
        {
          var orders = XElement.Parse(result).Elements("Order").Select(e =>
          {
            var hasDeliveryNote = Guid.TryParse(e.Element("FileIdDeliveryNote").Value, out Guid fileIdDeliveryNote);
            return new Order()
            {
              OrderDate = DateTime.Parse(e.Element("OrderDate").Value),
              OrderNumber = int.Parse(e.Element("OrderNumber").Value),
              Name = e.Element("Name").Value,
              Address = e.Element("Address").Value,
              IsShipped = bool.Parse(e.Element("IsShipped").Value),
              Id = int.Parse(e.Element("Id").Value),
              IsTransfered = bool.Parse(e.Element("IsTransfered").Value),
              FileIdOrderSheet = Guid.Parse(e.Element("FileIdOrderSheet").Value),
              FileIdDeliveryNote = hasDeliveryNote ? fileIdDeliveryNote: null
            };
          }).ToArray();
          return orders;
        }
        else
        {
          //todo: error handling
          return new List<Order>();
        }
      }
    }

    public async Task<Stream> GetFile(Guid id)
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(_baseAddress);

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));

        var response = await client.GetAsync("order/download?id=" + id.ToString()).ConfigureAwait(false);
        
        var result = response.Content.ReadAsStreamAsync().Result;
        var suceeded = response.IsSuccessStatusCode;
        
        //todo: this gets the original file name from header --> perhaps needed?
        IEnumerable<string> values;
        response.Content.Headers.TryGetValues("Content-Disposition", out values);
        
        if (suceeded)
        {
          return result;
        }
        else
        {
          //todo: error handling
          return Stream.Null;
        }
      }
    }
  }
}