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




namespace WpfApp
{
    public partial class MainWindow : Window
    {        
        ModbusTcpSlave Slave;
        TcpListener Listener;

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            DataForConnection objectInfo = new DataForConnection
            {
                sIPAdr = Constans.sIP_default,
                sPortTCP = Constans.sTCP_default,
                sClient = Constans.sClient_default,
                sCountNewValues = Constans.sCountNewValues_default,
                sStartValue = Constans.sStartValue_default,
                sStep = Constans.sStep_default
            };

            this.DataContext = objectInfo;
            List<DataNewRegValues> NewRegValue = new List<DataNewRegValues>(int.Parse(objectInfo.sCountNewValues));
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

          /*  // 0 - H; 1 - I; 2 - C; 3 - DI 
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
            }*/
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
                for (int i = iClient; i < Slave.Masters.Count; i++)
                {
                    lblStep.Content = i.ToString();
                    
                    Slave.Masters[i].Client.Disconnect(false);
                }
            }
            UpdateDataGrid();
        }


        private void cbTypeRegister_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 0 - H; 1 - I; 2 - C; 3 - DI 
            if (cbTypeRegister.SelectedIndex == 0) 
            {
                //12345
            }
            else if (cbTypeRegister.SelectedIndex == 1)
            {
                //12345
            }
            else if (cbTypeRegister.SelectedIndex == 2)
            {
                //true
            }
            else if (cbTypeRegister.SelectedIndex == 3)
            {
                //true
            }
        }

        private void edtCountNewValues_SelectionChanged(object sender, RoutedEventArgs e)
        {
            dtRegValues.IsEnabled = true;
            dtRegValues.Visibility = Visibility.Visible;
            //генерация нужного количества строк
            int countRowNewValues = 0;
            dtRegValues.Items.Clear();
            if (edtCountNewValues.Text != "" && int.TryParse(edtCountNewValues.Text, out countRowNewValues))
            {               
                for (int i = 0; i < countRowNewValues; i++)
                {
                    dtRegValues.Items.Add("");
                }
            }
        }
    }
}