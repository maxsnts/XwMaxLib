using System;
using System.Threading;
using XwMaxLib.IO;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuXwConcurrentFile()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- MLS ---------------------------");
                Console.WriteLine("0 - Main Menu");
                Console.WriteLine("1 - Test");
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
                        TestXwConcurrentFile();
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
        static public void TestXwConcurrentFile()
        {
            XwConcurrentFile.Clear("c:\\data\\test.log");
            for (int i = 0; i < 3000; i++)
            {
                Console.Write($"{i.ToString()}");
                XwConcurrentFile.Write("c:\\data\\test.log", "Lets hope this solves the problem...\r\n");
                Thread.Sleep(100);
            }
        }
    }
}
