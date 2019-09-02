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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;

//maine
using System.IO.Ports;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace COMPORT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort port;
        public delegate void Delegat1();
        public Delegat1 moj_del1;
        private Thread myThread;

        public MainWindow()
        {
            InitializeComponent();
            port = new SerialPort();
            //Timeouty
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;

            Opcje.Loaded += new RoutedEventHandler(Opcje_enter);
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            moj_del1 = new Delegat1(WypiszOdebrane);
            tbCommunication.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        void Opcje_enter(object sender, EventArgs e)
        {
            this.cbPortName.Items.Clear();
            this.cbParity.Items.Clear();
            this.cbStop.Items.Clear();

            foreach (String s in SerialPort.GetPortNames()) this.cbPortName.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(Parity))) this.cbParity.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(StopBits))) this.cbStop.Items.Add(s);

            //name actualization
            cbPortName.Text = port.PortName.ToString();
            cbBaudrate.Text = port.BaudRate.ToString();
            cb_databit.Text = port.DataBits.ToString();
            cbParity.Text = port.Parity.ToString();
            cbStop.Text = port.StopBits.ToString();
            
        }
        private void Button_Default_Click(object sender, RoutedEventArgs e)
        {
            this.cbPortName.Items.Clear();
            this.cbParity.Items.Clear();
            this.cbStop.Items.Clear();

            foreach (String s in SerialPort.GetPortNames()) this.cbPortName.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(Parity))) this.cbParity.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(StopBits))) this.cbStop.Items.Add(s);

            this.cbPortName.Text = "COM3";
            this.cbBaudrate.Text = "9600";
            this.cb_databit.Text = "8";
            this.cbParity.Text = "None";
            this.cbStop.Text = "One";

        }

        private void Button_Ods_Click(object sender, RoutedEventArgs e)
        {
            this.cbPortName.Items.Clear();
            this.cbParity.Items.Clear();
            this.cbStop.Items.Clear();

            foreach (String s in SerialPort.GetPortNames()) this.cbPortName.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(Parity))) this.cbParity.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(StopBits))) this.cbStop.Items.Add(s);

            //name actualization
            cbPortName.Text = port.PortName.ToString();
            cbBaudrate.Text = port.BaudRate.ToString();
            cb_databit.Text = port.DataBits.ToString();
            cbParity.Text = port.Parity.ToString();
            cbStop.Text = port.StopBits.ToString();
        }
        
        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (port.IsOpen)
            {
                port.Close();
                System.Windows.MessageBox.Show("Connection Lost");
            }
            else
            {
                try
                {
                    port.PortName = this.cbPortName.Text;
                    port.BaudRate = Int32.Parse(this.cbBaudrate.Text);
                    port.DataBits = Int32.Parse(this.cb_databit.Text);
                    port.Parity = (Parity)Enum.Parse(typeof(Parity), this.cbParity.Text);
                    port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.cbStop.Text);

                    port.Open();
                    System.Windows.MessageBox.Show("Connection OK");


                }
                catch (Exception exc)
                {
                    System.Windows.MessageBox.Show("Blad polaczenie: \n" + exc.Message);
                }
            }
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            if(port.IsOpen)
            {
                int numericUDVal = Int32.Parse(this.cbNumeric.Text);
                DodajText(tbCommunication,"Send data: " + numericUDVal.ToString("X") + "\n", System.Drawing.Color.Black);
                Byte[] send = { (Byte)numericUDVal };
                port.Write(send, 0, 1);
            }
            else System.Windows.Forms.MessageBox.Show("Aby wysłać bajt musisz ustanowić połączenie");
        }


        private void DodajText(System.Windows.Controls.TextBox TextBox, string Text, System.Drawing.Color Color)
        {
            var StartIndex = TextBox.Text.Length;
            TextBox.AppendText(Text);
            var EndIndex = TextBox.Text.Length;
            TextBox.Select(StartIndex, EndIndex - StartIndex);  
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            tbCommunication.Text = "";
        }

        private void WypiszOdebrane()
        {
            DodajText(tbCommunication, "Read data: "+port.ReadByte().ToString("X") + "\n", System.Drawing.Color.Blue);
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            myThread = new Thread(new ThreadStart(ThreadFunction));
            myThread.Start();
        }
        private void ThreadFunction()
        {
            MyThreadClass myThreadClassObject = new MyThreadClass(this);
            myThreadClassObject.Run();
        }

    }
    public class MyThreadClass
    {
        MainWindow mainWindow1;
        public MyThreadClass(MainWindow mainWind)
        {
            mainWindow1 = mainWind;
        }
        
        public void Run()
        {
            mainWindow1.tbCommunication.Dispatcher.Invoke(mainWindow1.moj_del1);
        }
    }

}
