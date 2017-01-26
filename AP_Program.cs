using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP_2E10_Lab1
{
    class Program
    {
        static String message = "";
        static bool isRunning = true;

        static void Main(string[] args)
        {
            while(isRunning)
            {
                Console.Write(">> ");

                message = Console.ReadLine();

                if (message.StartsWith("print: "))
                {
                    string nstr = message.Split(' ')[1];
                    string pstr = message.Substring(8 + nstr.Length);

                    int n = int.Parse(nstr);

                    for (int i = 0; i < n; i++)
                        Console.WriteLine("MSG: " + pstr);
                }
                else if (message.StartsWith("rev-print: "))
                {
                    string nstr = message.Split(' ')[1];
                    string pstr = message.Substring(12 + nstr.Length);

                    int n = int.Parse(nstr);

                    for (int i = 0; i < n; i++)
                    {
                        Console.Write("MSG: ");
                        for (int c = pstr.Length - 1; c >= 0; c--)
                            Console.Write(pstr[c]);
                        Console.WriteLine();
                    }
                }
                else if (message.StartsWith("stop:"))
                {
                    Console.WriteLine("MSG: stop");
                    isRunning = false;
                }
                else
                {
                    Console.WriteLine("MSG: '" + message + "' not valid");
                }
            }

            Console.ReadLine();
        }
    }
}
