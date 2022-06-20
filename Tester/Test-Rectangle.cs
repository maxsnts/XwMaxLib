using System;
using XwMaxLib.DNS;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuRectangle()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- Rectangle ---------------------------");
                Console.WriteLine("0 - Main Menu");
                Console.WriteLine("1 - Open Test Form");
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
                        OPEN_TAB_FORM();
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
        static public void OPEN_TAB_FORM()
        {
            Test_Rectangle_Form form = new Test_Rectangle_Form();
            form.ShowDialog();
        }

    }
}
