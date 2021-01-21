using OrderClient.Model;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrderClient.ViewModels
{
  public class MainWindowViewModel : BindableBase
  {
    private readonly IOrderAPIService _orderAPIService;
    private readonly Context _context;
    public MainWindowViewModel(IOrderAPIService apiService, Context context)
    {
      _orderAPIService = apiService;
      _context = context;
      FilterString = string.Empty;
      GetOpenOrdersCommand = new DelegateCommand(GetOrders);
      ClearFilterCommand = new DelegateCommand(ClearFilter);
      DeliverOrderCommand = new DelegateCommand<OrderViewModel>(DeliverOrder);
      OpenOrderSheetCommand = new DelegateCommand<OrderViewModel>(OpenOrderSheet);
      OpenDeliveryNoteCommand = new DelegateCommand<OrderViewModel>(OpenDeliveryNote);
    }

    private string _filterString;
    public string FilterString
    {
      get => _filterString;
      set
      {
        SetProperty(ref _filterString, value);
        RaisePropertyChanged(nameof(Orders));
      }
    }

    private bool _isWorking;
    public bool IsWorking
    {
      get => _isWorking;
      set => SetProperty(ref _isWorking, value);
    }

    private Stream _document;
    public Stream Document
    {
      get => _document;
      set
      {
        SetProperty(ref _document, value);
        RaisePropertyChanged(nameof(HasDocument));
      }
    }

    public bool HasDocument
    {
      get => _document != null;
    }

    private IEnumerable<OrderViewModel> _orders;
    public IEnumerable<OrderViewModel> Orders
    {
      get => GetFilteredItems();
      set => SetProperty(ref _orders, value);
    }

    private IEnumerable<OrderViewModel> GetFilteredItems()
    {
      if (_orders == null)
      {
        return _orders;
      }
      var filterString = FilterString.ToLower();
      return _orders.Where(o => o.Name.ToLower().Contains(filterString) || o.Address.ToLower().Contains(filterString) || o.OrderNumber.ToString().Contains(filterString));
    }

    public DelegateCommand ClearFilterCommand { get; private set; }
    public DelegateCommand GetOpenOrdersCommand { get; private set; }
    public DelegateCommand<OrderViewModel> DeliverOrderCommand { get; private set; }
    public DelegateCommand<OrderViewModel> OpenOrderSheetCommand { get; private set; }
    public DelegateCommand<OrderViewModel> OpenDeliveryNoteCommand { get; private set; }

    private void ClearFilter()
    {
      FilterString = string.Empty;
    }

    private async void GetOrders()
    {
      var ordersFromDataBase = _context.Orders.ToArray().Select(o => new OrderViewModel(o));

      IsWorking = true;
      var ordersFromAPI = await _orderAPIService.GetOpenOrders();
      foreach (var newOrder in ordersFromAPI)
      {
        newOrder.IsTransfered = await _orderAPIService.TransferOrder(newOrder.Id);
        _context.Orders.Add(newOrder);
      }
      _context.SaveChanges();
      Orders = ordersFromAPI.Select(o => new OrderViewModel(o, true)).Concat(ordersFromDataBase).ToArray();

      IsWorking = false;
    }

    private async void DeliverOrder(OrderViewModel order)
    {      
      order.IsShipped = await _orderAPIService.DeliverOrder(order.Id);
    }

    private async void OpenDeliveryNote(OrderViewModel order)
    {
      Document = await _orderAPIService.GetFile(order.FileIdDeliveryNote.Value);
    }

    private async void OpenOrderSheet(OrderViewModel order)
    {
      Document = await _orderAPIService.GetFile(order.FileIdOrderSheet);      
    }
  }
}