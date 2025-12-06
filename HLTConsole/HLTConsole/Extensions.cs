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
using HLTStudio.Commons;

namespace HLTStudio
{
	// ////////////////////////////////////////////////////////////////////////////////
	// ///// ///////////////////////// /////
	// ////////////////////////////////////////////////////////////////////////////////
	// ////////////////////////////////////////////////
	// ////////////////////
	// ///////////////////////////////////////
	// ////////////////////////////////////////////////////////////////////////////////

	public static class Extensions
	{
		public static IEnumerable<T> DistinctOrderBy<T>(this IEnumerable<T> src, Comparison<T> comp)
		{
			List<T> list = src.ToList();
			SCommon.DistinctSort(list, comp);
			return list;
		}

		public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> src, Comparison<T> comp)
		{
			List<T> list = src.ToList();
			list.Sort(comp);
			return list;
		}

		public static bool EqualsIgnoreCase(this string a, string b)
		{
			return SCommon.EqualsIgnoreCase(a, b);
		}

		public static bool StartsWithIgnoreCase(this string a, string b)
		{
			return SCommon.StartsWithIgnoreCase(a, b);
		}

		public static bool EndsWithIgnoreCase(this string a, string b)
		{
			return SCommon.EndsWithIgnoreCase(a, b);
		}

		public static bool ContainsIgnoreCase(this string a, string b)
		{
			return SCommon.ContainsIgnoreCase(a, b);
		}

		public static int IndexOfIgnoreCase(this string a, string b)
		{
			return SCommon.IndexOfIgnoreCase(a, b);
		}

		public static int IndexOfIgnoreCase(this string a, string b, int startIndex)
		{
			return SCommon.IndexOfIgnoreCase(a, b, startIndex);
		}

		public static int IndexOfIgnoreCase(this string a, char b)
		{
			return SCommon.IndexOfIgnoreCase(a, b);
		}

		public static int IndexOfIgnoreCase(this string a, char b, int startIndex)
		{
			return SCommon.IndexOfIgnoreCase(a, b, startIndex);
		}

		public static int IndexOfIgnoreCase(this IList<string> strs, string str)
		{
			return SCommon.IndexOfIgnoreCase(strs, str);
		}

		public static int IndexOfIgnoreCase(this IList<string> strs, string str, int index)
		{
			return SCommon.IndexOfIgnoreCase(strs, str, index);
		}

		public static int IndexOfIgnoreCase(this IList<string> strs, string str, int index, int count)
		{
			return SCommon.IndexOfIgnoreCase(strs, str, index, count);
		}

		public static string ReplaceIgnoreCase(this string str, string oldPtn, string newPtn)
		{
			return SCommon.ReplaceIgnoreCase(str, oldPtn, newPtn);
		}

		public static IEnumerable<T> WithProgressBar<T>(this IEnumerable<T> src)
		{
			IList<T> list = SCommon.AsIList(src);

			if (list.Count == 0)
			{
				Console.Write("[*****************************************************************************]");
			}
			else
			{
				int currBarLen = 0;

				Console.Write("[-----------------------------------------------------------------------------]\r[");

				for (int index = 0; index < list.Count; index++)
				{
					int barLen = (int)(((index + 1) * 77L) / list.Count);

					while (currBarLen < barLen)
					{
						Console.Write("*");
						currBarLen++;
					}
					yield return list[index];
				}
			}
			Console.WriteLine();
		}

		public static IEnumerable<T> Linearize<T>(this IEnumerable<T[]> src)
		{
			return SCommon.Linearize(src);
		}

		public static string ReplaceAll(this string text, params string[] replacements)
		{
			return SCommon.ReplaceAll(text, replacements);
		}

		public static string ReplaceAllIgnoreCase(this string text, params string[] replacements)
		{
			return SCommon.ReplaceAllIgnoreCase(text, replacements);
		}
	}
}

//
// <<< Processed by SolutionConv
//