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
using NeuroSky.ThinkGear;
using System.Diagnostics;
using System.Threading;
namespace MindWaveApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Connector> connectors;
        private  Connector test;
        public MainWindow()
        {
            InitializeComponent();
            connectors = new List<Connector>();
            test = new Connector();
            //Thread thread1 = new Thread(() => PrepareHeadset(new Connector(), "COM32", text, progres1));
            //thread1.Start();
            Thread thread2 = new Thread(() => PrepareHeadset("COM32", Box2, progres2));
            
            thread2.Start();

        }
        void PrepareHeadset(string v_port, TextBox v_text, ProgressBar v_progress)
        {
            Connector v_connector = new Connector();
            v_connector.setBlinkDetectionEnabled(true);
            v_connector.DeviceConnected += delegate(object sender2, EventArgs e2)
            {
                OnDeviceConnected(sender2, e2, v_text, v_progress);
            };
            // v_connector.DeviceFound += new EventHandler(OnDeviceFound);
            v_connector.DeviceConnectFail += new EventHandler(OnDeviceFail);
            v_connector.DeviceValidating += new EventHandler(OnDeviceValidating);
            v_connector.Connect(v_port);
            connectors.Add(v_connector);



            //connector2.DeviceConnected += new EventHandler(OnDeviceConnected);

        }
        //void OnDeviceFound(object sender, EventArgs e)
        //{

        //    Connector.PortEventArgs deviceEventArgs = (Connector.PortEventArgs)e;
        //    Debug.WriteLine("Port was found." + deviceEventArgs.PortName);
        //    connector.Connect(deviceEventArgs.PortName);

        //}
        void OnDeviceConnected(object sender, EventArgs e, TextBox text, ProgressBar v_progress)
        {
            Connector test = (Connector)sender;    
            Connector.DeviceEventArgs deviceEventArgs = (Connector.DeviceEventArgs)e;
            Debug.WriteLine("New Headset Created." + deviceEventArgs.Device.PortName);

            //deviceEventArgs.Device.DataReceived += new EventHandler(OnDataReceived);
            deviceEventArgs.Device.DataReceived += delegate(object sender2, EventArgs e2)
            {
                OnDataReceived(sender2, e2, text, v_progress);
            };

        }
        void OnDataReceived(object sender, EventArgs e, TextBox v_text, ProgressBar v_progress)
        {
            /* Cast the event sender as a Device object, and e as the Device's DataEventArgs */
            Device d = (Device)sender;
            
            Device.DataEventArgs de = (Device.DataEventArgs)e;
            /* Create a TGParser to parse the Device's DataRowArray[] */
            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);
          
            /* Loop through parsed data TGParser for its parsed data... */
            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {
                // See the Data Types documentation for valid keys such
                // as "Raw", "PoorSignal", "Attention", etc.
                if (tgParser.ParsedData[i].ContainsKey("Raw"))
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        v_text.Text = (tgParser.ParsedData[i]["Raw"]).ToString();
                    }));
            
                    //Console.WriteLine("Raw Value:" + tgParser.ParsedData[i]["Raw"]);
                }

                if (tgParser.ParsedData[i].ContainsKey("BlinkStrength"))
                {
                    if (tgParser.ParsedData[i]["BlinkStrength"] > 50)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            Console.WriteLine(square.Fill.ToString());
                            if (square.Fill.ToString() == "#642048FF")
                            {
                                mySolidColorBrush.Color = Color.FromArgb(100, 201, 10, 10);
                            }
                            else
                            {
                                mySolidColorBrush.Color = Color.FromArgb(100, 32, 72, 255);
                            }
                            square.Fill = mySolidColorBrush;
                            //v_progress.Value = 100 - (Double)(tgParser.ParsedData[i]["PoorSignal"]);
                        }));
                    }
                    Console.WriteLine("Blink:" + tgParser.ParsedData[i]["BlinkStrength"]);
                }
                if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
                {               
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        v_progress.Value = 100 - (Double)(tgParser.ParsedData[i]["PoorSignal"]);
                    }));
                     Console.WriteLine("PQ Value:" + tgParser.ParsedData[i]["PoorSignal"]);
                }

                if (tgParser.ParsedData[i].ContainsKey("Attention"))
                {

                    Console.WriteLine("Att Value:" + tgParser.ParsedData[i]["Attention"]);
                }
                if (tgParser.ParsedData[i].ContainsKey("Meditation"))
                {
                    Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);
                }
                if (tgParser.ParsedData[i].ContainsKey("MindWandering"))
                {
                    Console.WriteLine("MindWandering Level:" + tgParser.ParsedData[i]["MindWandering"]);
                }
            }
        }
        void OnDeviceFail(object sender, EventArgs e)
        {
            Console.WriteLine("No devices found! :(");
        }

        void OnDeviceValidating(object sender, EventArgs e)
        {
            Console.WriteLine("Validating: ");

            // heartRateRecovery.enableHeartRateRecovery();
        }
    }
}
