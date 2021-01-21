using OrderClient.Model;
using Prism.Mvvm;
using System;

namespace OrderClient.ViewModels
{
  public class OrderViewModel : BindableBase
  {
    private readonly Order _order;
    private readonly DateTime _actualDate;
    public OrderViewModel(Order order, DateTime actualDate)
    {
      _actualDate = actualDate;
      _order = order;
    }
    
    public int Id
    {
      get => _order.Id;
    }

    public string Name
    {
      get => _order.Name;      
    }

    public string Address
    {
      get => _order.Address;
    }

    public int OrderNumber
    {
      get => _order.OrderNumber;
    }

    public bool IsShipped
    {
      get => _order.IsShipped;
      set
      {
        _order.IsShipped = value;
        RaisePropertyChanged();
      }
    }

    public bool IsTransfered
    {
      get => _order.IsTransfered;
      set
      {
        _order.IsTransfered = value;
        RaisePropertyChanged();
      }
    }

    public bool IsNew
    {
      get => _order.ReceivedDate == _actualDate;
    }

    public DateTime OrderDate
    {
      get => _order.OrderDate;
    }

    public Guid FileIdOrderSheet
    {
      get => _order.FileIdOrderSheet;
    }

    public Guid? FileIdDeliveryNote
    {
      get => _order.FileIdDeliveryNote;
    }

    public bool HasDeliveryNote
    {
      get => _order.FileIdDeliveryNote.HasValue;
    }
  }
}
