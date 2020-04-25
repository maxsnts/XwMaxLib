using System;
using XwMaxLib.Data;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static void Main(string[] args)
        {
            while(true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- MaxLib Test ---------------------------");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Test XwDbCommand");
                Console.WriteLine("2 - Test DNS");
                Console.WriteLine("3 - Test Profiler");
                Console.WriteLine("4 - Test Crypto");
                Console.WriteLine("5 - Test Rectangle");
                Console.WriteLine("6 - Test MLS");
                Console.WriteLine("7 - Test XwConcurrentFile");
                Console.ResetColor();
                Console.Write("Select option: ");
                string option = Console.ReadLine().Trim();
                Console.Clear();
                Console.WriteLine("----------------------------- Menu ------------------------------");
                try
                {
                    switch (option)
                    {
                        case "0":
                            return;
                        case "1":
                            MenuXwDbCommand();
                            break;
                        case "2":
                            MenuDNS();
                            break;
                        case "3":
                            MenuProfiler();
                            break;
                        case "4":
                            MenuCrypto();
                            break;
                        case "5":
                            MenuRectangle();
                            break;
                        case "6":
                            MenuMLS();
                            break;
                        case "7":
                            MenuXwConcurrentFile();
                            break;
                        default:
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Unknown option");
                                Console.ReadKey();
                            }
                            break;
                    }
                }
                catch (XwDbException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ReadKey();
                }
            }
        }
    }
}
