using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataModder
{
	public static class FileSystemHasher
	{
		public static string HashFilesAndFolders(params string[] paths)
		{
			var sb = new StringBuilder();

			foreach (var path in paths)
			{
				if (File.Exists(path))
				{
					sb.Append(CombineFileInfo(path));
				}
				else if (Directory.Exists(path))
				{
					var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).OrderBy(a => a);
					foreach (var filePath in files)
						sb.Append(CombineFileInfo(filePath));
				}
			}

			var hasher = new SHA256Managed();
			var combined = Encoding.UTF8.GetBytes(sb.ToString());
			var hash = BitConverter.ToString(hasher.ComputeHash(combined)).Replace("-", "");

			return hash;
		}

		private static string CombineFileInfo(string filePath)
		{
			var info = new FileInfo(filePath);
			var lastWrite = info.LastWriteTime.ToString(CultureInfo.InvariantCulture);
			var length = info.Length;

			return (filePath + "," + length + "," + lastWrite + ";");
		}
	}
}
