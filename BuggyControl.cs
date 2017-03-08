using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace Buggy
{
    class Program
    {
        const bool XBee = true;

        static List<String> archive = new List<String>();
        static SerialPort port;
        static Track track;
        static int round;

        static void Main(string[] args)
        {
            track = new Track();
            track.setOrientationOfBuggy(0, BuggyOrientation.Clockwise);
            track.setSectionForBuggy(0, 3);
            round = 0;

            port = new SerialPort();
            port.PortName = "COM8";
            port.BaudRate = 9600;
            port.Open();

            if (XBee)
            {
                port.Write("+++");
                Thread.Sleep(1100);
                port.WriteLine("ATID 3304, CH C, CN");

                const double duration = 9;
                const double T = 0.25;

                for (int i = 0; i < duration / T; i++)
                {
                    Thread.Sleep((int)(T * 1000));
                    Console.Write("+");
                }

                Console.WriteLine();
                Console.WriteLine("Ready");
            }

            port.DiscardInBuffer();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (true)
            {
                Console.Write(">");

                String outmessage = Console.ReadLine();

                if (outmessage == "print archive")
                {
                    Console.WriteLine();

                    for (int i = 0; i < archive.Count; i++)
                    {
                        Console.WriteLine(archive[i]);
                    }

                }
                else if (outmessage == "exit")
                {
                    port.Close();
                    return;
                }
                else if (outmessage == "Start Bronze")
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

            archive.Add(inmessage);

            if (inmessage.StartsWith("Detected Gantry "))
            {
                int gantry = Int32.Parse(inmessage.Substring(16));

                if (gantry == 0)
                {
                    gantry = track.getNextGantryForBuggy(0, false);
                }
                gantry--;
                if (gantry != track.getGantryForBuggy(0))
                {
                    if (gantry == 1)
                    {
                        round++;
                    }

                    track.setGantryForBuggy(0, gantry);
                    printTrack();
                    if (round < 2)
                    {
                        send("leave gantry");
                        track.setSectionForBuggy(0, track.getNextSectionForBuggy(0, false));
                        printTrack();
                    }
                    else
                    {
                        send("park right");
                        track.setSectionForBuggy(0, track.getNextSectionForBuggy(0, true));
                        printTrack();
                    }

                }


            }
            else if (inmessage == "Buggy Parked")
            {
                Console.WriteLine();
                Console.Write("Bronze Challenge Complete!");
            }

            Console.WriteLine();
            Console.WriteLine("*" + inmessage);
            Console.Write(">");
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
