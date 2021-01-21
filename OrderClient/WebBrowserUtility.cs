using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OrderClient
{
  public static class WebBrowserUtility
  {
    public static readonly DependencyProperty BindableSourceProperty =
        DependencyProperty.RegisterAttached("BindableSource", typeof(Stream), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

    public static Stream GetBindableSource(DependencyObject obj)
    {
      return (Stream)obj.GetValue(BindableSourceProperty);
    }

    public static void SetBindableSource(DependencyObject obj, Stream value)
    {
      obj.SetValue(BindableSourceProperty, value);
    }

    public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      WebBrowser browser = o as WebBrowser;

      if (browser == null)
      {
        return;
      }
      var stream = e.NewValue as Stream;

      if (stream == null)
      {
        return;
      }

      var filePath = Path.GetTempFileName();
      using (FileStream outputFileStream = new FileStream(filePath, FileMode.Append))
      {
        stream.CopyTo(outputFileStream);
      }

      browser.Navigate(filePath);
    }
  }
}
