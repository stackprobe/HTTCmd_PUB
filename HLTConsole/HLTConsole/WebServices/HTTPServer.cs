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

namespace HLTStudio.WebServices
{
	// ////////////////////////////////////////////////////////////////////////////////
	// ///// ///////////////////////// /////
	// ////////////////////////////////////////////////////////////////////////////////
	// ////////////////////////////////////////////////
	// ////////////////////
	// ///////////////////////////////////////
	// ////////////////////////////////////////////////////////////////////////////////

	// ////////////
	// // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class HTTPServer : SockServer
	{
		/// /////////
		/// ////////
		/// ///
		/// // //////// //////
		/// //////////
		public Action<HTTPServerChannel> HTTPConnected = channel => { };

		/// /////////
		/// //////////////////
		/// // // ////////
		/// //////////
		public static int KeepAliveTimeoutMillis = 5000;

		// ///// //// // //////

		public HTTPServer()
		{
			PortNo = 80;
		}

		protected override IEnumerable<int> E_Connected(SockChannel channel)
		{
			DateTime startedTime = DateTime.Now;

			for (; ; )
			{
				HTTPServerChannel hsChannel = new HTTPServerChannel();
				int retval = -1;

				hsChannel.Channel = channel;

				foreach (int size in hsChannel.RecvRequest())
				{
					if (size == 0)
						throw null; // /////

					if (size < 0)
					{
						yield return retval;
						retval = -1;
					}
					else
					{
						retval = 1;
					}
				}

				SockCommon.NB("svlg", () =>
				{
					HTTPConnected(hsChannel);
					return -1; // /////
				});

				if (KeepAliveTimeoutMillis != -1 && KeepAliveTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
				{
					hsChannel.KeepAlive = false;
				}

				foreach (int size in hsChannel.SendResponse())
				{
					if (size == 0)
						throw null; // /////

					if (size < 0)
					{
						yield return retval;
						retval = -1;
					}
					else
					{
						retval = 1;
					}
				}

				if (!hsChannel.KeepAlive)
					break;
			}
		}
	}
}

//
// <<< Processed by SolutionConv
//