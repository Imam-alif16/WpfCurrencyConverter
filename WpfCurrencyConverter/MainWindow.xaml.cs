using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CurrencyConverter = WpfCurrencyConverter.CurrencyConverter;

namespace WpfCurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        CurrencyConverter currencyConverter;
        List<decimal> rateDatas = new List<decimal>();
        private ObservableCollection<KeyValuePair<string, decimal>> _valueList;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<KeyValuePair<string, decimal>> ValueList
        {
            get 
            { 
                if (_valueList == null)
                {
                    _valueList = new ObservableCollection<KeyValuePair<string, decimal>>();
                }
                return _valueList;
            }
            set
            {
                if (_valueList == value)
                {
                    return;
                }

                _valueList = value;
                OnPropertyChanged("ValueList");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            currencyConverter = new CurrencyConverter();

            Dictionary<string, string> symbolData = currencyConverter.GetSymbols();
            comboboxFrom.Items.Clear();
            comboboxTo.Items.Clear();

            comboboxFrom.ItemsSource = symbolData;
            comboboxFrom.DisplayMemberPath = "Value";
            comboboxFrom.SelectedValuePath = "Key";

            comboboxTo.ItemsSource = symbolData;
            comboboxTo.DisplayMemberPath = "Value";
            comboboxTo.SelectedValuePath = "Key";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string fromCurrency = ((KeyValuePair<string, string>)comboboxFrom.SelectedItem).Key;
                string toCurrency = ((KeyValuePair<string, string>)comboboxTo.SelectedItem).Key;

                Regex regex = new Regex("^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$");

                if (!(regex.IsMatch(inputCurrencyAmount.Text)))
                {
                    MessageBox.Show("Please use number", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (double.Parse(inputCurrencyAmount.Text) <= 0)
                {
                    MessageBox.Show("Use bigger number than 0", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    double currencyAmount = double.Parse(inputCurrencyAmount.Text);
                    decimal finalValue = currencyConverter.Exchange(fromCurrency, toCurrency, currencyAmount);
                    outputCurrencyAmount.Text = $"Today result is {finalValue.ToString("N", CultureInfo.CreateSpecificCulture("en-US"))} {toCurrency} ";

                    Dictionary<string, Dictionary<string, decimal>> rateData = new Dictionary<string, Dictionary<string, decimal>>();

                    if (rateData.Count == 0)
                    {
                        rateData = currencyConverter.GetTimeSeries(fromCurrency, toCurrency);

                        rateDatas.Add((rateData.ElementAt(0).Value).ElementAt(0).Value);
                        rateDatas.Add((rateData.ElementAt(1).Value).ElementAt(0).Value);
                        rateDatas.Add((rateData.ElementAt(2).Value).ElementAt(0).Value);
                        rateDatas.Add((rateData.ElementAt(3).Value).ElementAt(0).Value);
                        rateDatas.Add((rateData.ElementAt(4).Value).ElementAt(0).Value);
                        rateDatas.Add((rateData.ElementAt(5).Value).ElementAt(0).Value);
                        rateDatas.Add((rateData.ElementAt(6).Value).ElementAt(0).Value);

                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd"), rateDatas[0]));
                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"), rateDatas[1]));
                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd"), rateDatas[2]));
                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"), rateDatas[3]));
                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), rateDatas[4]));
                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), rateDatas[5]));
                        ValueList.Add(new KeyValuePair<string, decimal>(DateTime.Now.ToString("yyyy-MM-dd"), rateDatas[6]));

                        myChart.DataContext = ValueList;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }
    }
}
