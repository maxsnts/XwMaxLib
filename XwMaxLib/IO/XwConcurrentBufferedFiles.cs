
// Yes yes yes, i should be opening a StreamWriter at the begining and closing at the end 
// instead of using File.AppendAllText... but this is a conscious decision
// I want to be able to write from anywhere with one line: XwConcurrentFile.Write(...)
// That is it, no Open, no Close, just write
// And if i open and close the Stream in the Write function, well that is just the same as this

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace XwMaxLib.IO
{
    public class XwConcurrentBufferedFiles
    {
        private class XwConcurrentBufferedFileInternalBuffer
        {
            long maxSize = 0;
            Timer flushTimer = null;
            StringBuilder builder = new StringBuilder();
            public string filePath = string.Empty;
            public DateTime lastFlush = DateTime.Now;

            //-------------------------------------------------------------------------------------------------------------
            public XwConcurrentBufferedFileInternalBuffer(string file, long size, int interval)
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
                lock (builder)
                {
                    builder.Clear();
                    File.WriteAllText(filePath, "");
                }
            }

            //-------------------------------------------------------------------------------------------------------------
            public void Add(string content)
            {
                lock (builder)
                {
                    builder.Append(content);
                }
                
                if (builder.Length >= maxSize)
                    Flush();
            }

            //-------------------------------------------------------------------------------------------------------------
            public void Flush()
            {
                lock (builder)
                {
                    if (builder.Length == 0)
                        return;
                    
                    lastFlush = DateTime.Now;

                    File.AppendAllText(filePath, builder.ToString());
                    builder.Clear();
                }
            }

            //-------------------------------------------------------------------------------------------------------------
            private static void OnFlushTimer(XwConcurrentBufferedFileInternalBuffer buffer)
            {
                
                buffer.Flush();
            }
        }

        private static Dictionary<string, XwConcurrentBufferedFileInternalBuffer> XwConcurrentFileBufferCollection = new Dictionary<string, XwConcurrentBufferedFileInternalBuffer>();
        private static Timer XwConcurrentFileBufferCleanTimer = null;
        private static long XwConcurrentFileMaxBufferSize = 1024 * 4; //Kb 
        private static int XwConcurrentFileMaxFlushInterval = 1000 * 1; //Seconds

        //-------------------------------------------------------------------------------------------------------------
        private static void OnCleanTimer()
        {
            lock (XwConcurrentFileBufferCollection)
            {
                Console.WriteLine(XwConcurrentFileBufferCollection.Count.ToString());
                foreach (var file in XwConcurrentFileBufferCollection)
                {
                    if (DateTime.Now.AddMinutes(-1) > file.Value.lastFlush)
                    {
                        lock (string.Intern(file.Value.filePath))
                        {
                            file.Value.Flush();
                            //Dereference the object
                            XwConcurrentFileBufferCollection[file.Value.filePath] = null;
                        }
                    }
                }
                XwConcurrentFileBufferCollection = XwConcurrentFileBufferCollection.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
            }
        }

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
            lock (string.Intern(file))
            {
                XwConcurrentBufferedFileInternalBuffer buffer = null;
                XwConcurrentFileBufferCollection.TryGetValue(file, out buffer);
                if (buffer == null)
                {
                    lock (XwConcurrentFileBufferCollection)
                    {
                        buffer = new XwConcurrentBufferedFileInternalBuffer(file, XwConcurrentFileMaxBufferSize, XwConcurrentFileMaxFlushInterval);
                        XwConcurrentFileBufferCollection.Add(file, buffer);
                    }
                }
                buffer.Clear();
            }
        }

        //*****************************************************************************************************************
        public static void Write(string file, string content)
        {
            if (XwConcurrentFileBufferCleanTimer == null)
            {
                XwConcurrentFileBufferCleanTimer = new Timer(1000 * 6);
                XwConcurrentFileBufferCleanTimer.Elapsed += (sender, e) => OnCleanTimer();
                XwConcurrentFileBufferCleanTimer.AutoReset = true;
                XwConcurrentFileBufferCleanTimer.Enabled = true;
            }

            lock (string.Intern(file))
            {
                XwConcurrentBufferedFileInternalBuffer buffer = null;
                XwConcurrentFileBufferCollection.TryGetValue(file, out buffer);
                if (buffer == null)
                {
                    lock (XwConcurrentFileBufferCollection)
                    {
                        buffer = new XwConcurrentBufferedFileInternalBuffer(file, XwConcurrentFileMaxBufferSize, XwConcurrentFileMaxFlushInterval);
                        XwConcurrentFileBufferCollection.Add(file, buffer);
                    }
                }
                buffer.Add(content);
            }
        }

        //*****************************************************************************************************************
        public static void Flush(string file)
        {
            lock (string.Intern(file))
            {
                XwConcurrentBufferedFileInternalBuffer buffer = null;
                XwConcurrentFileBufferCollection.TryGetValue(file, out buffer);
                if (buffer != null)
                    buffer.Flush();
            }
        }
    }
}
