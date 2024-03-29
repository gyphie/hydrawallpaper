﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace HydraPaper
{
	public static class NonBlockingConsole
	{
		private static BlockingCollection<string> m_Queue = new BlockingCollection<string>();

		public static void StartProcessingMessages()
		{
			var thread = new Thread(() => {
				while (true) Console.WriteLine(m_Queue.Take());
			});
			thread.IsBackground = true;
			thread.Start();
		}

		public static void WriteLine(string value)
		{
			m_Queue.Add(value);
		}
	}
}
