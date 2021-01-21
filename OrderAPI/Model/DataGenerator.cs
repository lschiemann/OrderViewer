using System;
using System.Collections.Generic;
using Faker;

namespace OrderAPI.Model
{
  public static class DataGenerator
  {
    public static (IEnumerable<Order> Orders, IEnumerable<OrderFile> Files) Create(int startId, int? amount = null)
    {
      amount = amount.HasValue ? amount : new Random().Next(0, 50);

      List <OrderFile> files = new List<OrderFile>();

      List<Order> orders = new List<Order>();
      for (var i = 0; i < amount; i++)
      {
        var isShipped = Faker.Boolean.Random();

        orders.Add(new Order()
        {
          Id = startId + i + 1,
          Name = Name.FullName(),
          Address = string.Format("{0} {1} {2}", Address.StreetAddress(), Address.ZipCode(), Address.UkCountry()),
          OrderNumber = startId + i + 1,
          OrderDate = Identification.DateOfBirth(),
          IsShipped = isShipped,
          IsTransfered = Faker.Boolean.Random(),
          FileIdOrderSheet = CreateNewFile(files),
          FileIdDeliveryNote = isShipped ? CreateNewFile(files) : null
        });
      }

      return (orders, files);
    }

    private static Guid CreateNewFile(List<OrderFile> files)
    {
      var guid = Guid.NewGuid();
      files.Add(new OrderFile()
      {
        Id = guid,
        FileName = "samplefile.pdf"
      });
      return guid;
    }
  }
}
