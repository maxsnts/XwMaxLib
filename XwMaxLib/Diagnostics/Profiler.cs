using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XwMaxLib.Diagnostics
{
    public class ProfilerItem
    {
        public Stopwatch Watch = new Stopwatch();
        public ExecutionStopwatch ExWatch = new ExecutionStopwatch();
    }
    
    public class Profiler
    {
        private string Title = string.Empty;
        private Stopwatch Total = new Stopwatch();
        private ExecutionStopwatch ExTotal = new ExecutionStopwatch();
        private Dictionary<string, ProfilerItem> ProfileItems = new Dictionary<string, ProfilerItem>();
        private bool DoRecord = false;
        private int TotalTime = 0;


        //**************************************************************************************************
        public void Setup(bool StartRecord, int PrinfIfTotalTimeHigherMs = 0, string title = "")
        {
            if (!StartRecord)
                return;

            DoRecord = StartRecord;
            TotalTime = PrinfIfTotalTimeHigherMs;
            Title = title;
            Total.Start();
            ExTotal.Start();
        }

        //**************************************************************************************************
        public void Start(string key)
        {
            if (!DoRecord)
                return;

            lock(ProfileItems)
            {
                if (!ProfileItems.ContainsKey(key))
                {
                    ProfileItems.Add(key, new ProfilerItem());
                }
            }

            ProfileItems[key].Watch.Start();
            ProfileItems[key].ExWatch.Start();
        }

        //**************************************************************************************************
        public void Stop(string key)
        {
            if (!DoRecord)
                return;

            lock (ProfileItems)
            {
                if (ProfileItems.ContainsKey(key))
                {
                    ProfileItems[key].ExWatch.Stop();
                    ProfileItems[key].Watch.Stop();
                }
            }
        }

        //**************************************************************************************************
        public string Print()
        {
            if (!DoRecord)
                return string.Empty;

            Total.Stop();
            ExTotal.Stop();

            if (Total.ElapsedMilliseconds <= TotalTime)
                return string.Empty;

            int textSize = 60;
            int timeSize = 16;


            StringBuilder log = new StringBuilder();
            log.AppendLine("----------------------------------------------------------------------------------------------------------");

            if (Title != string.Empty)
            {
                log.AppendLine(String.Concat("| ", Title.PadRight(timeSize + timeSize + textSize + 10), " |"));
                log.AppendLine("----------------------------------------------------------------------------------------------------------");
            }
            
            log.AppendLine(String.Concat(
                   "| TT:",
                   (Total.IsRunning) ? ("- - - - -").PadLeft(timeSize) : Total.Elapsed.ToString(@"hh\:mm\:ss\.fffffff").PadLeft(timeSize),
                   " EX:",
                   (ExTotal.IsRunning) ? ("- - - - -").PadLeft(timeSize) : ExTotal.Elapsed.ToString(@"hh\:mm\:ss\.fffffff").PadLeft(timeSize),
                   " | ",
                   ("TOTAL (limit " + TotalTime + "ms)").ToString().PadRight(textSize),
                   " |"));

            foreach (KeyValuePair<string, ProfilerItem> pair in ProfileItems)
            {
                log.AppendLine(String.Concat(
                    "| TT:",
                    (pair.Value.Watch.IsRunning) ? ("- - - - -").PadLeft(timeSize) : pair.Value.Watch.Elapsed.ToString(@"hh\:mm\:ss\.fffffff").PadLeft(timeSize),
                    " EX:",
                    (pair.Value.ExWatch.IsRunning) ? ("- - - - -").PadLeft(timeSize) : pair.Value.ExWatch.Elapsed.ToString(@"hh\:mm\:ss\.fffffff").PadLeft(timeSize),
                    " | ",
                    pair.Key.ToString().PadRight(textSize),
                    " |"));
            }
            log.AppendLine("----------------------------------------------------------------------------------------------------------");
            ProfileItems.Clear();

            return log.ToString();
        }
    }
}
