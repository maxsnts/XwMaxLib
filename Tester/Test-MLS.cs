using NailGun.Objects;
using System;
using XwMaxLib.Diagnostics;
using XwMaxLib.DNS;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuMLS()
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
                        TEST_MLS();
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
        static public void TEST_MLS()
        {
            Profiler profiler = new Profiler();
            profiler.Setup(true, 0, "Test");

            //warmup
            MLS wmls = new MLS();
            XwMLS wxwmls = new XwMLS();

            profiler.Start("NEW");
            for (long i = 0; i < 50000; i++)
            {
                XwMLS mls = new XwMLS("[PT-PT]String de Teste #[/PT-PT][EN-GB]Test String #[/EN-GB][FR-FR]chaîne de test #[/FR-FR][ES-ES]cadena de prueba #[/ES-ES]");
                string PTPT = mls.GetTranslation("PT-PT");
                string ENGB = mls.GetTranslation("EN-GB");
                string FRFR = mls.GetTranslation("FR-FR");
                string ESES = mls.GetTranslation("ES-ES");
                mls.SetTranslation("PT-PT", PTPT.Replace("#", i.ToString()));
                mls.SetTranslation("EN-GB", PTPT.Replace("#", i.ToString()));
                mls.SetTranslation("FR-FR", PTPT.Replace("#", i.ToString()));
                mls.SetTranslation("ES-ES", PTPT.Replace("#", i.ToString()));
                string s = mls;
            }
            profiler.Stop("NEW");

            
            profiler.Start("OLD");
            for (long i = 0; i < 50000; i++)
            {
                MLS mls = new MLS("[PT-PT]String de Teste #[/PT-PT][EN-GB]Test String #[/EN-GB][FR-FR]chaîne de test #[/FR-FR][ES-ES]cadena de prueba #[/ES-ES]");
                string PTPT = mls.GetTranslation("PT-PT");
                string ENGB = mls.GetTranslation("EN-GB");
                string FRFR = mls.GetTranslation("FR-FR");
                string ESES = mls.GetTranslation("ES-ES");
                mls.SetTranslation("PT-PT", PTPT.Replace("#", i.ToString()));
                mls.SetTranslation("EN-GB", PTPT.Replace("#", i.ToString()));
                mls.SetTranslation("FR-FR", PTPT.Replace("#", i.ToString()));
                mls.SetTranslation("ES-ES", PTPT.Replace("#", i.ToString()));
                string s = mls;
            }
            profiler.Stop("OLD");
            

            Console.Write(profiler.Print());
        }

    }
}
