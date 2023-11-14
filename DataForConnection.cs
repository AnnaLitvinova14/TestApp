using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp
{
    public class DataForConnection: INotifyPropertyChanged //dataContext всего окна
    {
        public string sIPAdr { get; set; }
        public string sPortTCP { get; set; }
        public string sClient { get; set; }
        public string sCountNewValues { get; set; }
        public string sStartValue { get; set; }
        public string sStep { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private ObservableCollection<DataNewRegValues> newRegValues; //переменная, в которой хранятся внесенные изменения
        //свойство, отвечающее за (изменение)содержание таблицы
        public ObservableCollection<DataNewRegValues> NewRegValues { get => newRegValues; set { newRegValues = value; RaisePropertyChanged("NewRegValues"); } }

    }

    public class DataTable
    {
        public DataTable(string iAddr, string iValue)
        {
            this.iAddr = iAddr;
            this.iValue = iValue;
        }
        public string iAddr { get; set; }
        public string iValue { get; set; }

    }

    public class DataNewRegValues: INotifyPropertyChanged //dataContex для таблицы
    {
        //привязка к полям таблицы
        private string regAddress;
        private string regOldValue;
        private string regValue;
        public DataNewRegValues(string regAddress, string regOldValue, string regValue)
        {
            this.RegAddress = regAddress;
            this.regOldValue = regOldValue;
            this.RegValue = regValue;
        }
        public string RegAddress { get => regAddress; set { regAddress = value; RaisePropertyChanged("RegAddress"); } }
        public string RegOldValue { get => regOldValue; set { regValue = value; RaisePropertyChanged("RegOldValue"); } }
        public string RegValue { get => regValue; set { regValue = value; RaisePropertyChanged("RegValue"); } }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


}