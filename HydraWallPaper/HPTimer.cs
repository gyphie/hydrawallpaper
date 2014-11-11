using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydraPaper
{
	public class HPTimer : IDisposable
	{
		private System.Timers.Timer internalTimer;

		private DateTime intervalStarted;

		public HPTimer() : this(60000) { }

		public HPTimer(double interval)
		{
			this.internalTimer = new System.Timers.Timer(interval);
			this.internalTimer.Elapsed += this.InternalElapsed;
			this.internalTimer.AutoReset = true;
		}

		private System.Timers.ElapsedEventHandler userElapsed;

		public System.Timers.ElapsedEventHandler Elapsed
		{
			set
			{
				this.userElapsed = value;
			}
		}

		private void InternalElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.intervalStarted = DateTime.UtcNow;
			this.userElapsed(sender, e);
		}

		public double Interval
		{
			get
			{
				return this.internalTimer.Interval;
			}
			set
			{
				this.internalTimer.Interval = value;
			}
		}

		public void Stop()
		{
			this.internalTimer.Stop();
		}

		public void Start()
		{
			this.internalTimer.Start();
			this.intervalStarted = DateTime.UtcNow;
		}

		public bool Enabled
		{
			get
			{
				return this.internalTimer.Enabled;
			}
		}

		public double MillisecondsPassed
		{
			get
			{
				if (this.internalTimer.Enabled)
				{
					return (DateTime.Now - this.intervalStarted).TotalMilliseconds;
				}
				else
				{
					return 0;
				}
			}
		}
		public double MillisecondsRemaining
		{
			get
			{
				if (this.internalTimer.Enabled)
				{
					return Math.Max(0, this.internalTimer.Interval - (DateTime.Now - this.intervalStarted).TotalMilliseconds);
				}
				else
				{
					return 0;
				}
			}
		}

		public void Dispose()
		{
			this.internalTimer.Dispose();
		}
	}
}
