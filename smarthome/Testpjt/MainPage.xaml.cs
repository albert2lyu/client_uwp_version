using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Diagnostics;

namespace Testpjt
{
    public sealed partial class MainPage : Page
    {
        UsbSerial usb;                          //Handle the USB connction
        RemoteDevice arduino;                   //Handle the arduino
        private DispatcherTimer textKeyboardTimer;     //Timer for the LED to textKeyboard every one second
        ushort Keyboard = 0;       //Pin number of the on Keyboard
        int NUM_KEYS = 5;
        int[] keys_val = new int[5];
        int abc_key_in;
        int key = -1;
        int oldkey = -1;
        public MainPage()
        {
            this.InitializeComponent();
            keys_val[0] = 600;
            keys_val[1] = 650;
            keys_val[2] = 700;
            keys_val[3] = 800;
            keys_val[4] = 900;
            //USB VID and PID of the "Arduino Expansion Shield for Raspberry Pi B+"
            usb = new UsbSerial("VID_2341", "PID_8036");

            //Arduino RemoteDevice Constractor via USB.
            arduino = new RemoteDevice(usb);
            //Add DeviceReady callback when connecting successfully
            arduino.DeviceReady += onDeviceReady;
            textblock.Text = "S1";

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
            arduino.pinMode("A5", PinMode.ANALOG);
            textblock.Text = "START";
            //Set the timer to schedule textKeyboard() every one second.
            textKeyboardTimer = new DispatcherTimer();
            textKeyboardTimer.Interval = TimeSpan.FromMilliseconds(500);
            textKeyboardTimer.Tick += textKeyboard;
            textKeyboardTimer.Start();
        }

        private void textKeyboard(object sender, object e)
        {
            Keyboard = arduino.analogRead("A5");

            key = get_key(Keyboard);

            Debug.WriteLine(key);
            if (key >= 0)
            {
                switch (key)
                {
                    case 0:
                        textblock.Text = "S1";
                        Debug.WriteLine("S1 OK");
                        break;
                    case 1:
                        textblock.Text = "S2";
                        Debug.WriteLine("S2 OK");
                        break;
                    case 2:
                        textblock.Text = "S3";
                        Debug.WriteLine("S3 OK");
                        break;
                    case 3:
                        textblock.Text = "S4";
                        Debug.WriteLine("S4 OK");
                        break;
                    case 4:
                        textblock.Text = "S5";
                        Debug.WriteLine("S5 OK");
                        break;
                    default:
                        textblock.Text = "NONE";
                        Debug.WriteLine("None");
                        break;
                }

            }


        }

        private int get_key(ushort keyboard)
        {
            int k;

            for (k = 0; k < NUM_KEYS; k++)
            {
                if (keyboard < keys_val[k])
                {

                    return k;
                }
            }

            if (k >= NUM_KEYS) k = -1;

            return k;
        }
    }
}


