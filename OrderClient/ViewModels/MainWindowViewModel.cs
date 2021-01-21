using OrderClient.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrderClient.ViewModels
{
  public class MainWindowViewModel : BindableBase
  {
    private readonly IOrderAPIService _orderAPIService;
    private readonly Context _context;
    private DateTime _lastUpdate;

    public MainWindowViewModel(IOrderAPIService apiService, Context context)
    {
      _orderAPIService = apiService;
      _context = context;
      GetOpenOrdersCommand = new DelegateCommand(GetOrdersFromAPI);
      ClearFilterCommand = new DelegateCommand(ClearFilter);
      NextPageCommand = new DelegateCommand(GoToNextPage, IsNextPagePossible);
      PreviousPageCommand = new DelegateCommand(GoToPreviousPage, IsPreviousPagePossible);
      LastPageCommand = new DelegateCommand(GoToLastPage, IsNextPagePossible);
      FirstPageCommand = new DelegateCommand(GoToFirstPage, IsPreviousPagePossible);
      DeliverOrderCommand = new DelegateCommand<OrderViewModel>(DeliverOrder);
      OpenOrderSheetCommand = new DelegateCommand<OrderViewModel>(OpenOrderSheet);
      OpenDeliveryNoteCommand = new DelegateCommand<OrderViewModel>(OpenDeliveryNote);
      FilterString = string.Empty;
    }

    private int _pageSize = 100;
    public int PageSize
    {
      get => _pageSize;
      set
      {
        SetProperty(ref _pageSize, value);
        GoToFirstPage();        
      }
    }

    private readonly IEnumerable<int> _availablePageSizes = new int[] { 10, 50, 100, 500, 1000, 10000 };
    public IEnumerable<int> AvailablePageSizes
    {
      get => _availablePageSizes;
    }

    private int _currentPage;
    private int CurrentPage
    {
      get => _currentPage;
      set
      {
        _currentPage = value;
        RaisePropertyChanged(nameof(Orders));
        FirstPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
        RaisePropertyChanged(nameof(CurrentPageLabel));
      }
    }

    private int _lastPage;
    private int LastPage
    {
      get => _lastPage;
      set
      {
        _lastPage = value;
        RaisePropertyChanged(nameof(CurrentPageLabel));
      }
    }

    public string CurrentPageLabel
    {
      get => $"{CurrentPage} of {LastPage}";
    }

    private string _filterString;
    public string FilterString
    {
      get => _filterString;
      set
      {
        SetProperty(ref _filterString, value);
        GoToFirstPage();
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

    public IEnumerable<OrderViewModel> Orders
    {
      get => GetOrdersForCurrentPage();
    }

    public DelegateCommand FirstPageCommand { get; private set; }
    public DelegateCommand NextPageCommand { get; private set; }
    public DelegateCommand PreviousPageCommand { get; private set; }
    public DelegateCommand LastPageCommand { get; private set; }
    public DelegateCommand ClearFilterCommand { get; private set; }
    public DelegateCommand GetOpenOrdersCommand { get; private set; }
    public DelegateCommand<OrderViewModel> DeliverOrderCommand { get; private set; }
    public DelegateCommand<OrderViewModel> OpenOrderSheetCommand { get; private set; }
    public DelegateCommand<OrderViewModel> OpenDeliveryNoteCommand { get; private set; }

    private bool IsPreviousPagePossible()
    {
      return CurrentPage > 1;
    }

    private bool IsNextPagePossible()
    {
      return CurrentPage < LastPage; ;
    }

    private void GoToNextPage()
    {
      CurrentPage++;
    }

    private void GoToPreviousPage()
    {
      CurrentPage--;
    }

    private void GoToFirstPage()
    {
      CurrentPage = 1;
    }

    private void GoToLastPage()
    {
      CurrentPage = LastPage;
    }

    private void ClearFilter()
    {
      FilterString = string.Empty;
    }

    private IEnumerable<OrderViewModel> GetOrdersForCurrentPage()
    {
      var filterString = FilterString.ToLower();
      int.TryParse(filterString, out var filterNumber);

      var ordersForFilter = _context.Orders.Where(o => o.Name.ToLower().Contains(filterString) || o.Address.ToLower().Contains(filterString) || filterNumber == o.OrderNumber).OrderByDescending(o => o.ReceivedDate);
      
      var count = ordersForFilter.Count();
      var pageCount = count / PageSize;
      if (count % PageSize > 0)
      {
        pageCount++;
      }
      LastPage = pageCount;

      var skipCount = (CurrentPage - 1) * PageSize;

      var ordersForPage = ordersForFilter.Skip(skipCount).Take(PageSize);

      return ordersForPage.Select(o => new OrderViewModel(o, _lastUpdate)).ToArray();
    }

    private async void GetOrdersFromAPI()
    {
      IsWorking = true;

      _lastUpdate = DateTime.Now;

      foreach (var newOrder in await _orderAPIService.GetOpenOrders(_lastUpdate))
      {
        //acknowledge receiving of order
        _context.Orders.Add(newOrder);
        newOrder.IsTransfered = await _orderAPIService.TransferOrder(newOrder.Id);
      }
      _context.SaveChanges();
      GoToFirstPage();
      IsWorking = false;
    }

    private async void DeliverOrder(OrderViewModel order)
    {
      order.IsShipped = await _orderAPIService.DeliverOrder(order.Id);
      _context.SaveChanges();
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