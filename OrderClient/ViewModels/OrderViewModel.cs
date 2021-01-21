using OrderClient.Model;
using Prism.Mvvm;
using System;

namespace OrderClient.ViewModels
{
  public class OrderViewModel : BindableBase
  {
    public OrderViewModel(Order order, bool isNew = false)
    {
      IsNew = isNew;
      Model = order;
    }
    public Order Model { get; private set; }

    public int Id
    {
      get => Model.Id;
    }

    public string Name
    {
      get => Model.Name;      
    }

    public string Address
    {
      get => Model.Address;
    }

    public int OrderNumber
    {
      get => Model.OrderNumber;
    }

    public bool IsShipped
    {
      get => Model.IsShipped;
      set
      {
        Model.IsShipped = value;
        RaisePropertyChanged();
      }
    }

    public bool IsTransfered
    {
      get => Model.IsTransfered;
      set
      {
        Model.IsTransfered = value;
        RaisePropertyChanged();
      }
    }

    private bool _isNew;
    public bool IsNew
    {
      get => _isNew;
      set
      {
        SetProperty(ref _isNew, value);
      }
    }

    public DateTime OrderDate
    {
      get => Model.OrderDate;
    }

    public Guid FileIdOrderSheet
    {
      get => Model.FileIdOrderSheet;
    }

    public Guid? FileIdDeliveryNote
    {
      get => Model.FileIdDeliveryNote;
    }

    public bool HasDeliveryNote
    {
      get => Model.FileIdDeliveryNote.HasValue;
    }
  }
}
