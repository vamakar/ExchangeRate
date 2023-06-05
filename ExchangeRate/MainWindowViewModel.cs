using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ExchangeRate.Annotations;

namespace ExchangeRate
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private readonly GetExchangeRateService _getExchangeRateService;
        private readonly CompareExchangeRatesService _compareService;
        private ObservableCollection<CurrencyData> _filteredDataCollection;
        private string _currencyToRubAndUsd = string.Empty;
        private CurrencyData _selectedData;
        private CurrencyData _convertFrom;
        private CurrencyData _convertTo;
        private IReadOnlyCollection<CurrencyData> _currencyRateToday;
        private IReadOnlyCollection<CurrencyData> _currencyRateByDate;
        private decimal _valueToConvert;
        private decimal _valueConverted;
        private DateTime _toLoadDateTime;
        private object _comparisonTitle;

        public MainWindowViewModel()
        {
            _getExchangeRateService = new GetExchangeRateService();
            _compareService = new CompareExchangeRatesService();
            ToLoadDateTime = DateTime.Now;
            ComparisonTitle = $"Курсы вылют ЦБ на {ToLoadDateTime:dd.MM.yyyy}";
        }

        public ObservableCollection<CurrencyData> FilteredDataCollection
        {
            get => _filteredDataCollection;
            private set
            {
                _filteredDataCollection = value; 
                OnPropertyChanged(nameof(FilteredDataCollection));
            }
        }

        public CurrencyData SelectedData
        {
            get => _selectedData;
            set
            {
                _selectedData = value;
                OnListItemSelected();
            }
        }

        public CurrencyData ConvertFrom
        {
            get => _convertFrom;
            set
            {
                _convertFrom = value;
                RefreshConvert();
                OnPropertyChanged(nameof(ConvertFrom));
            }
        }

        public CurrencyData ConvertTo
        {
            get => _convertTo;
            set
            {
                _convertTo = value;
                RefreshConvert();
                OnPropertyChanged(nameof(ConvertTo));
            }
        }

        public decimal ValueToConvert
        {
            get => _valueToConvert;
            set
            {
                _valueToConvert = value;
                OnPropertyChanged(nameof(ValueToConvert));
            }
        }

        public decimal ValueConverted
        {
            get => _valueConverted;
            private set
            {
                _valueConverted = value;
                OnPropertyChanged(nameof(ValueConverted));
            }
        }

        public string CurrencyToRubAndUsd
        {
            get => _currencyToRubAndUsd;
            set
            {
                _currencyToRubAndUsd = value;
                OnPropertyChanged(nameof(CurrencyToRubAndUsd));
            }
        }

        public DateTime ToLoadDateTime
        {
            get => _toLoadDateTime;
            set
            {
                _toLoadDateTime = value;
                OnPropertyChanged(nameof(ToLoadDateTime));
            }
        }

        public string ToSearch { get; set; }

        public object ComparisonTitle
        {
            get => _comparisonTitle;
            set
            {
                _comparisonTitle = value; 
                OnPropertyChanged(nameof(ComparisonTitle));
            }
        }

        public void OnSearchButtonClick()
        {
            if(_currencyRateByDate != null) SelectCodeList();
        }

        public void OnSwapButtonClick()
        {
            decimal tmpConverted = ValueConverted;
            (ConvertFrom, ConvertTo) = (ConvertTo, ConvertFrom);
            ValueToConvert = tmpConverted;
        }

        public async void OnLoadButtonClick()
        {
            ExchangeRateData exchangeRateToday = await _getExchangeRateService.GetExchangeRateByDate(DateTime.Now);
            ExchangeRateData exchangeRateByDate = await _getExchangeRateService.GetExchangeRateByDate(ToLoadDateTime);
            _currencyRateToday = exchangeRateToday.DataCollection;
            _currencyRateByDate = exchangeRateByDate.DataCollection;

            ComparisonTitle = $"Курсы вылют ЦБ на {exchangeRateByDate.Date}";
            CurrencyToRubAndUsd = string.Empty;
            RefreshCodeList();
        }

        public void OnListItemSelected()
        {
            if (SelectedData != null)
            {
                CurrencyToRubAndUsd = _compareService.CompareExchangeRates(SelectedData, _currencyRateToday, _currencyRateByDate);
            }
        }

        public void RefreshCodeList()
        {
            FilteredDataCollection = new ObservableCollection<CurrencyData>(_currencyRateByDate);
        }

        public void RefreshConvert()
        {
            if (ConvertFrom != null && ConvertTo != null)
                ValueConverted = ValueToConvert / ConvertTo.Value * ConvertTo.Nominal * ConvertFrom.Value / ConvertFrom.Nominal;
        }

        public void SelectCodeList()
        {
            FilteredDataCollection = ToSearch != null
                ? new ObservableCollection<CurrencyData>(_currencyRateByDate.Where(
                    x => x.Code.Contains(ToSearch, StringComparison.InvariantCultureIgnoreCase)
                         || x.Name.Contains(ToSearch, StringComparison.InvariantCultureIgnoreCase)))
                : new ObservableCollection<CurrencyData>(_currencyRateByDate);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
