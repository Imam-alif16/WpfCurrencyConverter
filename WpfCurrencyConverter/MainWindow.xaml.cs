using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using CurrencyConverter = WpfCurrencyConverter.CurrencyConverter;

namespace WpfCurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CurrencyConverter currencyConverter;
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
    }
}
