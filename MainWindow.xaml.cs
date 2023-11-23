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
using System.Text.RegularExpressions;
using System.Net;
using Modbus.Device;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Net.Http;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;
using Modbus.Data;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        //server
        ModbusTcpSlave Slave;
        TcpListener Listener;

        //регистр типа ushort
        ModbusDataCollection<ushort> uRegister; 

        //регистр типа bool
        ModbusDataCollection<bool> bRegister; 

        //запись; относится к таблице
        DataNewRegValues item;

        string SelectItemValue;

        //словарик для записи внесенных в таблицу значений со стороны сервера
        Dictionary<int, string> updateRegValues = new Dictionary<int, string>(); 
        
        //данные из таблицы
        ObservableCollection<DataNewRegValues> collectionRegValues = new ObservableCollection<DataNewRegValues>();
        
        //регистры
        ObservableCollection<Register> collectionRegisters = new ObservableCollection<Register>()
        {
            new Register(){ NameRegister = "Holding Registers" },
            new Register(){ NameRegister = "Input Registers" },
            new Register(){ NameRegister = "Coils" },
            new Register(){ NameRegister = "Discrete Inputs" }
        };
        
        //для колонки таблицы с combobox 
        ObservableCollection<BoolValue> collectionB_Values = new ObservableCollection<BoolValue>()
        {
            new BoolValue(){ Bool_Value = "False" },
            new BoolValue(){ Bool_Value = "True" }
        };
        
        //таймер
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        //datacontex всего окна
        DataForConnection objectInfo;

        //флаг для видимости в таблице колонки "Новое значение"
        int flagVisibility;
        public MainWindow()
        {
            InitializeComponent();

            //подвязываем значения по умолчанию к UI
            objectInfo = new DataForConnection 
            {
                sIPAdr = Constans.sIP_default,
                sPortTCP = Constans.sTCP_default,
                sClient = Constans.sClient_default,
                sStartValue = Constans.sStartValue_default,
                sStep = Constans.sStep_default,
                //подвязали коллекцию (рег+значения) таблицы к свойству графического элемента datagrid
                NewRegValues = collectionRegValues,
                //подвязали коллекцию регистров к свойству графического элемента combobox
                Registers = collectionRegisters 
            };

            //привязка dataContext всего окна
            this.DataContext = objectInfo;
        }

        private void UpdateDataGrid() //обновление данных в таблице
        {
            //шаг записей
            int countRecords = int.Parse(objectInfo.sStep);
            //с какой записи стартуем
            int iStartValue = int.Parse(objectInfo.sStartValue);

            int indexCombobox = 0;

            //СТАРТ чтение новых, введенных в таблицу, значений в словарик перед очисткой таблицы
            updateRegValues.Clear();
            //если есть новые введенные значения
            if (objectInfo.NewRegValues.Count > 0)
            {
                //пробег по таблице
                for (int j = 0; j < objectInfo.NewRegValues.Count; j++)
                {
                    //если столбец с изменениями текстовый
                    if (flagVisibility == 0)
                    {                        
                        //запись в словарик по нужному адресу(ключ) нового введенного значения
                        updateRegValues.Add(int.Parse(objectInfo.NewRegValues[j].RegAddress), objectInfo.NewRegValues[j].RegValue);
                    }
                    else if (flagVisibility == 1)
                    {                     
                        SelectItemValue = Convert.ToBoolean(objectInfo.NewRegValues[j].CbSelectItem) ? "True" : "False";
                        updateRegValues.Add(int.Parse(objectInfo.NewRegValues[j].RegAddress), SelectItemValue);
                    }
                }
            }

            //обновление таблицы, внесение записей
            collectionRegValues.Clear(); 
            for (int i = iStartValue; i < countRecords + iStartValue; i++)
            {
                string result;
                //если есть измененные значения в словарике и по адресу можно получить новое значение
                if (updateRegValues.Count != 0 && updateRegValues.TryGetValue(i, out string sRegValue))
                {
                    result = RegisterType(i, sRegValue);
                    //для текстового столбца таблицы
                    if (flagVisibility == 0)
                    {
                        //запись в таблицу с новыми значениями
                        item = new DataNewRegValues(i.ToString(), result, null, indexCombobox);
                    }
                    //для комбобокс столбца таблицы
                    else
                    {
                        if (result == "False") indexCombobox = 0; else indexCombobox = 1;
                        item = new DataNewRegValues(i.ToString(), result, collectionB_Values, indexCombobox);
                    }
                }
                //если нет изменений, грузим текущие значения клиента
                else
                {
                    //запись в таблицу текущих значений
                    //0 - текстовый; 1 - комбобокс
                    result = RegisterType(i);
                    if (flagVisibility == 0)
                        item = new DataNewRegValues(i.ToString(), result, null, indexCombobox);
                    else
                    {
                        if (result == "False") indexCombobox = 0; else indexCombobox = 1;
                        item = new DataNewRegValues(i.ToString(), result, collectionB_Values, indexCombobox);
                    }

                }
                collectionRegValues.Add(item);
            }
            //КОНЕЦ чтение новых, введенных в таблицу, значений в словарик перед очисткой таблицы

        }

        private void VisibilityColumns(int flag) //видимость в таблице столбца "Новое значение" в зависимости от выбранного регистра
        {
            if (flag == 0) //Holding
            {
                cb_rValue.Visibility = Visibility.Collapsed;
                rValue.Visibility = Visibility.Visible;
            }
            else if (flag == 1) //Coils
            {
                cb_rValue.Visibility = Visibility.Visible;
                rValue.Visibility = Visibility.Collapsed;
            }
            else if (flag == 2) //Input, Discrete
            {
                cb_rValue.Visibility = Visibility.Collapsed;
                rValue.Visibility = Visibility.Collapsed;
            }
            
        }
        private string RegisterType(int rAddr, string rValue = null)//назначение регистра и получение значений 
        {            
            // 0 - H; 1 - I; 2 - C; 3 - DI
            string Result = "error";

            if (objectInfo != null)
            {
                if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.H_Register)
                {
                    VisibilityColumns(flagVisibility);
                    uRegister = Slave.DataStore.HoldingRegisters;

                    //если есть новое значение, то присваиваем его регистру по нужному адресу
                    if (rValue != null) 
                        uRegister[rAddr] = ushort.Parse(rValue);
                    
                    Result = uRegister[rAddr].ToString();
                }
                else if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.I_Register)
                {
                    VisibilityColumns(flagVisibility);
                    uRegister = Slave.DataStore.InputRegisters;

                    Result = uRegister[rAddr].ToString();
                }
                else if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.C_Register)
                {
                    VisibilityColumns(flagVisibility);
                    bRegister = Slave.DataStore.CoilDiscretes;

                    //если есть новое значение, то присваиваем его регистру по нужному адресу
                    if (rValue != null)
                        bRegister[rAddr] = bool.Parse(rValue);

                    //  Console.WriteLine($"rAddr = {rAddr}; rValue = {rValue}");
                    Result = bRegister[rAddr].ToString();
                }
                else if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.DI_Register)
                {
                    VisibilityColumns(flagVisibility);
                    bRegister = Slave.DataStore.InputDiscretes;
                    Result = bRegister[rAddr].ToString();
                }
                return Result;
            }
            else return Result;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)//нажатие на кнопку "Запустить"/"Остановить"
        {
            //нажатие на кнопку "Запустить"
            if (btnStart.Content.ToString() == "Запустить")
            {
                btnStart.Content = "Остановить";
                Listener = new TcpListener(IPAddress.Parse(objectInfo.sIPAdr), int.Parse(objectInfo.sPortTCP));
                Listener.Start();
                Slave = ModbusTcpSlave.CreateTcp(Constans.slaveID, Listener);       
                Slave.Listen();
                
                btnUpdateData.IsEnabled = true;

                lblStatus.Content = "Статус: В работе";
                timer.Tick += new EventHandler(timerTick);
                timer.Interval = new TimeSpan(0, 0, 5);
                timer.Start();
            }
            //нажатие на кнопку "Остановить"
            else if (btnStart.Content.ToString() == "Остановить")
            {
                btnStart.Content = "Запустить";
                btnUpdateData.IsEnabled = false;
                Slave.Dispose();
                Slave = null;

                lblStatus.Content = "Статус: Остановлен";
                timer.Stop();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)//нажатие на кнопку "Обновить данные"
        {
            UpdateDataGrid();
        }

        private void timerTick(object sender, EventArgs e)//таймер, сканирующий новые подключения
        {
            int iClient = int.Parse(edtClient.Text);
            
            if (Slave.Masters.Count > iClient)
            {
                //если подключается больше клиентов, чем указано
                for (int i = iClient; i < Slave.Masters.Count; i++)
                {   
                    //закрыть соединение
                    Slave.Masters[i].Client.Disconnect(false);
                }
            }
        }

        private void cbTypeRegister_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //присвоение значения флагу видимости колонки "Новое значение"
            //0 - текстовый столбец; 1 - комбобокс; 2 - скрыть
            if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.H_Register) 
                flagVisibility = 0;
            else 
            if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.I_Register || objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.DI_Register)
                flagVisibility = 2;
            else 
            if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.C_Register)
                flagVisibility = 1;

            if (Slave != null)
                UpdateDataGrid();
        }

    }
}