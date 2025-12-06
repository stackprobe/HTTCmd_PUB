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
using HLTStudio.Tools;

namespace HLTStudio.WebServices
{
	// ////////////////////////////////////////////////////////////////////////////////
	// ///// ///////////////////////// /////
	// ////////////////////////////////////////////////////////////////////////////////
	// ////////////////////////////////////////////////
	// ////////////////////
	// ///////////////////////////////////////
	// ////////////////////////////////////////////////////////////////////////////////

	public static class HTTPBodyOutputStream
	{
		public static IBOS Create(bool fileMode)
		{
			if (fileMode)
				return new FileBOS();
			else
				return new MemoryBOS();
		}

		public interface IBOS : IDisposable
		{
			/// /////////
			/// //////////
			/// //////////
			/// ////// ////////////////////////////
			void Write(byte[] data);

			/// /////////
			/// //////////////
			/// //////////
			/// /////////////////////////////
			long GetWroteSize();

			/// /////////
			/// ////////////////////
			/// //////////
			/// ////////////////////////////
			byte[] ToByteArray();

			/// /////////
			/// //////////////////////////
			/// //////////
			/// ////// //////////////////////////////
			void ToFile(string destFile);

			/// /////////
			/// /////////////////////
			/// //////////
			/// ////// ////////////////////////////
			void ReadToEnd(SCommon.Write_d writer);
		}

		private class FileBOS : IBOS
		{
			public WorkingDir WD = new WorkingDir();
			public string BufferFile;
			public long WroteSize = 0L;
			public CtrCipher CtrCipher = CtrCipher.CreateTemporary();

			public FileBOS()
			{
				this.BufferFile = this.WD.MakePath();
			}

			public void Write(byte[] data)
			{
				byte[] maskedPart = new byte[data.Length];

				this.CtrCipher.Mask(data, 0, maskedPart, 0, data.Length);

				using (FileStream writer = new FileStream(this.BufferFile, FileMode.Append, FileAccess.Write))
				{
					writer.Write(maskedPart, 0, data.Length);
				}
				this.WroteSize += data.Length;
			}

			public long GetWroteSize()
			{
				return this.WroteSize;
			}

			public byte[] ToByteArray()
			{
				byte[] data = File.ReadAllBytes(this.BufferFile);
				SCommon.DeletePath(this.BufferFile);
				this.WroteSize = 0L;

				this.CtrCipher.Reset();
				this.CtrCipher.Mask(data);
				this.CtrCipher.Reset();

				return data;
			}

			public void ToFile(string destFile)
			{
				this.CtrCipher.Reset();

				using (FileStream reader = new FileStream(this.BufferFile, FileMode.Open, FileAccess.Read))
				using (FileStream writer = new FileStream(destFile, FileMode.Create, FileAccess.Write))
				{
					SCommon.ReadToEnd(reader.Read, (buff, offset, count) =>
					{
						this.CtrCipher.Mask(buff, offset, count);
						writer.Write(buff, offset, count);
					});
				}

				SCommon.DeletePath(this.BufferFile);
				this.WroteSize = 0L;

				this.CtrCipher.Reset();
			}

			public void ReadToEnd(SCommon.Write_d writer)
			{
				this.CtrCipher.Reset();

				using (FileStream reader = new FileStream(this.BufferFile, FileMode.Open, FileAccess.Read))
				{
					SCommon.ReadToEnd(reader.Read, (buff, offset, count) =>
					{
						this.CtrCipher.Mask(buff, offset, count);
						writer(buff, offset, count);
					});
				}

				SCommon.DeletePath(this.BufferFile);
				this.WroteSize = 0;

				this.CtrCipher.Reset();
			}

			public void Dispose()
			{
				if (this.WD != null)
				{
					this.WD.Dispose();
					this.WD = null;

					this.CtrCipher.Dispose();
					this.CtrCipher = null;
				}
			}
		}

		private class MemoryBOS : IBOS
		{
			private MemoryStream Buffer = new MemoryStream();

			public void Write(byte[] data)
			{
				SCommon.Write(this.Buffer, data);
			}

			public long GetWroteSize()
			{
				return this.Buffer.Length;
			}

			public byte[] ToByteArray()
			{
				byte[] data = this.Buffer.ToArray();
				this.Buffer.SetLength(0L);
				return data;
			}

			public void ToFile(string destFile)
			{
				File.WriteAllBytes(destFile, this.ToByteArray());
			}

			public void ReadToEnd(SCommon.Write_d writer)
			{
				SCommon.Write(writer, this.ToByteArray());
			}

			public void Dispose()
			{
				if (this.Buffer != null)
				{
					this.Buffer.Dispose();
					this.Buffer = null;
				}
			}
		}
	}
}

//
// <<< Processed by SolutionConv
//