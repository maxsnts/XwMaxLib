using System;
using XwMaxLib.DNS;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuDNS()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- DNS ---------------------------");
                Console.WriteLine("0 - Main Menu");
                Console.WriteLine("1 - List Domains");
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
                        LIST_DOMAINS();
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
        static public void LIST_DOMAINS()
        {
            DnsMsServer dns = new DnsMsServer("10.0.0.74", "nta", "********");
            var zones = dns.ListZones();
            foreach (var zone in zones)
            {
                Console.WriteLine(zone["ContainerName"]);
            }
        }

    }
}
