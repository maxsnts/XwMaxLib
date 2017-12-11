using System;
using System.Diagnostics;

namespace XwMaxLib.Diagnostics
{
    /// <summary>
    /// Creates a stopwatch that can be used with the "using" scope
    /// </summary>
    public class ScopedStopwatch : Stopwatch, IDisposable
	{
		private String _sName;

		/// <summary>
		/// starts a new Stopwatch
		/// </summary>
		/// <returns></returns>
		public new static ScopedStopwatch StartNew()
		{
			StackTrace oStack = new StackTrace();
			return StartNew("At \"" + oStack.GetFrame(0).GetMethod().Name + "\" Function");
		}
		/// <summary>
		///  starts a new Stopwatch width the given debug name
		/// </summary>
		/// <param name="sName"></param>
		/// <returns></returns>
		public static ScopedStopwatch StartNew(String sName)
		{
			ScopedStopwatch stopwatch = new ScopedStopwatch();
			stopwatch._sName = sName;
			stopwatch.Start();
			return stopwatch;
		}

		#region IDisposable Members

		/// <summary>
		/// stops the clock and outputs to debuger
		/// </summary>
		public void Dispose()
		{
			this.Stop();
			Debug.WriteLine(String.Format("ScopedStopwatch({0}):{1}", String.IsNullOrEmpty(this._sName) ? "Unnamed" : this._sName, this.Elapsed));
			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// stops the clock and outputs to debuger
		/// </summary>
		~ScopedStopwatch() 
		{
			if (IsRunning)
			{
				this.Stop();
				Debug.WriteLine(String.Format("ScopedStopwatch({0}) was not properlly scoped.", String.IsNullOrEmpty(this._sName) ? "Unnamed" : this._sName));
			}
		
		}
	}
}
