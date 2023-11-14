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

namespace WpfApp
{
    public partial class MainWindow : Window
    {        
        ModbusTcpSlave Slave;
        TcpListener Listener;
        ModbusIpMaster master;
        public ObservableCollection<DataNewRegValues> collectionRegValues = new ObservableCollection<DataNewRegValues>();
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        DataForConnection objectInfo;
        public MainWindow()
        {
            InitializeComponent();

            objectInfo = new DataForConnection
            {
                sIPAdr = Constans.sIP_default,
                sPortTCP = Constans.sTCP_default,
                sClient = Constans.sClient_default,
                sCountNewValues = Constans.sCountNewValues_default,
                sStartValue = Constans.sStartValue_default,
                sStep = Constans.sStep_default,
                NewRegValues = collectionRegValues //подвязали коллекцию таблицы к свойству графического элемента
            }; 

            this.DataContext = objectInfo;

        }


        private void UpdateDataGrid()
        {
            /* int length = int.Parse(edtStep.Text);
             List<DataTable> dTable = new List<DataTable>(length);
             for (int i = int.Parse(edtStartValue.Text); i < length + int.Parse(edtStartValue.Text); i++)
             {
                 dTable.Add(new DataTable(i.ToString(), RegisterType(i, 1)));
             }
             dtGrid.ItemsSource = dTable; */

            int length = int.Parse(edtStep.Text); //objectInfo.sStep
            List<DataTable> dTable = new List<DataTable>(length);
            for (int i = int.Parse(edtStartValue.Text); i < length + int.Parse(edtStartValue.Text); i++)
            {
                dTable.Add(new DataTable(i.ToString(), RegisterType(i, 1)));
            }
            dtGrid.ItemsSource = dTable;

        }

        private string RegisterType(int rAddr, int flag)
        {

            // 0 - H; 1 - I; 2 - C; 3 - DI 

          /*  else if (cbTypeRegister.SelectedIndex == 1)
            {
                var register = Slave.DataStore.InputRegisters; 
                ushort rValue = ushort.Parse(edtRegValue.Text);
                if (edtRegValue.Text != "") rValue = ushort.Parse(edtRegValue.Text);
                if (flag == 0)
                    register[rAddr] = rValue;
                return register[rAddr].ToString();
            }
            else if (cbTypeRegister.SelectedIndex == 2)
            {
                var register = Slave.DataStore.CoilDiscretes;
                bool rValue = false;
                if (edtRegValue.Text == "1" || edtRegValue.Text == "true")
                {
                    rValue = true;
                }
                if (flag == 0)
                    register[rAddr] = rValue;
                return register[rAddr].ToString();
            }
            else if (cbTypeRegister.SelectedIndex == 3)
            {
                var register = Slave.DataStore.InputDiscretes;
                bool rValue = false;
                if (edtRegValue.Text == "1" || edtRegValue.Text == "true")
                {
                    rValue = true;
                }
                if (flag == 0)
                    register[rAddr] = rValue;
                return register[rAddr].ToString();
            }*/
            return "error";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString() == "Запустить")
            {
                btnStart.Content = "Остановить";
                Listener = new TcpListener(IPAddress.Parse(objectInfo.sIPAdr), int.Parse(objectInfo.sPortTCP));
                Listener.Start();
                Slave = ModbusTcpSlave.CreateTcp(Constans.slaveID, Listener);
                master = ModbusIpMaster.CreateIp((Modbus.IO.IStreamResource)Slave.Masters[0].Client);
                Slave.Listen();
                
                btnUpdateData.IsEnabled = true;

                lblStatus.Content = "Статус: В работе";
                timer.Tick += new EventHandler(timerTick);
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.Start();
            }
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

            //  int rAddr = int.Parse(edtRegAddr.Text);

            //  RegisterType(rAddr, 0);
            dtRegValues.IsEnabled = false;
            dtRegValues.Visibility = Visibility.Collapsed;
            UpdateDataGrid();
        }

        private void timerTick(object sender, EventArgs e)
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


        private void cbTypeRegister_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 0 - H; 1 - I; 2 - C; 3 - DI 
            if (objectInfo != null)
            {
                int countRecords = int.Parse(objectInfo.sStep);
                int iStartValue = int.Parse(objectInfo.sStartValue);
                ushort uStartValue = ushort.Parse(objectInfo.sStartValue);
                ushort uStep = ushort.Parse(objectInfo.sStep);

                if (cbTypeRegister.SelectedIndex == 0)
                {
                    //вывести на редактирование рег.адрес и текущие значения  
                    ushort[] registersHolding = master.ReadHoldingRegisters(Constans.slaveID, uStartValue, uStep);

                    for (int i = iStartValue; i < countRecords + iStartValue; i++)
                    {
                        DataNewRegValues item = new DataNewRegValues(i.ToString(), registersHolding[i].ToString(), "");
                        collectionRegValues.Add(item);
                    }
                }
                else if (cbTypeRegister.SelectedIndex == 1)
                {
                    //12345
                }
                else if (cbTypeRegister.SelectedIndex == 2)
                {
                    bool[] registersCoil = master.ReadCoils(Constans.slaveID, uStartValue, uStep);

                    for (int i = iStartValue; i < countRecords + iStartValue; i++)
                    {
                        DataNewRegValues item = new DataNewRegValues(i.ToString(), registersCoil[i].ToString(), "");
                        collectionRegValues.Add(item);
                    }

                }
                else if (cbTypeRegister.SelectedIndex == 3)
                {
                    //true
                }
            }
        }

        private void edtCountNewValues_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
            dtRegValues.IsEnabled = true;
            dtRegValues.Visibility = Visibility.Visible;
            /*
            //генерация нужного количества строк
            int countRowNewValues = 0;
            string s = "";
            collectionRegValues.Clear(); 
            if (edtCountNewValues.Text != "" && int.TryParse(edtCountNewValues.Text, out countRowNewValues))
            {               
                for (int i = 0; i < countRowNewValues; i++)
                {
                    DataNewRegValues item = new DataNewRegValues(s,s);
                    collectionRegValues.Add(item);
                }
            }*/
        }
    }
}