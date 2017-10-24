using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Diagnostics;
using DFrobotWindowIoTTempelate;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace DFRobotWindowsIoTTempelate
{
    public sealed partial class MainPage : Page
    {
        UsbSerial usb;                          //Handle the USB connction
        RemoteDevice arduino;                   //Handle the arduino
        private DispatcherTimer readTemperatureTimer;     //Timer for the LED to blink every one second
        private DispatcherTimer readKeyboardTimer;     
        private const string LM35_PIN = "A5";
        private const string MOISTURE_PIN = "A0";
        private const string KEYBOARD_PIN = "A1";

       
        Socket _dataSocket;
        ServiceSocket _serviceSocket;

        double temperature = 0;
        int moisturevalue = 0;
        ushort Keyboard = 0;
        public static byte[] getByte = new byte[1024];

        SmartService service = null;
        public MainPage()
        {
            this.InitializeComponent();

             usb = new UsbSerial("VID_2341", "PID_8036");

            //Arduino RemoteDevice Constractor via USB.
             arduino = new RemoteDevice(usb);
            //Add DeviceReady callback when connecting successfully
             arduino.DeviceReady += onDeviceReady;

            //Baudrate on 57600 and SerialConfig.8N1 is the default config for Arduino devices over USB
             usb.begin(57600, SerialConfig.SERIAL_8N1);




        }
        private void onDeviceReady()
        {
            //After device is ready this function will be evoked.

            //Debug message "Device Ready" will be shown in the "Output" dialog.
            Debug.WriteLine("Device Ready");
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                setup();
            }));
        }

        private void setup()
        {
            //Set the pin mode of the led.
            arduino.pinMode(LM35_PIN, PinMode.ANALOG);
            arduino.pinMode(MOISTURE_PIN, PinMode.ANALOG);
            arduino.pinMode(KEYBOARD_PIN, PinMode.ANALOG);

            //Set the timer to schedule every 500 ms.
            readTemperatureTimer = new DispatcherTimer();
            readTemperatureTimer.Interval = TimeSpan.FromMilliseconds(5000);
            readTemperatureTimer.Tick += readTemperature;
            readTemperatureTimer.Start();

            readKeyboardTimer = new DispatcherTimer();
            readKeyboardTimer.Interval = TimeSpan.FromMilliseconds(500);
            readKeyboardTimer.Tick += textKeyboard;
            readKeyboardTimer.Start();

          //  service = new SmartHendler();
            //new Thread(new ThreadStart(serviceStart)).Start();
             new Thread(new ThreadStart(dataGetStart)).Start();
        }

        private void readTemperature(object sender, object e)
        {
            //Read analog value from 0 to 1023, which maps from 0 to 5V
            int temperatureValue = arduino.analogRead(LM35_PIN);
            moisturevalue = arduino.analogRead(MOISTURE_PIN);
            //Convert analog value to temperature.
            temperature = (500.0 * temperatureValue) / 1024.0;

            //Print temperature to Output.
            Debug.WriteLine(temperature);
            Debug.WriteLine(moisturevalue);
        }

        private void textKeyboard(object sender, object e) {
            Keyboard = arduino.analogRead("A1");
            Debug.WriteLine(Keyboard);

            if (850 < Keyboard && Keyboard < 900)
            {
                _serviceSocket = new ServiceSocket("192.168.0.12");
                String task = "CONNECT";
                _serviceSocket.setTask(task);
                _serviceSocket.send();

                Debug.WriteLine("S5");
            }
            else if (700 < Keyboard && Keyboard < 800)
            {
                Debug.WriteLine("S4");
            }
            else if (650 < Keyboard && Keyboard < 700)
            {
                Debug.WriteLine("S3");
            }
            else if (600 < Keyboard && Keyboard < 650)
            {
                Debug.WriteLine("S2");
            }
            else if (550 < Keyboard && Keyboard < 600)
            {
                Debug.WriteLine("S1");
            }
            else {
                Debug.WriteLine("Default");
            }
        }

        public void dataGetStart()
        {

            DataContainer dataContainer = null;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.0.12"), 5001);
            try
            {
                _dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _dataSocket.Connect(ipep);
                while (true) {
                    dataContainer = new DataContainer();
                    dataContainer.Moisture = moisturevalue;
                    dataContainer.Temperature = temperature;

                    var json = JsonConvert.SerializeObject(dataContainer);
                    // String s = temperature.ToString() + ":" + moisturevalue.ToString();
                    _dataSocket.Send(Encoding.UTF8.GetBytes(json));

              }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally {
                _dataSocket.Dispose();
            }
        }

       

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}