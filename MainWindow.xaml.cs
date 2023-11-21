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
        ModbusTcpSlave Slave;//server
        TcpListener Listener;

        ModbusDataCollection<ushort> uRegister; //регистр типа ushort
        ModbusDataCollection<bool> bRegister; //регистр типа bool

        DataNewRegValues item; //запись; относится к таблице

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
            new BoolValue(){ Bool_Value = "True" },
            new BoolValue(){ Bool_Value = "False" }
        };
        
        //таймер
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
     
        DataForConnection objectInfo;//datacontex всего окна
        int flagVisibility;//флаг для видимости в таблице колонки "Новое значение"
        public MainWindow()
        {
            InitializeComponent();

            objectInfo = new DataForConnection //подвязываем значения по умолчанию к UI
            {
                sIPAdr = Constans.sIP_default,
                sPortTCP = Constans.sTCP_default,
                sClient = Constans.sClient_default,
                sStartValue = Constans.sStartValue_default,
                sStep = Constans.sStep_default,
                NewRegValues = collectionRegValues, //подвязали коллекцию (рег+значения) таблицы к свойству графического элемента datagrid
                Registers = collectionRegisters //подвязали коллекцию регистров к свойству графического элемента combobox
            };

            this.DataContext = objectInfo;//привязка dataContext всего окна
        }

        private void UpdateDataGrid() //обновление данных в таблице
        {
            int countRecords = int.Parse(objectInfo.sStep); //шаг записей
            int iStartValue = int.Parse(objectInfo.sStartValue); //с какой записи стартуем

            //чтение новых, введенных в таблицу, значений в словарик перед очисткой таблицы
            updateRegValues.Clear();
            if (objectInfo.NewRegValues.Count > 0)
            {                
                for (int j = 0; j < objectInfo.NewRegValues.Count; j++)
                {
                    updateRegValues.Add(int.Parse(objectInfo.NewRegValues[j].RegAddress), objectInfo.NewRegValues[j].RegValue);
                }
            }

            //обновление таблицы
            collectionRegValues.Clear();
            for (int i = iStartValue; i < countRecords + iStartValue; i++)
            {           
                if (flagVisibility == 1) //если = 1, отобразить столбец с комбобокс
                {
                    Console.WriteLine("flagVisibility == 1");
                    /*if (updateRegValues.Count != 0 && updateRegValues.TryGetValue(i, out string sRegValue)
                        item = new DataNewRegValues(i.ToString(), RegisterType(i, sRegValue), null, collectionB_Values)
                    else*/

                }
                else //отобразить редактируемый столбец без комбобокс
                {
                    Console.WriteLine("flagVisibility == " + flagVisibility.ToString());
                    //если есть измененные значения и по адресу можно получить новое значение
                    if (updateRegValues.Count != 0 && updateRegValues.TryGetValue(i, out string sRegValue))
                    {
                        item = new DataNewRegValues(i.ToString(), RegisterType(i, sRegValue)); //запись в таблицу с новыми значениями
                    }    
                    else
                    {
                        item = new DataNewRegValues(i.ToString(), RegisterType(i));//запись в таблицу старых значений
                    }                        
                }                
                collectionRegValues.Add(item);
            }

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
                    flagVisibility = 0;
                    VisibilityColumns(flagVisibility);
                    uRegister = Slave.DataStore.HoldingRegisters;
                    
                    if (rValue != null) //если есть новое значение, то присваиваем его регистру по нужному адресу
                        uRegister[rAddr] = ushort.Parse(rValue);
                    
                    Result = uRegister[rAddr].ToString();
                }
                else if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.I_Register)
                {
                    flagVisibility = 2;
                    VisibilityColumns(flagVisibility);
                    uRegister = Slave.DataStore.InputRegisters;

                    Result = uRegister[rAddr].ToString();
                }
                else if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.C_Register)
                {
                    flagVisibility = 1;
                    VisibilityColumns(flagVisibility);
                    bRegister = Slave.DataStore.CoilDiscretes;

                    if (rValue != null) //если есть новое значение, то присваиваем его регистру по нужному адресу
                        bRegister[rAddr] = bool.Parse(rValue);

                    Result = bRegister[rAddr].ToString();
                }
                else if (objectInfo.Registers[cbTypeRegister.SelectedIndex].NameRegister == Constans.DI_Register)
                {
                    flagVisibility = 2;
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
            if (btnStart.Content.ToString() == "Запустить")//нажатие на кнопку "Запустить"
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
            else if (btnStart.Content.ToString() == "Остановить")//нажатие на кнопку "Остановить"
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
            UpdateDataGrid();
        }

    }
}