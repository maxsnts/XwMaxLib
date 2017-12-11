using System;
using System.Runtime.InteropServices;

namespace XwMaxLib.Diagnostics
{
    /// <summary>
    /// Provides a set of methods and properties that you can use to accurately measure execution time. 
    /// </summary>
    public class ExecutionStopwatch
	{
		#region private menbers
		private long _nEndTime;
		private long _nStartTime;
		private bool _bIsRunning;
		#endregion

		#region Win32 import
		[DllImport("kernel32.dll", EntryPoint = "GetThreadTimes")]
		private static extern long _GetThreadTimes(IntPtr threadHandle, out long createionTime,out long exitTime, out long kernelTime, out long userTime);

		[DllImport("kernel32.dll", EntryPoint = "GetCurrentThread")]
		private static extern IntPtr _GetCurrentThread();
		#endregion

		#region private methods
		private long _GetThreadTimes()
		{
			long nDiscardable;
			long nKernelTime;
			long nUserTime;

			long nRetCode = _GetThreadTimes(_GetCurrentThread(), out nDiscardable, out nDiscardable, out nKernelTime, out nUserTime);

			if (!Convert.ToBoolean(nRetCode))
				throw new Exception(string.Format("failed to get timestamp. error code: {0}", nRetCode));

			return nKernelTime + nUserTime;
		}
		#endregion

		#region public properties
        /// <summary>
        /// Gets a value indicating whether the ExecutionStopwatch timer is running.
        /// </summary>
		public bool IsRunning
		{
			get { return _bIsRunning; }
		}
        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
		public TimeSpan Elapsed
		{
			get { return TimeSpan.FromMilliseconds((_nEndTime - _nStartTime) / 10000); }
		}
        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
		public long ElapsedMilliseconds
		{
			get { return (_nEndTime - _nStartTime) / 10000; }
		}
        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in timer ticks.
        /// </summary>
		public long ElapsedTicks
		{
			get { return _nEndTime - _nStartTime; }
		}
		#endregion

		#region public methods
        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval. 
        /// </summary>
		public void Start()
		{
			_bIsRunning = true;
			_nStartTime = _GetThreadTimes();
		}
        /// <summary>
        /// Stops measuring elapsed time for an interval. 
        /// </summary>
		public void Stop()
		{
			_nEndTime = _GetThreadTimes();
			_bIsRunning = false;
		}
        /// <summary>
        /// Stops time interval measurement and resets the elapsed time to zero. 
        /// </summary>
		public void Reset()
		{
			_nEndTime = 0;
			_nStartTime = _GetThreadTimes();
		}
		#endregion

	}
}
