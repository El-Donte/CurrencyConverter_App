using CurrencyConverter_App.CurrencyApi;
using CurrencyConverter_App.CurrencyConvert;
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

namespace CurrencyConverter_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CurrencyConverter converter;
        public MainWindow()
        {
            InitializeComponent();
            // собрать конвертер (TODO: использовать фабрику)
            converter = new CurrencyConverter(new CurrencyApiClient());
            // синхронное вызвать обновление в синхронном режиме при первой загрузке
           // RefreshData().Wait();
        }

        // вспомогательный метод для обновления данных на форме
        private async Task RefreshData()
        {
            try
            {
                await converter.RefreshAsync();
                List<string> currencyCodes = converter.GetCurrencyCodes();
                // set from codes
                fromCurrencyComboBox.ItemsSource = null;
                fromCurrencyComboBox.ItemsSource = currencyCodes;
                fromCurrencyComboBox.SelectedIndex = 0;
                // set to codes
                toCurrencyComboBox.ItemsSource = null;
                toCurrencyComboBox.ItemsSource = currencyCodes;
                toCurrencyComboBox.SelectedIndex = 0;
                // set data grid codes
                dataGridCurrencyComboBox.ItemsSource = null;
                dataGridCurrencyComboBox.ItemsSource = currencyCodes;
                dataGridCurrencyComboBox.SelectedIndex = 0;
                //
                fromValueTextBox.Text = "1";
                toValueTextBox.Text = "1";
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fromCurrencyCode = fromCurrencyComboBox.Text;
                string toCurrencyCode = toCurrencyComboBox.Text;
                decimal value = Convert.ToDecimal(fromValueTextBox.Text);
                decimal result = converter.Convert(fromCurrencyCode, toCurrencyCode, value);
                toValueTextBox.Text = Math.Round(result, 3).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // обработчик кнопки "поменять местами"
        private void swapButton_Click(object sender, RoutedEventArgs e)
        {
            (fromValueTextBox.Text, toValueTextBox.Text) = (toValueTextBox.Text, fromValueTextBox.Text);
            (fromCurrencyComboBox.Text, toCurrencyComboBox.Text) = (toCurrencyComboBox.Text, fromCurrencyComboBox.Text);
        }

        // вспомогательный метод выгрузки валют в DataGrid
        private void FillCurrencyDataGrid(string basicCurrency)
        {
            List<CurrencyData> currencyDatas = converter.GetCurrencyData(basicCurrency);
            // выведем данные в dataGrid
            currencyDataGrid.Items.Clear();
            currencyDataGrid.Columns.Clear();
            //
            DataGridTextColumn codeCol = new DataGridTextColumn();
            DataGridTextColumn unitCostCol = new DataGridTextColumn();
            currencyDataGrid.Columns.Add(codeCol);
            currencyDataGrid.Columns.Add(unitCostCol);
            codeCol.Binding = new Binding("CurrencyCode");
            unitCostCol.Binding = new Binding("UnitCost");
            codeCol.Header = "Код";
            unitCostCol.Header = $"Стоимость 1 ед. в {basicCurrency}";
            foreach (CurrencyData currencyData in currencyDatas)
            {
                currencyData.UnitCost = Math.Round(currencyData.UnitCost, 3);
                currencyDataGrid.Items.Add(currencyData);
            }
        }

        private void dataGridCurrencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCurrencyComboBox.SelectedIndex != -1)
            {
                FillCurrencyDataGrid(dataGridCurrencyComboBox.SelectedItem.ToString());
            }
        }
    }
}
