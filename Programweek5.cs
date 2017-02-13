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
    
        static String inmessage = "";
        static String outmessage = "";
        static List<String> archive = new List<String>();
        static SerialPort port;
        //static void send(string message);

        static void Main(string[] args)
        {

            port = new SerialPort();
            port.PortName = "COM7";
            port.BaudRate = 9600;
            port.Open();

            port.Write("+++");
            Thread.Sleep(1100);
            port.WriteLine("ATID 3304, CH C, CN");
            Thread.Sleep(9000);
            //port.DiscardInBuffer();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (true)
            {
                Console.WriteLine("> ");

                outmessage = Console.ReadLine();
                /*if (outmessage == "printarchive")
                {
                    Console.WriteLine();
                    for (int i = 0; i < archive.Count; i++)
                    {

                        Console.WriteLine(archive[i]);

                    }

                    Console.WriteLine();
                    continue;
                }
                else
                {
                    archive.Add(outmessage);*/
                // outmessage += "!";
                //port.DiscardOutBuffer();
                send(outmessage);
                //}

            }

        }

        static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            inmessage += port.ReadLine();
            /*if (inmessage.EndsWith("!"))
            {
                inmessage = inmessage.Substring(0, inmessage.Length - 1);
                archive.Add(inmessage);
                Console.WriteLine();*/
                Console.WriteLine("*" + inmessage);
                inmessage = "";
                Console.Write("> ");
            //}


        }

        static void send(string message)
        {
            port.WriteLine(outmessage);
        }
    }

}