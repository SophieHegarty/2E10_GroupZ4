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
        static const bool XBee = true;

        static List<String> archive = new List<String>();
        static SerialPort port;

        static void Main(string[] args)
        {
            port = new SerialPort();
            port.PortName = "COM7";
            port.BaudRate = 9600;
            port.Open();

            if (XBee) {
                port.Write("+++");
                Thread.Sleep(1100);
                port.WriteLine("ATID 3304, CH C, CN");
                Thread.Sleep(9000);
            }
            
            port.DiscardInBuffer();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (true) {
                Console.WriteLine("> ");

                String outmessage = Console.ReadLine();
               
                if (outmessage == "print archive") {
                    Console.WriteLine();

                    for (int i = 0; i < archive.Count; i++) {
                        Console.WriteLine(archive[i]);
                    }

                }else{
                    send(outmessage);
                }
            }
        }

        static void DataReceivedHandler(object sender,
                                        SerialDataReceivedEventArgs e)
        {
            String inmessage = port.ReadLine();

            archive.add(inmessage);

            Console.WriteLine();
            Console.WriteLine("*" + inmessage);
            Console.Write("> ");
        }

        static void send(String message)
        {
            archive.Add(message);

            port.DiscardOutBuffer();
            port.WriteLine(message);
        }
    }

}
