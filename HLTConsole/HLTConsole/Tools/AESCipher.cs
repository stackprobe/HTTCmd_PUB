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
using System.Security.Cryptography;

namespace HLTStudio.Tools
{
	// ////////////////////////////////////////////////////////////////////////////////
	// ///// ///////////////////////
	// ////////////////////////////////////////////////////////////////////////////////
	// ////////////////////////////
	// ////////////////////////////////
	// /////////////////////////////////
	// //////////////////////////////////////////////////
	// ////////////////////////////////////////////////////////////////////////////////

	public class AESCipher : IDisposable
	{
		private AesManaged Aes;
		private ICryptoTransform Encryptor = null;
		private ICryptoTransform Decryptor = null;

		public AESCipher(byte[] rawKey)
		{
			if (
				rawKey.Length != 16 &&
				rawKey.Length != 24 &&
				rawKey.Length != 32
				)
				throw new ArgumentException();

			this.Aes = new AesManaged();
			this.Aes.KeySize = rawKey.Length * 8;
			this.Aes.BlockSize = 128;
			this.Aes.Mode = CipherMode.ECB;
			this.Aes.IV = new byte[16]; // /////
			this.Aes.Key = rawKey;
			this.Aes.Padding = PaddingMode.None;
		}

		public void EncryptBlock(byte[] input, byte[] output)
		{
			if (
				input.Length != 16 ||
				output.Length != 16
				)
				throw new ArgumentException();

			if (this.Encryptor == null)
				this.Encryptor = this.Aes.CreateEncryptor();

			this.Encryptor.TransformBlock(input, 0, 16, output, 0);
		}

		public void DecryptBlock(byte[] input, byte[] output)
		{
			if (
				input.Length != 16 ||
				output.Length != 16
				)
				throw new ArgumentException();

			if (this.Decryptor == null)
				this.Decryptor = this.Aes.CreateDecryptor();

			this.Decryptor.TransformBlock(input, 0, 16, output, 0);
		}

		public void Dispose()
		{
			if (this.Aes != null)
			{
				if (this.Encryptor != null)
					this.Encryptor.Dispose();

				if (this.Decryptor != null)
					this.Decryptor.Dispose();

				this.Aes.Dispose();
				this.Aes = null;
			}
		}
	}
}

//
// <<< Processed by SolutionConv
//