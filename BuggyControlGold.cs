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

        private enum OperationMode { Manual, Bronze, Silver, Gold };
        static OperationMode mode;
        static int buggy1rounds;
        static int buggy2rounds;
        static int goldtarget;

        static void Main(string[] args)
        {
            track = new Track();

            mode = OperationMode.Manual;
            buggy1rounds = 0;
            buggy2rounds = 0;
            goldtarget = 0;

            Console.Title = "Z4 Buggy Control";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();

            port = new SerialPort();
            port.PortName = "COM3";
            port.BaudRate = 9600;
            port.Open();

            if (XBee)
            {
                port.Write("+++");
                Thread.Sleep(1100);
                port.WriteLine("ATID 3304, CH C, CN");

                const double duration = 9;
                const double T = 0.25;

                Console.WriteLine();
                Console.WriteLine(" Group Z4 Buggy Control (c) 2017");
                Console.WriteLine();
                Console.Write(" Loading: ");
                Console.ForegroundColor = ConsoleColor.Blue;

                for (int i = 0; i < duration / T; i++)
                {
                    Thread.Sleep((int)(T * 1000));
                    Console.Write("█");
                }

                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            port.DiscardInBuffer();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (true)
            {
                Console.Write(">");

                String outmessage = Console.ReadLine();

                if (outmessage == "exit")
                {
                    port.Close();
                    return;
                }
                else if (outmessage == "clear")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                }
                else if (outmessage == "print archive")
                {
                    Console.WriteLine();

                    for (int i = 0; i < archive.Count; i++)
                    {
                        Console.WriteLine(archive[i]);
                    }

                }
                else if (outmessage == "Connect Buggy 1")
                {
                    send(0, "Buggy is 1");
                }
                else if (outmessage == "Connect Buggy 2")
                {
                    send(0, "Buggy is 2");
                }
                else if (outmessage == "Start Bronze")
                {
                    mode = OperationMode.Bronze;
                    send(1, "Run");
                }
                else if (outmessage == "Start Silver")
                {
                    mode = OperationMode.Silver;

                    track.setSectionForBuggy(0, 4);
                    track.setOrientationOfBuggy(0, BuggyOrientation.Clockwise);
                    track.setSectionForBuggy(1, 4);
                    track.setOrientationOfBuggy(1, BuggyOrientation.CounterClockwise);

                    send(1, "Run");
                }
                else if (outmessage == "Start Gold")
                {
                    Console.Write("Number of Rounds: ");
                    String nstr = Console.ReadLine(); 
                    int n = 0;
                    if (Int32.TryParse(nstr, out n) && n > 0)
                    {
                        mode = OperationMode.Gold;
                        goldtarget = n;

                        track.setSectionForBuggy(0, 4);
                        track.setOrientationOfBuggy(0, BuggyOrientation.Clockwise);
                        track.setSectionForBuggy(1, 4);
                        track.setOrientationOfBuggy(1, BuggyOrientation.CounterClockwise);

                        send(1, "Run");
                    }
                    else
                    {
                        Console.WriteLine("invalid input!");
                    }
                }
                else if (outmessage.StartsWith("0: "))
                {
                    send(0, outmessage.Substring(3));
                }
                else if (outmessage.StartsWith("1: "))
                {
                    send(1, outmessage.Substring(3));
                }
                else if (outmessage.StartsWith("2: "))
                {
                    send(2, outmessage.Substring(3));
                }
                else if (outmessage.StartsWith("X: "))
                {
                    send(-1, outmessage.Substring(3));
                }
            }
        }

        static void DataReceivedHandler(object sender,
                                        SerialDataReceivedEventArgs e)
        {
            String inmessage = port.ReadLine();

            archive.Add(inmessage);

            Console.WriteLine();
            Console.WriteLine(inmessage);

            if ('0' > inmessage[0] || '9' < inmessage[0])
            {
                Console.Write(">");
                return;
            }

            int buggy = Int32.Parse(inmessage.Substring(0, 1));
            inmessage = inmessage.Substring(3, inmessage.Length - 4).ToLower();

            if (inmessage == "buggy turned on")
            {
                BuggyMovement movement = track.getMovementOfBuggy(buggy - 1);
                double speed = track.getSpeedOfBuggy(buggy - 1);

                if (movement == BuggyMovement.Stopped)
                {
                    send(buggy, "stop");
                }
                else if (movement == BuggyMovement.FollowingLine)
                {
                    send(buggy, "run");
                }
                else if (movement == BuggyMovement.TurningRight)
                {
                    send(buggy, "turn right");
                }
                else if (movement == BuggyMovement.TurningLeft)
                {
                    send(buggy, "turn left");
                }
                else if (movement == BuggyMovement.Rotating)
                {
                    send(buggy, "rotate left");
                }

                send(buggy, "full power");
                for (int i = 0; i < (1.0 - speed) / 0.1; i++)
                    send(buggy, "reduce power");

            }
            else if (inmessage == "buggy id set to 1")
            {
                track.setOrientationOfBuggy(0, BuggyOrientation.Clockwise);
                track.setSectionForBuggy(0, 3);

                Console.WriteLine("Buggy 1 connected!");
            }
            else if (inmessage == "buggy id set to 2")
            {
                track.setOrientationOfBuggy(1, BuggyOrientation.CounterClockwise);
                track.setSectionForBuggy(1, 3);

                Console.WriteLine("Buggy 2 connected!");
            }
            else if (inmessage == "buggy running")
            {
                track.setMovementOfBuggy(buggy - 1, BuggyMovement.FollowingLine);
            }
            else if (inmessage == "buggy stopping")
            {
                track.setMovementOfBuggy(buggy - 1, BuggyMovement.Stopped);
            }
            else if (inmessage == "buggy turning right")
            {
                track.setMovementOfBuggy(buggy - 1, BuggyMovement.TurningRight);
            }
            else if (inmessage == "buggy turning left")
            {
                track.setMovementOfBuggy(buggy - 1, BuggyMovement.TurningLeft);
            }
            else if (inmessage == "buggy rotating left")
            {
                track.setMovementOfBuggy(buggy - 1, BuggyMovement.Rotating);
            }
            else if (inmessage == "buggy reducing power")
            {
                track.setSpeedOfBuggy(buggy - 1, track.getSpeedOfBuggy(buggy - 1) - 0.1);
            }
            else if (inmessage == "buggy increasing power")
            {
                track.setSpeedOfBuggy(buggy - 1, track.getSpeedOfBuggy(buggy - 1) + 0.1);
            }
            else if (inmessage == "buggy half power")
            {
                track.setSpeedOfBuggy(buggy - 1, 0.5);
            }
            else if (inmessage == "buggy full power")
            {
                track.setSpeedOfBuggy(buggy - 1, 1.0);
            }
            else if (inmessage == "passed gantry")
            {
                track.setSectionForBuggy(buggy - 1, track.getNextSectionForBuggy(buggy - 1, false));
                printTrack();
            }
            else if (inmessage == "buggy parked")
            {
                track.setSectionForBuggy(buggy - 1, track.getNextSectionForBuggy(buggy - 1, true));
                printTrack();

                if (mode == OperationMode.Bronze)
                {
                    Console.WriteLine("Bronze challenge complete!");
                }
                else if (mode == OperationMode.Silver)
                {
                    if (buggy == 1)
                    {
                        Console.WriteLine("Silver challenge complete!");
                    }
                    else if (buggy == 2)
                    {
                        send(1, "leave gantry");
                    }
                }
                else if (mode == OperationMode.Gold)
                {
                    if (buggy == 1)
                    {
                        Console.WriteLine("Gold challenge complete!");
                    }
                    else if (buggy == 2)
                    {
                        send(1, "leave gantry");
                    }
                }

            }
            else if (inmessage.StartsWith("detected gantry"))
            {
                int gantry = Int32.Parse(inmessage.Substring(16));
                track.setGantryForBuggy(buggy - 1, gantry - 1);
                printTrack();

                if (mode == OperationMode.Bronze)
                {
                    if (gantry == 2)
                    {
                        buggy1rounds++;
                    }
                    if (buggy1rounds < 2)
                    {
                        Thread.Sleep(500);
                        send(1, "leave gantry");
                    }
                    else
                    {
                        Thread.Sleep(500);
                        send(1, "park right");
                    }
                }
                else if (mode == OperationMode.Silver)
                {
                    if (buggy == 1)
                    {
                        if (gantry == 0)
                        {
                            gantry = track.getNextGantryForBuggy(0, false);
                        }

                        if (gantry == 1)
                        {
                            Thread.Sleep(500);
                            send(1, "leave gantry");
                        }
                        else if (gantry == 2)
                        {
                            buggy1rounds++;
                            if (buggy1rounds == 1)
                            {
                                Thread.Sleep(500);
                                send(1, "leave gantry");
                            }
                            else if (buggy1rounds == 2)
                            {
                                Thread.Sleep(500);
                                send(1, "park right");
                            }
                        }
                        else if (gantry == 3)
                        {
                            send(2, "run");
                        }
                    }
                    else if (buggy == 2)
                    {
                        if (gantry == 0)
                        {
                            gantry = track.getNextGantryForBuggy(1, false);
                        }

                        if (gantry == 2)
                        {
                            Thread.Sleep(500);
                            send(2, "leave gantry");
                        }
                        else if (gantry == 1)
                        {
                            Thread.Sleep(500);
                            send(2, "park left");
                        }
                    }
                }
                else if (mode == OperationMode.Gold)
                {
                    if (buggy == 1)
                    {
                        if (gantry == 0)
                        {
                            gantry = track.getNextGantryForBuggy(0, false);
                        }

                        if (gantry == 1)
                        {
                            Thread.Sleep(500);
                            send(1, "leave gantry");
                        }
                        else if (gantry == 2)
                        {
                            buggy1rounds++;
                            Console.WriteLine("Buggy 1 completed round " + buggy1rounds);
                            if (buggy1rounds < goldtarget)
                            {
                                Thread.Sleep(500);
                                send(1, "leave gantry");
                            }
                            else
                            {
                                Thread.Sleep(500);
                                send(1, "park right");
                            }
                        }
                        else if (gantry == 3)
                        {
                            send(2, "run");
                        }
                    }
                    else if (buggy == 2)
                    {
                        if (gantry == 0)
                        {
                            gantry = track.getNextGantryForBuggy(1, false);
                        }

                        if (gantry == 2)
                        {
                            Thread.Sleep(500);
                            send(2, "leave gantry");
                        }
                        else if (gantry == 1)
                        {
                            buggy2rounds++;
                            Thread.Sleep(500);
                            send(2, "park left");
                        }
                    }
                }
            }
            else if (inmessage == "obstacle detected")
            {
                track.setBuggyHasObstacle(buggy - 1, true);
            }
            else if (inmessage == "obstacle gone")
            {
                track.setBuggyHasObstacle(buggy - 1, false);
            }

            Console.Write(">");
        }

        static void send(int buggy, String message)
        {
            archive.Add(buggy + ": " + message);
            port.DiscardOutBuffer();
            if (buggy == -1)
            {
                port.WriteLine("X: " + message);
            }
            else
            {
                port.WriteLine(buggy + ": " + message);
            }
        }

        static void printTrack()
        {
            Console.WriteLine();
            Console.Write(track.getMap());
            Console.Write("> ");
        }
    }

}

