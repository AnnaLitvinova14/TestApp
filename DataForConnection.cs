using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


//БИНДИНГИ
namespace WpfApp
{
    public class DataForConnection : INotifyPropertyChanged //dataContext всего окна
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

        //новые значения
        private ObservableCollection<DataNewRegValues> newRegValues; //переменная(коллекция), в которой хранятся внесенные изменения
        //свойство, отвечающее за (изменение)содержание таблицы
        public ObservableCollection<DataNewRegValues> NewRegValues { get => newRegValues; set { newRegValues = value; RaisePropertyChanged("NewRegValues"); } }

        //регистры
        private ObservableCollection<Register> registers; //переменная(коллекция), в которой хранится перечень регистров
        public ObservableCollection<Register> Registers { get => registers; set { registers = value; } }

    }

    public class Register //регистры
    {
        private string nameRegister;
        public string NameRegister { get => nameRegister; set { nameRegister = value;} }
    }

    public class BoolValue
    {
        private string bool_Value;
        public string Bool_Value { get => bool_Value; set { bool_Value = value; } }
    }

    public class DataNewRegValues: INotifyPropertyChanged //dataContex для таблицы
    {
        //привязка к полям таблицы
        private string regAddress;
        private string regOldValue;
        private string regValue;
        public DataNewRegValues(string regAddress, string regOldValue, string regValue = null, ObservableCollection<BoolValue> B_Values = null)
        {
            this.RegAddress = regAddress;
            this.regOldValue = regOldValue;
            this.RegValue = regValue;
            this.B_Values = b_Values;
        }
        public string RegAddress { get => regAddress; set { regAddress = value; RaisePropertyChanged("RegAddress"); } }
        public string RegOldValue { get => regOldValue; set { regOldValue = value; RaisePropertyChanged("RegOldValue"); } }
        public string RegValue { get => regValue; set { regValue = value; RaisePropertyChanged("RegValue"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //комбобокс таблицы 
        private ObservableCollection<BoolValue> b_Values; //переменная(коллекция), в которой хранится перечень bool значений для combobox
        public ObservableCollection<BoolValue> B_Values { get => b_Values; set { b_Values = value; } }


    }


}