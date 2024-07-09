using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using Fiken.Sdk;
using Shopify2Fiken.Domain.Types;

namespace GUI;

public partial class LoadedOrdersWindow : Window
{
    private readonly List<ParsedCsvOrder> _orders;

    public LoadedOrdersWindow(List<ParsedCsvOrder> orders)
    {
        InitializeComponent();
        
        OrdersDataGrid.ItemsSource = orders;
        OrdersDataGrid.IsReadOnly = true;
        _orders = orders;
    }

    private async void ImportButton_OnClick(object sender, RoutedEventArgs e)
    {
        ImportButton.IsEnabled = false;
        OrdersDataGrid.Visibility = Visibility.Collapsed;
        StatusTextBox.Visibility = Visibility.Visible;

        var configString = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appsettings.json"));
        
        var config = JsonSerializer.Deserialize<FikenConfig>(configString);

        var communicator = new FikenCommunicator(config);

        await communicator.AddOrders(_orders);
        
        






    }
}
