using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public partial class MainWindow : Window
    {
        CurrencyConverter currencyConverter;
        //DateTime dateStart = DateTime.Today;
        //DateTime dateEnd = DateTime.Today.AddDays(-7);
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
            string fromCurrency = ((KeyValuePair<string, string>)comboboxFrom.SelectedItem).Key;
            string toCurrency = ((KeyValuePair<string, string>)comboboxTo.SelectedItem).Key;

            Regex regex = new Regex("^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$");
            
            if (!(regex.IsMatch(inputCurrencyAmount.Text)))
            {
                MessageBox.Show("Please use number");
            }
            else if (double.Parse(inputCurrencyAmount.Text) <= 0)
            {
                MessageBox.Show("Use bigger number than 0");
            }
            else
            {
                double currencyAmount = double.Parse(inputCurrencyAmount.Text);
                decimal finalValue = currencyConverter.Exchange(fromCurrency, toCurrency, currencyAmount);
                outputCurrencyAmount.Text = $"The result is {finalValue.ToString("N", CultureInfo.CreateSpecificCulture("en-US"))} {toCurrency}";
            }

            Dictionary<string, Dictionary<string, decimal>> rateData = currencyConverter.GetTimeSeries(fromCurrency, toCurrency);

            ((SplineSeries)myChart.Series[0]).ItemsSource =
                new KeyValuePair<DateTime, object>[]
                {
                    new KeyValuePair<DateTime, object>(DateTime.Now, (rateData.ElementAt(0).Value).ElementAt(0).Value),
                    new KeyValuePair<DateTime, object>(DateTime.Now.AddDays(-1), (rateData.ElementAt(1).Value).ElementAt(0).Value),
                    new KeyValuePair<DateTime, object>(DateTime.Now.AddDays(-2), (rateData.ElementAt(2).Value).ElementAt(0).Value),
                    new KeyValuePair<DateTime, object>(DateTime.Now.AddDays(-3), (rateData.ElementAt(3).Value).ElementAt(0).Value),
                    new KeyValuePair<DateTime, object>(DateTime.Now.AddDays(-4), (rateData.ElementAt(4).Value).ElementAt(0).Value),
                    new KeyValuePair<DateTime, object>(DateTime.Now.AddDays(-5), (rateData.ElementAt(5).Value).ElementAt(0).Value),
                    new KeyValuePair<DateTime, object>(DateTime.Now.AddDays(-6), (rateData.ElementAt(6).Value).ElementAt(0).Value)

                };

        }

        //private void LoadAreaChartData()
        //{
        //    ((SplineSeries)myChart.Series[0]).ItemsSource =
        //        new KeyValuePair<DateTime, int>[]
        //        {
        //            new KeyValuePair<DateTime, int>(DateTime.Now, 10000),
        //            new KeyValuePair<DateTime, int>(DateTime.Now.AddDays(-1), 9800),
        //            new KeyValuePair<DateTime, int>(DateTime.Now.AddDays(-2), 9750),
        //            new KeyValuePair<DateTime, int>(DateTime.Now.AddDays(-3), 9600),
        //            new KeyValuePair<DateTime, int>(DateTime.Now.AddDays(-4), 9500),
        //            new KeyValuePair<DateTime, int>(DateTime.Now.AddDays(-5), 9800),
        //            new KeyValuePair<DateTime, int>(DateTime.Now.AddDays(-6), 9700)

        //        };
        //}
    }
}
