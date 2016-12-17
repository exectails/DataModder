using MackLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModder
{
	public static class PackVoider
	{
		private const char VoidCharacter = '_';

		public static void ModifyFileNamesInPack(string packPath, IEnumerable<string> fileNames, bool doVoidNames)
		{
			int len;
			byte[] strBuffer;

			using (var fs = new FileStream(packPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
			using (var br = new BinaryReader(fs, Encoding.ASCII))
			using (var bw = new BinaryWriter(fs, Encoding.ASCII))
			{
				var header = new PackHeader();
				header.Signature = br.ReadBytes(8);
				header.D1 = br.ReadUInt32();
				header.Sum = br.ReadUInt32();
				header.FileTime1 = DateTime.FromFileTimeUtc(br.ReadInt64());
				header.FileTime2 = DateTime.FromFileTimeUtc(br.ReadInt64());

				strBuffer = br.ReadBytes(480);
				len = Array.IndexOf(strBuffer, (byte)0);
				header.DataPath = Encoding.UTF8.GetString(strBuffer, 0, len);

				header.FileCount = br.ReadUInt32();
				header.HeaderLength = br.ReadUInt32();
				header.BlankLength = br.ReadUInt32();
				header.DataLength = br.ReadUInt32();
				header.Zero = br.ReadBytes(16);

				for (int i = 0; i < header.FileCount; ++i)
				{
					var entry = new PackListEntry(packPath, header, br);
					var nameStart = -1L;

					entry.NameType = (PackListNameType)br.ReadByte();

					if (entry.NameType <= PackListNameType.L64)
					{
						var size = (0x10 * ((byte)entry.NameType + 1));
						nameStart = br.BaseStream.Position;
						strBuffer = br.ReadBytes(size - 1);
					}
					else if (entry.NameType == PackListNameType.L96)
					{
						var size = 0x60;
						nameStart = br.BaseStream.Position;
						strBuffer = br.ReadBytes(size - 1);
					}
					else if (entry.NameType == PackListNameType.LDyn)
					{
						var size = (int)br.ReadUInt32() + 5;
						nameStart = br.BaseStream.Position;
						strBuffer = br.ReadBytes(size - 1 - 4);
					}
					else
						throw new Exception("Unknown entry name type '" + entry.NameType + "'.");

					len = Array.IndexOf(strBuffer, (byte)0);
					entry.FullName = Encoding.UTF8.GetString(strBuffer, 0, len);
					entry.FileName = Path.GetFileName(entry.FullName);

					var nameEnd = br.BaseStream.Position;
					var entryFullName = entry.FullName;

					foreach (var unvoidedName in fileNames)
					{
						if (doVoidNames)
						{
							if (entryFullName == unvoidedName)
							{
								bw.BaseStream.Seek(nameStart, SeekOrigin.Begin);
								bw.Write(VoidCharacter);
								bw.BaseStream.Seek(nameEnd, SeekOrigin.Begin);
								break;
							}
						}
						else
						{
							var voidedName = VoidCharacter + unvoidedName.Substring(1);

							if (entryFullName == voidedName)
							{
								var firstCharacter = unvoidedName[0];

								bw.BaseStream.Seek(nameStart, SeekOrigin.Begin);
								bw.Write(firstCharacter);
								bw.BaseStream.Seek(nameEnd, SeekOrigin.Begin);
							}
						}
					}

					entry.Seed = br.ReadUInt32();
					entry.Zero = br.ReadUInt32();
					entry.DataOffset = br.ReadUInt32();
					entry.CompressedSize = br.ReadUInt32();
					entry.DecompressedSize = br.ReadUInt32();
					entry.IsCompressed = br.ReadUInt32() == 1;
					entry.FileTime1 = DateTime.FromFileTimeUtc(br.ReadInt64());
					entry.FileTime2 = DateTime.FromFileTimeUtc(br.ReadInt64());
					entry.FileTime3 = DateTime.FromFileTimeUtc(br.ReadInt64());
					entry.FileTime4 = DateTime.FromFileTimeUtc(br.ReadInt64());
					entry.FileTime5 = DateTime.FromFileTimeUtc(br.ReadInt64());
				}
			}
		}
	}
}
