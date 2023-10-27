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




class DataTable
{
    public DataTable(string iAddr, string iValue)
    {
        this.iAddr = iAddr;
        this.iValue = iValue;
    }
    public string iAddr { get; set; }
    public string iValue { get; set; }
}

namespace WpfApp
{
    public partial class MainWindow : Window
    {        
        ModbusTcpSlave Slave;
        TcpListener Listener;

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        string strReg = @"^([0-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-5][0-9][0-9][0-9][0-9]|6[0-5][0-5][0-3][0-5])$";

        public MainWindow()
        {
            InitializeComponent();
            edtClient.Text = "1";
            edtIP.Text = "127.0.0.1";
            edtRegAddr.Text = "2";
            edtTCP.Text = "502";
            edtRegValue.Text = "333";
        }

        private void CheckEdt(int flag)
        {

            if (flag == 0)
            {
                Match match1 = Regex.Match(edtIP.Text, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
                if (!match1.Success)
                {
                    edtIP.Text = "127.0.0.1";
                    MessageBox.Show("Некорректный IP адрес");
                }
            }
            else if (flag == 1)
            {
                Match match = Regex.Match(edtTCP.Text, strReg);
                if (!match.Success)
                {
                    edtTCP.Text = "502";
                    MessageBox.Show("Некорректный порт");
                }
            }
            else if (flag == 2)
            {
                Match match = Regex.Match(edtRegAddr.Text, strReg);
                if (!match.Success)
                {
                    edtRegAddr.Text = "2";
                    MessageBox.Show("Некорректный рег.адрес");
                }
            }
            else if (flag == 3)
            {
                int count = int.Parse(edtStartValue.Text) + int.Parse(edtStep.Text);
                Match match = Regex.Match(edtStartValue.Text, strReg);
                Match match1 = Regex.Match(count.ToString(), strReg);
                if (!match.Success || !match1.Success)
                {
                    edtStartValue.Text = "1";
                    edtStep.Text = "10";
                    MessageBox.Show("Недопустимый диапозон вывода в таблицу");
                }
            }
        }

        private void UpdateDataGrid()
        {
            int length = int.Parse(edtStep.Text);
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
            if (cbTypeRegister.SelectedIndex == 0)
            {
                var register = Slave.DataStore.HoldingRegisters;
                ushort rValue = 0; 
                if (edtRegValue.Text != "") rValue = ushort.Parse(edtRegValue.Text);
                if (flag == 0)
                    register[rAddr] = rValue;
                return register[rAddr].ToString();
            }
            else if (cbTypeRegister.SelectedIndex == 1)
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
            }
            return "error";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            IPAddress IPAdr = IPAddress.Parse(edtIP.Text);
            int PortTCP = int.Parse(edtTCP.Text); 
            if (btnStart.Content.ToString() == "Запустить")
            {
                btnStart.Content = "Остановить";
                Listener = new TcpListener(IPAdr, PortTCP);
                Listener.Start();
                Slave = ModbusTcpSlave.CreateTcp(1, Listener);              
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
            
            int rAddr = int.Parse(edtRegAddr.Text);

            RegisterType(rAddr, 0);

            UpdateDataGrid();
        }

        private void timerTick(object sender, EventArgs e)
        {
            int iClient = int.Parse(edtClient.Text);
            
            if (Slave.Masters.Count > iClient)
            {
                for (int i = iClient; i < Slave.Masters.Count; i++)
                {
                    lblStep.Content = i.ToString();
                    
                    Slave.Masters[i].Client.Disconnect(false);
                }
            }
            UpdateDataGrid();
        }

        private void edtIP_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckEdt(0);
        }

        private void edtTCP_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckEdt(1);
        }

        private void edtRegAddr_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckEdt(2);
        }

        private void edtStartValue_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckEdt(3);
        }
    }
}