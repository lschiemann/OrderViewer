using System.Windows;
using OrderClient.Model;
using OrderClient.Views;
using Prism.Ioc;

namespace OrderClient
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    protected override Window CreateShell()
    {
      return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<IOrderAPIService, OrderAPIService>();
      containerRegistry.RegisterInstance(new Context());
    }
  }
}
