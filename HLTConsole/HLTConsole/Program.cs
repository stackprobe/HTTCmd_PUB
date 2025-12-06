// Processed by SolutionConv >>>
//
// 本ソースファイルは、公開時の所定の手続きとして一部のセンシティブな情報をマスキングしています。
// 元データの機微に触れる可能性がある箇所を伏せ字化したものであり、
// リリース版との処理内容に実質的な差異が生じない範囲で調整を加えています。
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HLTStudio.Commons;
using HLTStudio.Tools;
using HLTStudio.WebServices;

namespace HLTStudio
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			if (ProcMain.DEBUG)
			{
				Main3();
			}
			else
			{
				Main4(ar);
			}
			SCommon.OpenOutputDirIfCreated();
		}

		private void Main3()
		{
#if DEBUG
			// // ////// /// //

			/////////// ////////////// //////// / ////
			/////////// ////////////// //////// / ////////////////////////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// //// ////
			///////// ////////////// //////// / /////////////////////////////////// ///// //// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// //////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// ///////////////// //// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// ///// //////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// ///// //////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// ///// ///////////////// ///// //////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// ///// //////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// /////// ///// ///// ///////////////// ///// //////////////// ////
			/////////// ////////////// //////// / /////////////////////////////////// ///// ///// ///// ///////////////// ////

			// //
#endif
			SCommon.Pause();
		}

		private void Main4(ArgsReader ar)
		{
			try
			{
				Main5(ar);
			}
			catch (Exception ex)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, ex);
			}
		}

		private void Main5(ArgsReader ar)
		{
			// /////////////////////////////////////////
			using (EventWaitHandle evStop = new EventWaitHandle(false, EventResetMode.ManualReset, Consts.SERVER_STOP_EVENT_NAME))
			{
				HTTPServer hs = new HTTPServer()
				{
					PortNo = 80,
					Backlog = 300,
					ConnectMax = 100,
					Interlude = () => !evStop.WaitOne(0),
					HTTPConnected = P_Connected,
				};

				SockChannel.ThreadTimeoutMillis = 100;

				HTTPServer.KeepAliveTimeoutMillis = 5000;

				HTTPServerChannel.RequestTimeoutMillis = 10000; // // ///
				HTTPServerChannel.ResponseTimeoutMillis = -1;
				HTTPServerChannel.FirstLineTimeoutMillis = 2000;
				HTTPServerChannel.IdleTimeoutMillis = 600000; // // ///
				HTTPServerChannel.BodySizeMax = 0;
				HTTPServerChannel.BodyOnStorage = false;

				SockCommon.TimeWaitMonitor.CTR_ROT_SEC = 60;
				SockCommon.TimeWaitMonitor.COUNTER_NUM = 5;
				SockCommon.TimeWaitMonitor.COUNT_LIMIT = 10000;

				// ////

				if (ar.ArgIs("/S"))
				{
					evStop.Set();
					return;
				}
				if (ar.HasArgs())
				{
					this.DocRoot = SCommon.ToFullPath(ar.NextArg());

					if (!Directory.Exists(this.DocRoot))
						throw new Exception("ドキュメントルートが見つかりません");

					if (ar.HasArgs())
					{
						hs.PortNo = int.Parse(ar.NextArg());

						if (hs.PortNo < 1 || 65535 < hs.PortNo)
							throw new Exception("不正なポート番号");

						for (; ; ) // ///////
						{
							if (ar.ArgIs("/K"))
							{
								Func<bool> baseEvent = hs.Interlude;
								hs.Interlude = () =>
								{
									if (Console.KeyAvailable)
									{
										ProcMain.WriteLog("ANY KEY PRESSED");
										return false;
									}
									return baseEvent();
								};
								ProcMain.WriteLog("キー入力を検出するとサーバーは停止します。");
								continue;
							}
							if (ar.ArgIs("/T"))
							{
								ContentTypeCollection.I.AddContentTypesByTsvFile(ar.NextArg());
								continue;
							}
							if (ar.ArgIs("/H"))
							{
								LoadHost2DocRoot(ar.NextArg());
								continue;
							}
							if (ar.ArgIs("/N"))
							{
								this.Page404File = SCommon.ToFullPath(ar.NextArg());
								ProcMain.WriteLog("Page404File: " + this.Page404File);

								if (!File.Exists(this.Page404File))
									throw new Exception("Page404File is not found");

								continue;
							}
							if (ar.ArgIs("/C"))
							{
								this.ResChunkMode = true;
								ProcMain.WriteLog("ResChunkMode ENABLED");
								continue;
							}
							break;
						}
					}
				}
				else
				{
					this.DocRoot = Directory.GetCurrentDirectory();
				}

				ProcMain.WriteLog("HTTCmd-Start");
				ProcMain.WriteLog("DocRoot: " + this.DocRoot);
				ProcMain.WriteLog("PortNo: " + hs.PortNo);

				hs.Run();

				ProcMain.WriteLog("HTTCmd-End");
			}
		}

		private void LoadHost2DocRoot(string tsvFile)
		{
			this.Host2DocRoot = SCommon.CreateDictionaryIgnoreCase<string>();

			using (CsvFileReader reader = new CsvFileReader(tsvFile, Encoding.UTF8, CsvFileReader.DELIMITER_TAB))
			{
				foreach (string[] row in reader.ReadToEnd())
				{
					if (row.Length != 2)
						continue;

					string host = row[0];
					string docRoot = SCommon.ToFullPath(row[1]);

					SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, string.Format("Add Host-DocRoot Pair: {0} = {1}", host, docRoot));

					if (!Directory.Exists(docRoot))
						throw new Exception("ドキュメントルートが見つかりません(Host2DocRoot)");

					this.Host2DocRoot.Add(host, docRoot);
				}
			}
		}

		private string DocRoot;
		private Dictionary<string, string> Host2DocRoot = null;
		private string Page404File = null;
		private bool ResChunkMode = false;

		private void P_Connected(HTTPServerChannel channel)
		{
			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "CLIENT " + channel.Channel.Handler.RemoteEndPoint);

			if (10 < channel.Method.Length) // ///// /////
				throw new Exception("Received method is too long");

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "METHOD " + channel.Method);

			bool head;
			if (channel.Method == "HEAD")
			{
				head = true;
			}
			else if (channel.Method == "GET")
			{
				head = false;
			}
			else
			{
				throw new Exception("Unsupported method: " + channel.Method);
			}

			string docRoot = this.DocRoot;
			string host = GetHeaderValue(channel, "Host");
			if (host != null && this.Host2DocRoot != null)
			{
				string hostName = host;

				// ///////
				{
					int colon = hostName.IndexOf(':');

					if (colon != -1)
						hostName = hostName.Substring(0, colon);
				}

				if (this.Host2DocRoot.ContainsKey(hostName))
					docRoot = this.Host2DocRoot[hostName];
			}

			string urlPath = channel.PathQuery;

			// /////
			{
				int ques = urlPath.IndexOf('?');

				if (ques != -1)
					urlPath = urlPath.Substring(0, ques);
			}

			if (1000 < urlPath.Length) // ///// /////
				throw new Exception("Received path is too long");

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "URL-PT " + urlPath);

			string relPath;
			string path;
			if (urlPath == "/")
			{
				relPath = "_ROOT"; // ///////////
				path = docRoot;
			}
			else
			{
				relPath = SCommon.ToFairRelPath(urlPath, -1);
				path = Path.Combine(docRoot, relPath);
			}

			bool targetToFile = false;

			if (urlPath.EndsWith("/"))
			{
				path = Path.Combine(path, "index.htm");

				if (!File.Exists(path))
					path += "l";

				targetToFile = true;
			}

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "DST-PT " + path);

			if (!targetToFile && Directory.Exists(path))
			{
				if (host == null)
					throw new Exception("No HOST header value");

				channel.ResStatus = 301;
				channel.ResHeaderPairs.Add(new string[] { "Location", $"http://{host}/{string.Join("", SCommon.Tokenize(relPath, "\\").Select(v => EncodeUrl(v) + "/"))}" });
				channel.ResBody = null;
				channel.ResBodyLength = -1L;
			}
			else if (File.Exists(path))
			{
				string file = path;
				FileInfo fileInfo = new FileInfo(path);

				channel.ResStatus = 200;
				channel.ResHeaderPairs.Add(new string[] { "Content-Type", ContentTypeCollection.I.GetContentType(Path.GetExtension(path)) });
				channel.ResBody = E_ReadFile(file, fileInfo.Length);
				channel.ResBodyLength = this.ResChunkMode ? -1L : fileInfo.Length;

				if (head)
				{
					channel.ResHeaderPairs.Add(new string[] { "Content-Length", fileInfo.Length.ToString() });
					channel.ResHeaderPairs.Add(new string[] { "X-Last-Modified-Time", new SimpleDateTime(fileInfo.LastWriteTime).ToTimeStamp().ToString() });
					channel.ResBody = null;
				}
			}
			else
			{
				channel.ResStatus = 404;
				///////////////////////////////
				channel.ResBody = null;
				channel.ResBodyLength = -1L;

				if (!head && this.Page404File != null)
				{
					string file = this.Page404File;
					long fileSize = new FileInfo(file).Length;

					channel.ResHeaderPairs.Add(new string[] { "Content-Type", "text/html" });
					channel.ResBody = E_ReadFile(file, fileSize);
					channel.ResBodyLength = this.ResChunkMode ? -1L : fileSize;
				}
			}

			channel.ResHeaderPairs.Add(new string[] { "Server", "HTTCmd" });

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-STATUS " + channel.ResStatus);

			foreach (string[] pair in channel.ResHeaderPairs)
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-HEADER " + pair[0] + " = " + pair[1]);

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-BODY " + (channel.ResBody != null));
			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-BODY-LENGTH " + channel.ResBodyLength);
		}

		private static string GetHeaderValue(HTTPServerChannel channel, string name)
		{
			foreach (string[] pair in channel.HeaderPairs)
				if (SCommon.EqualsIgnoreCase(pair[0], name))
					return pair[1];

			return null;
		}

		private static string EncodeUrl(string str)
		{
			StringBuilder buff = new StringBuilder();

			foreach (byte chr in Encoding.UTF8.GetBytes(str))
			{
				buff.Append('%');
				buff.Append(chr.ToString("x2"));
			}
			return buff.ToString();
		}

		private static IEnumerable<byte[]> E_ReadFile(string file, long fileSize)
		{
			for (long offset = 0L; offset < fileSize;)
			{
				int readSize = (int)Math.Min(fileSize - offset, (long)(512 * 1024));
				byte[] buff = new byte[readSize];

				/////////////////////////////////////////////////// ///// / / ////// / / / / //////// / / / / //////// / / / / /////// / ///// / //////////////////////// / / / / //////// / ///////// / ///// / ////////////////////////// // ////////

				using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
				{
					reader.Seek(offset, SeekOrigin.Begin);
					reader.Read(buff, 0, readSize);
				}
				yield return buff;

				offset += (long)readSize;
			}
		}
	}
}

//
// <<< Processed by SolutionConv
//