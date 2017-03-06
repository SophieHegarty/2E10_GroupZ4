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
        static Track track;

        static void Main(string[] args)
        {
            track = new Track();
            track.setOrientationForBuggy(0, BuggyOrientation.Clockwise);
            track.setSectionForBuggy(0, 1);

            port = new SerialPort();
            port.PortName = "COM7";
            port.BaudRate = 9600;
            port.Open();

            if (XBee)
            {
                port.Write("+++");
                Thread.Sleep(1100);
                port.WriteLine("ATID 3304, CH C, CN");
                Thread.Sleep(9000);
            }

            port.DiscardInBuffer();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (true)
            {
                Console.WriteLine("> ");

                String outmessage = Console.ReadLine();

                if (outmessage == "print archive")
                {
                    Console.WriteLine();

                    for (int i = 0; i < archive.Count; i++)
                    {
                        Console.WriteLine(archive[i]);
                    }

                }else if(outmessage == "Start Bronze")
                {
                    send("Run");
                }
                else
                {
                    send(outmessage);
                }
            }
        }

        static void DataReceivedHandler(object sender,
                                        SerialDataReceivedEventArgs e)
        {
            String inmessage = port.ReadLine();

            archive.add(inmessage);

            if(inmessage == "Detected Gantry 1")
            {
                track.setGantryForBuggy(0, 0);
                printTrack();
                send("leave gantry");
                track.setSectionForBuggy(0, track.getNextSectionForBuggy(0,false));
                printTrack();

            }else if(inmessage == "Detected Gantry 2")
            {
                
                track.setGantryForBuggy(0, 1);
                printTrack();

                send("leave gantry");
                track.setSectionForBuggy(0, track.getNextSectionForBuggy(0, false));
                printTrack();
            }
            else if(inmessage == "Detected Gantry 3")
            {

                track.setGantryForBuggy(0, 2);
                printTrack();
                send("leave gantry");
                track.setSectionForBuggy(0, track.getNextSectionForBuggy(0, false));
                printTrack();
            }

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

        static void printTrack()
        {
            Console.WriteLine();
            Console.Write(track.getMap());
            Console.Write("> ");
        }
    }

}
