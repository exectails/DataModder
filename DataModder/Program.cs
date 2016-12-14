using DataModder.Modding;
using DataModder.Modding.Files;
using DataModder.Modding.Xml;
using MackLib;
using MeluaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DataModder
{
	class Program
	{
		private const char VoidCharacter = '_';
		private const string HashFileName = "hash";
		private const string ToolFolderName = "DataModder";

		static string _loadedXmlFile;
		static XmlModder _loadedXmlModder;
		static string _cwd;
		static IntPtr L;

		static ModPack _modPack;

		static void Main(string[] args)
		{
			var outPath = "data";
			var modsPath = Path.Combine(ToolFolderName, "mods");
			var packagesPath = ModPack.GetPackagePath();

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture;

			//var packagesPath = ModPack.GetPackagePath();
			//var fileNames = new List<string> { @"db\itemdb.xml" };

			//VoidFileNamesInPackages(packagesPath, fileNames);
			//Console.WriteLine("voided.");
			//Console.ReadLine();

			//UnvoidFileNamesInPackages(packagesPath, fileNames);
			//Console.WriteLine("unvoided.");
			//Console.ReadLine();

			//return;

			Console.WriteLine("Mods:  " + Path.GetFullPath(modsPath));
			Console.WriteLine("Out:   " + Path.GetFullPath(outPath));
			Console.WriteLine("Pckgs: " + Path.GetFullPath(packagesPath));

			var storedHash = GetStoredHash();
			var actualHash = HashFilesAndFolders(outPath, modsPath);

			Console.WriteLine("Stored: " + storedHash);
			Console.WriteLine("Actual: " + actualHash);

			if (actualHash == storedHash)
			{
				Console.WriteLine("Data folder is up-to-date.");
			}
			else if (!Directory.Exists(modsPath))
			{
				Console.WriteLine("Mods folder not found.");
			}
			else if (!Directory.Exists(packagesPath))
			{
				Console.WriteLine("Packages folder not found.");
			}
			else
			{
				var timer = Stopwatch.StartNew();

				_modPack = new ModPack(outPath, packagesPath);
				try
				{
					_modPack.GetPackReader();
				}
				catch (IOException)
				{
					Console.WriteLine("Failed to read pack files.");
					ExitAfter(2000);
				}

				Console.WriteLine("Loading mods...");

				timer.Restart();

				LoadLua();
				var errors = LoadMods(modsPath);

				timer.Stop();
				Console.WriteLine(timer.Elapsed);

				Console.WriteLine("Applying mods...");

				timer.Restart();

				try
				{
					_modPack.Apply(true);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error while applying mods: " + ex);
				}

				timer.Stop();
				Console.WriteLine(timer.Elapsed);

				actualHash = HashFilesAndFolders(outPath, modsPath);
				if (!errors)
					StoreHash(actualHash);
				Console.WriteLine("New:    " + actualHash);
			}

			ExitAfter(2000);
		}

		private static void ExitAfter(int milliseconds)
		{
			var dontclose = false;
			var keyWaitThread = Task.Run(() =>
			{
				Console.ReadKey(true);
				Console.WriteLine("Press [Return] to close.");
				dontclose = true;
			});

			Console.WriteLine("Done, closing in 2 seconds unless any key is pressed.");
			Thread.Sleep(milliseconds);

			if (dontclose)
				Console.ReadLine();

			Environment.Exit(0);
		}









		private static bool DataPackerInUse()
		{
			if (!File.Exists("UpPack.inf"))
				return false;

			var fi = new FileInfo("UpPack.inf");
			var filesModded = (fi.Length != 0);

			return filesModded;
		}

		private static string HashFilesAndFolders(params string[] paths)
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

		private static string GetStoredHash()
		{
			var path = Path.Combine(ToolFolderName, HashFileName);

			if (!File.Exists(path))
				return "0";

			return File.ReadAllText(path);
		}

		private static void StoreHash(string hash)
		{
			var path = Path.Combine(ToolFolderName, HashFileName);

			File.WriteAllText(path, hash);
		}

		private static bool IsInsideCwd(string path)
		{
			var fullPath = Path.GetFullPath(Path.Combine(_cwd, path));
			var cwd = Path.GetFullPath(_cwd);

			fullPath = ModPack.NormalizePath(fullPath);
			cwd = ModPack.NormalizePath(cwd);

			return (fullPath.StartsWith(cwd));
		}

		private static bool FileExistsInPackages(string filePath)
		{
			var packReader = _modPack.GetPackReader();
			return packReader.Exists(filePath);
		}

		private static bool IsXmlFileLoaded()
		{
			return (_loadedXmlFile != null);
		}

		private static bool IsXPathValid(string xpath)
		{
			try
			{
				XPathExpression.Compile(xpath);
				return true;
			}
			catch (XPathException)
			{
				return false;
			}
		}

		private static void VoidFileNamesInPackages(string packagesPath, IEnumerable<string> fileNames)
		{
			foreach (var path in Directory.EnumerateFiles(packagesPath, "*.pack", SearchOption.TopDirectoryOnly).OrderBy(a => a))
				VoidFileNamesInPack(path, fileNames, true);
		}

		private static void UnvoidFileNamesInPackages(string packagesPath, IEnumerable<string> fileNames)
		{
			foreach (var path in Directory.EnumerateFiles(packagesPath, "*.pack", SearchOption.TopDirectoryOnly).OrderBy(a => a))
				VoidFileNamesInPack(path, fileNames, false);
		}

		private static void VoidFileNamesInPack(string packagePath, IEnumerable<string> fileNames, bool voidName)
		{
			int len;
			byte[] strBuffer;

			using (var fs = new FileStream(packagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
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
					var entry = new PackListEntry(packagePath, header, br);
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
						if (voidName)
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













		private static bool LoadMods(string modPath)
		{
			var errors = false;

			var mainModFiles = Directory.EnumerateFiles(modPath, "main.lua", SearchOption.AllDirectories);
			if (!mainModFiles.Any())
			{
				Console.WriteLine("No mods found.");
				return false;
			}

			Console.WriteLine("Mods found: " + mainModFiles.Count());

			foreach (var filePath in mainModFiles)
			{
				var folderPath = (Path.GetDirectoryName(filePath));
				var folderName = Path.GetFileName(folderPath);

				_cwd = Path.GetFullPath(folderPath);

				Console.WriteLine("Loading '{0}'...", folderName);

				if (Melua.luaL_dofile(L, filePath) != 0)
				{
					Console.WriteLine("Error in {1}", folderName, Melua.lua_tostring(L, -1));
					errors = true;
				}
			}

			return errors;
		}

		private static void LoadLua()
		{
			L = Melua.luaL_newstate();

			Melua.melua_openlib(L, LuaLib.BaseSafe);

			Melua.melua_register(L, "loadxmlfile", loadxmlfile);
			Melua.melua_register(L, "setattributes", setattributes);
			Melua.melua_register(L, "addelement", addelement);
			Melua.melua_register(L, "removeelements", removeelements);
			Melua.melua_register(L, "replacefile", replacefile);
			Melua.melua_register(L, "replace", replace);
			Melua.melua_register(L, "include", include);
		}

		private static int loadxmlfile(IntPtr L)
		{
			var fileName = Melua.luaL_checkstring(L, 1);
			Melua.lua_pop(L, 1);

			fileName = ModPack.NormalizePath(fileName);

			if (!fileName.EndsWith(".xml") && !fileName.EndsWith(".xml.compiled"))
				return Melua.melua_error(L, "Expected XML file extension.");

			if (!FileExistsInPackages(fileName))
				return Melua.melua_error(L, "File '{0}' not found in packages.", fileName);

			try
			{
				_loadedXmlFile = fileName;
				_loadedXmlModder = _modPack.GetXmlModder(fileName);

				return 0;
			}
			catch (XmlException ex)
			{
				_loadedXmlFile = null;
				_loadedXmlModder = null;

				return Melua.melua_error(L, "Failed to parse XML: '{0}'", ex.Message);
			}
		}

		private static int setattributes(IntPtr L)
		{
			if (!IsXmlFileLoaded())
				return Melua.melua_error(L, "No XML file loaded.");

			var modder = _loadedXmlModder;

			var selector = Melua.luaL_checkstring(L, 1);
			if (!IsXPathValid(selector))
				return Melua.melua_error(L, "Invalid XPath.");

			if (Melua.lua_isstring(L, 2) && Melua.lua_isstring(L, 3))
			{
				var attributeName = Melua.lua_tostring(L, 2);
				var attributeValue = Melua.lua_tostring(L, 3);
				Melua.lua_pop(L, 2);

				_modPack.AddMod(new XmlAttributeSetter(modder, selector, attributeName, attributeValue));
			}
			else if (Melua.lua_istable(L, 2))
			{
				Melua.lua_pushnil(L);
				while (Melua.lua_next(L, -2) != 0)
				{
					var attributeName = Melua.luaL_checkstring(L, -2);
					var attributeValue = Melua.luaL_checkstring(L, -1);
					Melua.lua_pop(L, 1);

					_modPack.AddMod(new XmlAttributeSetter(modder, selector, attributeName, attributeValue));
				}

				Melua.lua_pop(L, 1);
			}
			else
			{
				return Melua.melua_error(L, "Invalid argument type.");
			}

			Melua.lua_pop(L, 1);

			return 0;
		}

		private static int addelement(IntPtr L)
		{
			if (!IsXmlFileLoaded())
				return Melua.melua_error(L, "No XML file loaded.");

			var selector = Melua.luaL_checkstring(L, 1);
			var xml = Melua.luaL_checkstring(L, 2);
			Melua.lua_pop(L, 2);

			if (!IsXPathValid(selector))
				return Melua.melua_error(L, "Invalid XPath.");

			var modder = _loadedXmlModder;
			_modPack.AddMod(new XmlElementAdder(modder, selector, xml));

			return 0;
		}

		private static int removeelements(IntPtr L)
		{
			if (!IsXmlFileLoaded())
				return Melua.melua_error(L, "No XML file loaded.");

			var selectors = new HashSet<string>();

			if (Melua.lua_isstring(L, -1))
			{
				var selector = Melua.luaL_checkstring(L, 1);
				Melua.lua_pop(L, 1);

				if (!IsXPathValid(selector))
					return Melua.melua_error(L, "Invalid XPath.");

				selectors.Add(selector);
			}
			else if (Melua.lua_istable(L, -1))
			{
				Melua.lua_pushnil(L);
				while (Melua.lua_next(L, -2) != 0)
				{
					var selector = Melua.luaL_checkstring(L, -1);
					Melua.lua_pop(L, 1);

					if (!IsXPathValid(selector))
						return Melua.melua_error(L, "Invalid XPath.");

					selectors.Add(selector);
				}
			}
			else
			{
				return Melua.melua_error(L, "Invalid argument type '{0}'.", Melua.luaL_typename(L, -1));
			}

			var modder = _loadedXmlModder;
			foreach (var selector in selectors)
				_modPack.AddMod(new XmlElementRemover(modder, selector));

			return 0;
		}

		private static int replacefile(IntPtr L)
		{
			var targetPath = Melua.luaL_checkstring(L, 1);
			var sourcePath = Melua.luaL_checkstring(L, 2);
			Melua.lua_pop(L, 2);

			targetPath = ModPack.NormalizePath(targetPath);
			sourcePath = ModPack.NormalizePath(sourcePath);

			var outPath = Path.Combine("data", targetPath);
			var localPath = Path.Combine(_cwd, sourcePath);
			var packPath = sourcePath;
			var packReader = _modPack.GetPackReader();

			if (File.Exists(localPath))
			{
				if (!IsInsideCwd(localPath))
					return Melua.melua_error(L, "Invalid path. ({0})", localPath);

				_modPack.AddMod(new FileCopier(localPath, outPath));
			}
			else if (packReader.Exists(packPath))
			{
				var entry = packReader.GetEntry(packPath);
				var data = entry.GetData();

				_modPack.AddMod(new FileSetter(outPath, data));
			}
			else
			{
				// TODO: Replace with string?
				return Melua.melua_error(L, "File not found: {0}", sourcePath);
			}

			return 0;
		}

		private static int replace(IntPtr L)
		{
			if (!IsXmlFileLoaded())
				return Melua.melua_error(L, "No XML file loaded.");

			var search = Melua.luaL_checkstring(L, 1);
			var replace = Melua.luaL_checkstring(L, 2);
			Melua.lua_pop(L, 2);

			var modder = _loadedXmlModder;
			_modPack.AddMod(new XmlReplacer(modder, search, replace));

			return 0;
		}

		private static int include(IntPtr L)
		{
			if (!IsXmlFileLoaded())
				return Melua.melua_error(L, "No XML file loaded.");

			var path = Melua.luaL_checkstring(L, 1);
			Melua.lua_pop(L, 1);

			var fullPath = Path.GetFullPath(Path.Combine(_cwd, path));

			if (!IsInsideCwd(fullPath))
				return Melua.melua_error(L, "Invalid path. ({0})", path);

			if (!File.Exists(fullPath))
				return Melua.melua_error(L, "File not found. ({0})", path);

			var status = Melua.luaL_dofile(L, fullPath);
			if (status != 0) // Error
				return status;

			var returnValues = Melua.lua_gettop(L);

			return returnValues;
		}
	}
}
