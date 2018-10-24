using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XwMaxLib.Data;
using XwMaxLib.Extensions;

namespace UnitTests
{
    [TestClass]
    public class Extensions
    {
        [TestMethod]
        public void ProcessException()
        {
            string msg = "";

            //Test XwDbCommand Exception
            try
            {
                using (XwDbCommand sql = new XwDbCommand("Data Source=:memory:;Version=3;New=True;", "Data.SQLite"))
                {
                    sql.ExecuteTX("SELECT test");
                }
            }
            catch (Exception ex)
            {
                msg += "\n\n##################################################################################################";
                msg += ex.ProcessException();
            }
            
            //Test File Exception
            try
            {
                File.ReadAllText("no file");
            }
            catch (Exception ex)
            {
                msg += "\n\n##################################################################################################";
                msg += ex.ProcessException();
            }


            Trace.Write(msg); //check output
            Assert.IsFalse(condition: msg.IsEmpty());
        }
    }
}
