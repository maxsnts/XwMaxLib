
// Yes yes yes, i should be opening a StreamWriter at the begining and closing at the end 
// instead of using File.AppendAllText... but this is a conscious decision
// I want to be able to write from anywhere with one line: XwConcurrentFile.Write(...)
// That is it, no Open, no Close, just write
// And if i open and close the Stream in the Write function, well that is just the same as this

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;

namespace XwMaxLib.IO
{
    public class XwConcurrentFile
    {
        private class XwConcurrentFileBuffer
        {
            string filePath = string.Empty;
            long maxSize = 0;
            Timer flushTimer = null;
            StringBuilder buffer = new StringBuilder();

            //-------------------------------------------------------------------------------------------------------------
            public XwConcurrentFileBuffer(string file, long size, int interval)
            {
                filePath = file;
                maxSize = size;
                flushTimer = new Timer(interval);
                flushTimer.Elapsed += (sender, e) => OnFlushTimer(this);
                flushTimer.AutoReset = true;
                flushTimer.Enabled = true;
            }

            //-------------------------------------------------------------------------------------------------------------
            public void Clear()
            {
                lock (buffer)
                {
                    buffer.Clear();
                    File.WriteAllText(filePath, "");
                }
            }

            //-------------------------------------------------------------------------------------------------------------
            public void Add(string content)
            {
                lock (buffer)
                {
                    buffer.Append(content);
                }
                
                if (buffer.Length >= maxSize)
                    Flush();
            }

            //-------------------------------------------------------------------------------------------------------------
            public void Flush()
            {
                lock (buffer)
                {
                    if (buffer.Length == 0)
                        return;

                    File.AppendAllText(filePath, buffer.ToString());
                    buffer.Clear();
                }
            }

            //-------------------------------------------------------------------------------------------------------------
            private static void OnFlushTimer(XwConcurrentFileBuffer buffer)
            {
                buffer.Flush();
            }
        }

        private static Dictionary<string, XwConcurrentFileBuffer> XwConcurrentFileBufferCollection = new Dictionary<string, XwConcurrentFileBuffer>();
        private static long XwConcurrentFileMaxBufferSize = 1024 * 4; //Kb 
        private static int XwConcurrentFileMaxFlushInterval = 1000 * 1; //Seconds

        //*****************************************************************************************************************
        public static void SetDefaultBufferSize(long size) 
        {
            XwConcurrentFileMaxBufferSize = size;
        }

        //*****************************************************************************************************************
        public static void SetDefaultFlushInterval(int intervalMs)
        {
            if (intervalMs < 500)
                throw new Exception("Flush Interval should not be lower that 500ms, and that is already too low!");
            XwConcurrentFileMaxFlushInterval = intervalMs;
        }

        //*****************************************************************************************************************
        public static void Clear(string file)
        {
            XwConcurrentFileBuffer buffer = null;
            XwConcurrentFileBufferCollection.TryGetValue(file, out buffer);
            if (buffer == null)
            {
                buffer = new XwConcurrentFileBuffer(file, XwConcurrentFileMaxBufferSize, XwConcurrentFileMaxFlushInterval);
                XwConcurrentFileBufferCollection.Add(file, buffer);
            }
            buffer.Clear();
        }

        //*****************************************************************************************************************
        public static void Write(string file, string content)
        {
            XwConcurrentFileBuffer buffer = null;
            XwConcurrentFileBufferCollection.TryGetValue(file, out buffer);
            if (buffer == null)
            {
                buffer = new XwConcurrentFileBuffer(file, XwConcurrentFileMaxBufferSize, XwConcurrentFileMaxFlushInterval);
                XwConcurrentFileBufferCollection.Add(file, buffer);
            }
            buffer.Add(content);
        }

        //*****************************************************************************************************************
        public static void Flush(string file)
        {
            XwConcurrentFileBuffer buffer = null;
            XwConcurrentFileBufferCollection.TryGetValue(file, out buffer);
            if (buffer != null)
                buffer.Flush();
        }
    }
}
