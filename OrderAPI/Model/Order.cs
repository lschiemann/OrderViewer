using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OrderAPI.Model
{
  [DataContract]
  public class Order
  {
    [Key]
    [DataMember]
    public int Id { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Address { get; set; }
    [DataMember]
    public int OrderNumber { get; set; }
    [DataMember]
    public bool IsShipped { get; set; }
    [DataMember]
    public bool IsTransfered { get; set; }
    [DataMember]
    public DateTime OrderDate { get; set; }
    [DataMember]
    public Guid FileIdOrderSheet { get; set; }
    [DataMember]
    public Guid? FileIdDeliveryNote { get; set; }
  }
}
