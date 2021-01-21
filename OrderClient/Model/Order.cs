using System;
using System.ComponentModel.DataAnnotations;

namespace OrderClient.Model
{
  public class Order
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int OrderNumber { get; set; }
    public bool IsShipped { get; set; }
    public bool IsTransfered { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid FileIdOrderSheet { get; set; }
    public Guid? FileIdDeliveryNote { get; set; }
  }
}
