using CashRegister.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CashRegisterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<ReceiptLine> ReceiptLines { get; set; } = new ObservableCollection<ReceiptLine>();
        public HttpClient client = new HttpClient();
        public decimal TotalAmount { get => ReceiptLines.Sum(rl => rl.TotalPrice); }
        public MainWindow()
        {
            InitializeComponent();
            FetchProducts();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task FetchProducts()
        {
            var resp = await client.GetAsync("http://localhost:49430/api/products");
            resp.EnsureSuccessStatusCode();
            var products = await JsonSerializer.DeserializeAsync<List<Product>>(await resp.Content.ReadAsStreamAsync());
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        public async Task Checkout()
        {
            var lines = ReceiptLines.Select(rl => new ReceiptLineDto { Amount = rl.Amount, ProductID = rl.Product.ID }).ToList();
            var content = new StringContent(JsonSerializer.Serialize(lines), Encoding.UTF8, "text/json");
            var resp = await client.PostAsync("http://localhost:49430/api/receipts", content);
            resp.EnsureSuccessStatusCode();
            ReceiptLines.Clear();
            OnPropertyChanged(nameof(TotalAmount));
        }

        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Checkout();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(((Button)sender).Tag.ToString());
            var existingLine = ReceiptLines.FirstOrDefault(rl => rl.Product.ID == id);
            if (existingLine != null)
            {
                existingLine.Amount += 1;
                existingLine.TotalPrice += existingLine.Product.UnitPrice;
            }
            else
            {
                var boughtProduct = Products.First(p => p.ID == id);
                var newLine = new ReceiptLine { Amount = 1, Product = boughtProduct, TotalPrice = boughtProduct.UnitPrice };
                ReceiptLines.Add(newLine);
            }
            OnPropertyChanged(nameof(ReceiptLines));
            OnPropertyChanged(nameof(TotalAmount));
        }
    }
}
