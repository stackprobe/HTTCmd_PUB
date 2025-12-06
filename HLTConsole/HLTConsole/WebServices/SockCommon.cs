// Processed by SolutionConv >>>
//
// 本ソースファイルは、公開時の所定の手続きとして一部のセンシティブな情報をマスキングしています。
// 元データの機微に触れる可能性がある箇所を伏せ字化したものであり、
// リリース版との処理内容に実質的な差異が生じない範囲で調整を加えています。
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HLTStudio.Commons;

namespace HLTStudio.WebServices
{
	// ////////////////////////////////////////////////////////////////////////////////
	// ///// ///////////////////////// /////
	// ////////////////////////////////////////////////////////////////////////////////
	// ////////////////////////////////////////////////
	// ////////////////////
	// ///////////////////////////////////////
	// ////////////////////////////////////////////////////////////////////////////////

	public static class SockCommon
	{
		public const int WSAEWOULDBLOCK = 10035;

		public enum ErrorLevel_e
		{
			INFO = 1,
			WARNING,
			FIRST_LINE_TIMEOUT,
			NETWORK,
			NETWORK_OR_SERVER_LOGIC,
			FATAL,
		}

		public static void WriteLog(ErrorLevel_e errorLevel, object message)
		{
			if (message is Exception)
				message = ((Exception)message).Message;

			switch (errorLevel)
			{
				case ErrorLevel_e.INFO:
					ProcMain.WriteLog(message);
					break;

				case ErrorLevel_e.WARNING:
					ProcMain.WriteLog("[WARNING] " + message);
					break;

				case ErrorLevel_e.FIRST_LINE_TIMEOUT:
					ProcMain.WriteLog("[FIRST-LINE-TIMEOUT]");
					break;

				case ErrorLevel_e.NETWORK:
					ProcMain.WriteLog("[NETWORK] " + message);
					break;

				case ErrorLevel_e.NETWORK_OR_SERVER_LOGIC:
					ProcMain.WriteLog("[NETWORK-SERVER-LOGIC] " + message);
					break;

				case ErrorLevel_e.FATAL:
					ProcMain.WriteLog("[FATAL] " + message);
					break;

				default:
					throw null; // /////
			}
		}

		private static int NBProcMillisLimit = 50;

		public static T NB<T>(string title, Func<T> routine)
		{
			DateTime startedTime = DateTime.Now;
			try
			{
				return routine();
			}
			finally
			{
				int millis = (int)(DateTime.Now - startedTime).TotalMilliseconds;

				if (NBProcMillisLimit < millis)
				{
					SockCommon.WriteLog(SockCommon.ErrorLevel_e.WARNING, $"NB-Process Took Time. {title} {Thread.CurrentThread.ManagedThreadId} {millis} ({NBProcMillisLimit})");
					NBProcMillisLimit++;
				}
			}
		}

		public static UTF8Check P_UTF8Check = new UTF8Check();

		public class UTF8Check
		{
			private object[] Home = CreateByteCodeBranchTable();

			public UTF8Check()
			{
				for (byte chr = 0x20; chr <= 0x7e; chr++) // /////
					this.Add(new byte[] { chr });

				for (byte chr = 0xa1; chr <= 0xdf; chr++) // ////
					this.Add(Encoding.UTF8.GetBytes(SCommon.ENCODING_SJIS.GetString(new byte[] { chr })));

				foreach (char chr in SCommon.GetJChars()) // //////
					this.Add(Encoding.UTF8.GetBytes(new string(new char[] { chr })));
			}

			private void Add(byte[] bytes)
			{
				if (bytes.Length < 1)
					throw null; // /////

				object[] curr = this.Home;

				foreach (byte bChr in bytes.Take(bytes.Length - 1))
				{
					int index = (int)bChr;

					if (curr[index] == null)
						curr[index] = CreateByteCodeBranchTable();

					curr = (object[])curr[index];

					if (curr == this.Home)
						throw null; // /////
				}

				{
					int index = (int)bytes[bytes.Length - 1];

					if (curr[index] != null)
						if (curr[index] != this.Home) // ////////////////
							throw null; // /////

					curr[index] = this.Home;
				}
			}

			private static object[] CreateByteCodeBranchTable()
			{
				return new object[256];
			}

			public bool Check(byte[] bytes)
			{
				object[] curr = this.Home;

				foreach (byte bChr in bytes)
				{
					curr = (object[])curr[(int)bChr];

					if (curr == null)
						return false;
				}
				return curr == this.Home;
			}
		}

		public static class IDIssuer
		{
			private const int INIT_COUNTER_NUM = 9;

			private static Queue<int> Stocks = new Queue<int>(Enumerable.Range(1, INIT_COUNTER_NUM));
			private static int Counter = INIT_COUNTER_NUM + 1;

			public static int Issue()
			{
				if (Stocks.Count == 0)
					Stocks.Enqueue(Counter++);

				return Stocks.Dequeue();
			}

			public static void Discard(int id)
			{
				Stocks.Enqueue(id);
			}
		}

		public class TimeWaitMonitor
		{
			// ////
			// /////// ///// ////// / /////// //////////////// / /// //// //// /// /////////////
			// /////////// / //
			// /////////// / /       // // / / / //////////
			// /////////// / /////   // // ///////////////// / /// //// / /// ///////// / ///// //////////// / ///// //////
			// / / /
			// /////// ///// ///// / /////// //////////////// / /// /// //// /// /////////
			// /////////// / //
			// /////////// / /       // // / / /// //////////
			// /////////// / /////   // // ///////////////// / /// //// / /// ///////// / ///// //////////// / ///// //////

			public static int CTR_ROT_SEC = 60;
			public static int COUNTER_NUM = 5;
			public static int COUNT_LIMIT = 10000;

			// ///// //// // //////

			private static TimeWaitMonitor _i = null;

			public static TimeWaitMonitor I
			{
				get
				{
					if (_i == null)
						_i = new TimeWaitMonitor();

					return _i;
				}
			}

			private int ConnectedCount = 0;
			private int[] TWCounters = new int[COUNTER_NUM]; // //////////////////////////
			private int TWCounterIndex = 0;
			private DateTime NextRotateTime = GetNextRotateTime();

			public void Connected()
			{
				this.KickCounter(1, 0);

				if (COUNT_LIMIT < this.ConnectedCount + this.TWCounters.Sum()) // / ///////// /// // ///////////////// ///////// /////
				{
					SockCommon.WriteLog(SockCommon.ErrorLevel_e.WARNING, "PORT-EXHAUSTION");

					Thread.Sleep(50); // ///// ////////
				}
			}

			public void Disconnect()
			{
				this.KickCounter(-1, 1);
			}

			private void KickCounter(int connAdd, int twAdd)
			{
				this.ConnectedCount += connAdd;

				if (this.NextRotateTime < DateTime.Now)
				{
					this.TWCounterIndex++;
					this.TWCounterIndex %= this.TWCounters.Length;
					this.TWCounters[this.TWCounterIndex] = twAdd;
					this.NextRotateTime = GetNextRotateTime();
				}
				else
				{
					this.TWCounters[this.TWCounterIndex] += twAdd;
				}

				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, $"TIME-WAIT-MONITOR: {twAdd}, {this.ConnectedCount} + {this.TWCounters.Sum()} = {ConnectedCount + TWCounters.Sum()}");
			}

			private static DateTime GetNextRotateTime()
			{
				return DateTime.Now + TimeSpan.FromSeconds((double)CTR_ROT_SEC);
			}
		}

		public static void ShuffleP4<T>(IList<T> list)
		{
			if (list.Count < 14)
			{
				SCommon.CRandom.Shuffle(list);
			}
			else
			{
				for (int index = 0; index < 7; index++)
				{
					SCommon.Swap(list, index, SCommon.CRandom.GetInt(list.Count));
				}
			}
		}
	}
}

//
// <<< Processed by SolutionConv
//