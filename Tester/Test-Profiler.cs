using XwMaxLib;
using XwMaxLib.Data;
using XwMaxLib.Diagnostics;
using XwMaxLib.DNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuProfiler()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- Profiler ---------------------------");
                Console.WriteLine("0 - Main Menu");
                Console.WriteLine("1 - Profiler");
                Console.ResetColor();
                Console.Write("Select option: ");
                string option = Console.ReadLine().Trim();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--------------------------- Run Test ----------------------------");
                Console.ResetColor();
                switch (option)
                {
                    case "0":
                        return;
                    case "1":
                        PROFILER();
                        break;
                    default:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unknown option");
                        }
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("----------------------------- DONE ------------------------------");
                Console.ReadKey();
            }
        }

        //***********************************************************************************
        static public void PROFILER()
        {
            Profiler profiler = new Profiler();
            profiler.Setup(true, 0, "Title");

            profiler.Start("Item 1");
            for (long i = 0; i < 3000000; i++)
            {
                int N1 = new Random().Next(1, 1000);
                int N2 = new Random().Next(1, 1000);
                int N3 = N1 * N2;
            }
            profiler.Stop("Item 1");

            profiler.Start("Item 2");
            for (long i = 0; i < 30000; i++)
            {
                int N1 = new Random().Next(1, 1000);
                int N2 = new Random().Next(1, 1000);
                int N3 = N1 * N2;
            }
            profiler.Stop("Item 2");

            profiler.Start("Item 3");

            profiler.Start("Item 4");
            Thread.Sleep(1000);
            profiler.Stop("Item 4");

            profiler.Start("Item 4");
            Thread.Sleep(1000);
            profiler.Stop("Item 4");

            profiler.Start("Item 4");
            Thread.Sleep(1000);
            profiler.Stop("Item 4");

            profiler.Start("Item 4");
            Thread.Sleep(1000);
            profiler.Stop("Item 4");

            Console.Write(profiler.Print());

        }

    }
}
