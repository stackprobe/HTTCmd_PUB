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
using System.Net;
using System.Net.Sockets;
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

	public abstract class SockServer
	{
		/// /////////
		/// /////
		/// //////////
		public int PortNo = 59999;

		/// /////////
		/// //////////
		/// //////////
		public int Backlog = 300;

		/// /////////
		/// ///////
		/// //////////
		public int ConnectMax = 100;

		/// /////////
		/// ////////////
		/// ////
		/// // //////////
		/// //////////
		public Func<bool> Interlude = () => !Console.KeyAvailable;

		// ///// //// // //////

		/// /////////
		/// ////////
		/// ////
		/// // / // //// // //////////////// ////////// / ////////////// /////
		/// // / // // ////
		/// // / // // ////
		/// //////////
		/// ////// /////////////////////////////
		/// //////////////////////
		protected abstract IEnumerable<int> E_Connected(SockChannel channel);

		private List<SockChannel> Channels = new List<SockChannel>();

		public int ChannelCount
		{
			get
			{
				return this.Channels.Count;
			}
		}

		public void Run()
		{
			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを開始しています...");

			try
			{
				using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					IPEndPoint endPoint = new IPEndPoint(0L, this.PortNo);

					try
					{
						listener.Bind(endPoint);
					}
					catch (Exception ex)
					{
						throw new Exception("バインドに失敗しました。指定されたポート番号は使用中です。", ex);
					}
					listener.Listen(this.Backlog);
					listener.Blocking = false;

					SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを開始しました。");

					int waitMillis = 0;

					while (this.Interlude())
					{
						if (waitMillis < 100)
						{
							//// /////////// // /// ///////////////////////////////////////////////// ////// // /////////
							waitMillis++;
						}

						for (int c = 0; c < 30; c++) // ///// /////
						{
							Socket handler = this.Channels.Count < this.ConnectMax ? this.Connect(listener) : null;

							if (handler == null) // / //// // //////////////
								break;

							waitMillis = 0; // /////

							SockCommon.TimeWaitMonitor.I.Connected();

							{
								SockChannel channel = new SockChannel();

								channel.Handler = handler;
								handler = null;
								channel.Handler.Blocking = false;
								channel.ID = SockCommon.IDIssuer.Issue();
								channel.Connected = SCommon.Supplier(this.E_Connected(channel));
								channel.BodyOutputStream = HTTPBodyOutputStream.Create(HTTPServerChannel.BodyOnStorage);
								channel.Parent = this;

								this.Channels.Add(channel);

								SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信開始 " + channel.ID);
							}
						}
						for (int index = 0; index < this.Channels.Count;)
						{
							SockChannel channel = this.Channels[index];
							int size;

							try
							{
								size = channel.Connected();

								if (0 < size) // / ////
								{
									waitMillis = 0; // /////
								}
							}
							catch (Exception ex)
							{
								if (channel.FirstLineRecving && ex is SockChannel.RecvIdleTimeoutException)
									SockCommon.WriteLog(SockCommon.ErrorLevel_e.FIRST_LINE_TIMEOUT, null);
								else
									SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK_OR_SERVER_LOGIC, ex);

								size = 0;
							}

							if (size == 0) // / //
							{
								SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信終了 " + channel.ID);

								this.Disconnect(channel);
								SCommon.FastDesertElement(this.Channels, index);

								SockCommon.TimeWaitMonitor.I.Disconnect();
							}
							else
							{
								index++;
							}
						}

						SockCommon.ShuffleP4(this.Channels); // /////////////////

						GC.Collect();

						if (0 < waitMillis)
							Thread.Sleep(waitMillis);
					}

					SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しています...");

					this.Stop();
				}
			}
			catch (Exception ex)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, ex);
			}

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しました。");
		}

		private Socket Connect(Socket listener) // //// //// // ////////
		{
			try
			{
				return SockCommon.NB("conn", () => listener.Accept());
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != SockCommon.WSAEWOULDBLOCK)
				{
					throw new Exception($"接続失敗({ex.ErrorCode})", ex);
				}
				return null;
			}
		}

		private void Stop()
		{
			foreach (SockChannel channel in this.Channels)
				this.Disconnect(channel);

			this.Channels.Clear();
		}

		private void Disconnect(SockChannel channel)
		{
			try
			{
				channel.Handler.Shutdown(SocketShutdown.Both);
			}
			catch (Exception ex)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK, ex);
			}

			try
			{
				channel.Handler.Close();
			}
			catch (Exception ex)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK, ex);
			}

			channel.BodyOutputStream.Dispose();

			SockCommon.IDIssuer.Discard(channel.ID);
		}
	}
}

//
// <<< Processed by SolutionConv
//