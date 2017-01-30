using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace SerialComms
{
    class Program
    {
        static String message = "";
        static List<String> archive = new List<String>();

        static void Main(string[] args)
        {

            SerialPort port = new SerialPort();
            port.PortName = "COM9";
            port.BaudRate = 9600;
            port.Open();

            port.Write("+++");
            Thread.Sleep(1100);
            port.WriteLine("ATID 3304, CH C, CN");
            Thread.Sleep(1100);
            port.DiscardInBuffer();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (true)
            {
                Console.Write("> ");

                message = Console.ReadLine();
                if(message == "printarchive")
                {
                    Console.WriteLine();
                    for(int i = 0; i < archive.Count; i++)
                    {
                        Console.WriteLine(archive[i]);

                    }

                    Console.WriteLine();
                    continue;
                }
                archive.Add(message);

                port.DiscardOutBuffer();
                port.WriteLine(message);

                

            }



        }

        static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            message = port.ReadExisting();
            archive.Add(message);
            Console.WriteLine();
            Console.WriteLine("*" + message);
            Console.Write("> ");
        }

    }

}

