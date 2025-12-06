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
using System.IO;

namespace HLTStudio.Commons
{
	// ////////////////////////////////////////////////////////////////////////////////
	// ///// ///////////////////////// /////
	// ////////////////////////////////////////////////////////////////////////////////
	// ////////////////////////////////////////////////
	// ////////////////////
	// ///////////////////////////////////////
	// ////////////////////////////////////////////////////////////////////////////////

	public class ArgsReader
	{
		private string[] Args = null;
		private int ArgIndex;

		public ArgsReader(string[] args, int argIndex = 0)
		{
			this.Args = args;
			this.ArgIndex = argIndex;
		}

		public bool HasArgs(int count = 1)
		{
			return count <= this.Args.Length - this.ArgIndex;
		}

		public bool ArgIs(string spell)
		{
			if (this.HasArgs() && this.GetArg().EqualsIgnoreCase(spell))
			{
				this.ArgIndex++;
				return true;
			}
			return false;
		}

		public string GetArg(int index = 0)
		{
			return this.Args[this.ArgIndex + index];
		}

		public string NextArg()
		{
			string arg = this.GetArg();
			this.ArgIndex++;
			return arg;
		}

		public IEnumerable<string> TrailArgs()
		{
			while (this.HasArgs())
				yield return this.NextArg();
		}

		/// /////////
		/// //////////////////////
		/// ////////////////////////////////////
		/// //////////
		public void End()
		{
			if (this.HasArgs())
				throw new Exception("Bad command line option-num");
		}
	}
}

//
// <<< Processed by SolutionConv
//