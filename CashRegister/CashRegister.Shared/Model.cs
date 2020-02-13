using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CashRegister.Shared
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }

    public class ReceiptLine: INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int IDValue;
        [JsonPropertyName("id")]
        public int ID
        {
            get => IDValue;
            set
            {
                IDValue = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        [JsonPropertyName("product")]
        public Product Product { get; set; }

        private int AmountValue;
        [JsonPropertyName("amount")]
        public int Amount
        {
            get => AmountValue;
            set
            {
                AmountValue = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
        private decimal TotalPriceValue;
        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice
        {
            get => TotalPriceValue;
            set
            {
                TotalPriceValue = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    public class Receipt
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("receiptTimestamp")]
        public DateTime ReceiptTimestamp { get; set; }

        [JsonPropertyName("receiptLines")]
        public List<ReceiptLine> ReceiptLines { get; set; }

        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }

    public class ReceiptLineDto
    {
        public int ProductID { get; set; }

        public int Amount { get; set; }
    }
}