using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RGBLed
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int switcher = 0;
        private GpioPin[] rgbPins;

        private GpioPin buzzerPin, buzzerPin2, fireSwitch, switchPin, forwardSwitchPin, redPin, greenPin, bluePin, noPin;

        DispatcherTimer timer;

        TimeSpan interval = TimeSpan.FromSeconds(1);

        public MainPage()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

            InitGpio();

            timer.Start();
        }

        ~MainPage()
        {
            var x = 0;
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();

            redPin = gpio.OpenPin(4);
            greenPin = gpio.OpenPin(27);
            bluePin = gpio.OpenPin(22);
            switchPin = gpio.OpenPin(13);
            forwardSwitchPin = gpio.OpenPin(26);

            noPin = gpio.OpenPin(5);
            noPin.SetDriveMode(GpioPinDriveMode.Output);

            buzzerPin = gpio.OpenPin(24);
            buzzerPin.SetDriveMode(GpioPinDriveMode.Output);
            buzzerPin2 = gpio.OpenPin(25);
            buzzerPin2.SetDriveMode(GpioPinDriveMode.Output);

            rgbPins = new GpioPin[] { redPin, greenPin, bluePin, noPin };

            fireSwitch = gpio.OpenPin(23);
            fireSwitch.SetDriveMode(GpioPinDriveMode.Input);

            fireSwitch.ValueChanged += FireSwitch_ValueChanged;

            SetRBGPins(GpioPinValue.Low);

            switchPin.SetDriveMode(GpioPinDriveMode.Input);
            forwardSwitchPin.SetDriveMode(GpioPinDriveMode.Output);

            redPin.SetDriveMode(GpioPinDriveMode.Output);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
            bluePin.SetDriveMode(GpioPinDriveMode.Output);

            switchPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            switchPin.ValueChanged += SwitchPin_ValueChanged;
        }

        bool asdf = true;

        private void Timer_Tick(object sender, object e)
        {
            if (asdf)
            {
                buzzerPin.Write(GpioPinValue.High);
                buzzerPin2.Write(GpioPinValue.Low);
            }
            else
            {
                buzzerPin.Write(GpioPinValue.Low);
                buzzerPin2.Write(GpioPinValue.High);
            }
            asdf = !asdf;
        }

        bool gone = false;

        private void FireSwitch_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                InvertRBGPins();
                fireSwitch.ValueChanged -= FireSwitch_ValueChanged;
                gone = true;
            }
        }

        private void SwitchPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                SetRBGPins(GpioPinValue.Low);
                
                rgbPins[switcher++ % rgbPins.Length].Write(GpioPinValue.High);

                if(gone)
                    fireSwitch.ValueChanged += FireSwitch_ValueChanged;
                gone = false;
            }
        }

        private void SetRBGPins(GpioPinValue value)
        {
            foreach (var pin in rgbPins)
                pin.Write(value);
            
        }

        private void InvertRBGPins()
        {
            foreach(var pin in rgbPins)
            {
                pin.Write(pin.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High);
            }
        }
    }
}
