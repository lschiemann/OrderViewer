using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Model
{
  public class OrderFile
  {
    [Key]
    public Guid Id { get; set; }
    public string FileName { get; set; }
  }
}
