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
using HLTStudio.Commons;

namespace HLTStudio.Tools
{
	public class CsvFileReader : IDisposable
	{
		public const char DELIMITER_COMMA = ','; // /// ////
		public const char DELIMITER_SPACE = ' '; // /// ////
		public const char DELIMITER_TAB = '\t';  // /// ////

		private char Delimiter;
		private StreamReader Reader;

		public CsvFileReader(string file)
			: this(file, GetFileEncoding(file))
		{ }

		public CsvFileReader(string file, Encoding encoding)
			: this(file, encoding, DELIMITER_COMMA)
		{ }

		public CsvFileReader(string file, Encoding encoding, char delimiter)
		{
			this.Delimiter = delimiter;
			this.Reader = new StreamReader(file, encoding);
		}

		private static Encoding GetFileEncoding(string file)
		{
			using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				// / ///// //// ///
				if (
					reader.ReadByte() == 0xEF &&
					reader.ReadByte() == 0xBB &&
					reader.ReadByte() == 0xBF
					)
					return Encoding.UTF8;
			}
			return SCommon.ENCODING_SJIS;
		}

		private int LastChar;

		private int ReadChar()
		{
			do
			{
				this.LastChar = this.Reader.Read();
			}
			while (this.LastChar == '\r');

			return this.LastChar;
		}

		private bool EnclosedCell;

		private string ReadCell()
		{
			StringBuilder buff = new StringBuilder();

			if (this.ReadChar() == '"')
			{
				while (this.ReadChar() != -1 && (this.LastChar != '"' || this.ReadChar() == '"'))
				{
					buff.Append((char)this.LastChar);
				}
				this.EnclosedCell = true;
			}
			else
			{
				while (this.LastChar != -1 && this.LastChar != '\n' && this.LastChar != this.Delimiter)
				{
					buff.Append((char)this.LastChar);
					this.ReadChar();
				}
				this.EnclosedCell = false;
			}
			return buff.ToString();
		}

		public string[] ReadRow()
		{
			List<string> row = new List<string>();

			do
			{
				row.Add(this.ReadCell());
			}
			while (this.LastChar != -1 && this.LastChar != '\n');

			if (this.LastChar == -1 && row.Count == 1 && row[0] == "" && !this.EnclosedCell)
				return null;

			return row.ToArray();
		}

		public string[][] ReadToEnd()
		{
			List<string[]> rows = new List<string[]>();

			for (; ; )
			{
				string[] row = this.ReadRow();

				if (row == null)
					break;

				rows.Add(row);
			}
			return rows.ToArray();
		}

		public void Dispose()
		{
			if (this.Reader != null)
			{
				this.Reader.Dispose();
				this.Reader = null;
			}
		}

		public static string[][] ReadToEnd(string file)
		{
			return ReadToEnd(file, GetFileEncoding(file));
		}

		public static string[][] ReadToEnd(string file, Encoding encoding)
		{
			return ReadToEnd(file, encoding, DELIMITER_COMMA);
		}

		public static string[][] ReadToEnd(string file, Encoding encoding, char delimiter)
		{
			using (CsvFileReader reader = new CsvFileReader(file, encoding, delimiter))
			{
				return reader.ReadToEnd();
			}
		}
	}
}

//
// <<< Processed by SolutionConv
//