using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspberry.IO.GeneralPurpose;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var redLed = ConnectorPin.P1Pin07.Output();
            var gpio = new GpioConnection(redLed);

            gpio.Toggle(redLed);

            Thread.Sleep(10000);
            gpio.Close();
        }
    }
}
