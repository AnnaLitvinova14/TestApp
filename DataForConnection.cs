using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp
{
    public class DataForConnection
    {
        public string sIPAdr { get; set; }
        public string sPortTCP { get; set; }
        public string sClient { get; set; }
        public string sCountNewValues { get; set; }
        public string sStartValue { get; set; }
        public string sStep { get; set; }

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

    public class DataNewRegValues
    {
        public DataNewRegValues(string RegAddress, string RegValue)
        {
            this.RegAddress = RegAddress;
            this.RegValue = RegValue;
        }
        public string RegAddress { get; set; }
        public string RegValue { get; set; }
    }


}