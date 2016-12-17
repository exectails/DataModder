using DataModder.Modding.Xml;
using Fetitor;
using MackLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DataModder.Modding
{
	/// <summary>
	/// Collection of mods.
	/// </summary>
	public class ModPack : IDisposable
	{
		private object _syncLock = new object();

		private List<IMod> _mods = new List<IMod>();
		static Dictionary<string, XmlModder> _xmlModders = new Dictionary<string, XmlModder>();
		private PackReader _packReader;

		/// <summary>
		/// The path to write the modded files to.
		/// </summary>
		public string OutPath { get; private set; }

		/// <summary>
		/// The path to read pack files from.
		/// </summary>
		public string PackagePath { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="dataPath">Path to write the modded files to.</param>
		/// <param name="packagePath">Path to read pack files from.</param>
		public ModPack(string dataPath, string packagePath)
		{
			this.OutPath = dataPath;
			PrepareFolder(dataPath);

			this.PackagePath = packagePath;
			if (!Directory.Exists(this.PackagePath))
				throw new DirectoryNotFoundException("Package directory not found.");
		}

		/// <summary>
		/// Adds mod to be applied.
		/// </summary>
		/// <param name="mod"></param>
		public void AddMod(IMod mod)
		{
			lock (_syncLock)
				_mods.Add(mod);
		}

		/// <summary>
		/// Applies all mods and saves the files.
		/// </summary>
		/// <param name="clearOutPath">Empty path folder first?</param>
		public void Apply(bool clearOutPath)
		{
			if (clearOutPath)
				EmptyDirectory(this.OutPath);

			lock (_syncLock)
			{
				foreach (var mod in _mods)
				{
					mod.Process();
				}

				foreach (var xmlModder in _xmlModders)
				{
					var outPath = Path.Combine(this.OutPath, xmlModder.Key);
					PrepareFolder(outPath);

					SaveXmlModder(xmlModder.Value, outPath);
				}
			}
		}

		/// <summary>
		/// Returns pack reader to read files inside packages from.
		/// </summary>
		/// <returns></returns>
		public PackReader GetPackReader()
		{
			lock (_syncLock)
			{
				if (_packReader == null)
					_packReader = new PackReader(this.PackagePath);
			}

			return _packReader;
		}

		/// <summary>
		/// Closes open pack reader.
		/// </summary>
		public void ClosePackReader()
		{
			if (_packReader == null)
				return;

			_packReader.Close();
			_packReader = null;
		}

		/// <summary>
		/// Closes open pack files.
		/// </summary>
		public void Dispose()
		{
			this.ClosePackReader();
		}

		/// <summary>
		/// Returns the path of the package folder.
		/// </summary>
		/// <returns></returns>
		public static string GetPackagePath()
		{
			if (Directory.Exists("package"))
				return "package";

			var mabiPackagePath = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			if (Directory.Exists(mabiPackagePath))
				return mabiPackagePath;

			return null;
		}

		/// <summary>
		/// Returns modder for the given XML file from the packages.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public XmlModder GetXmlModder(string fileName)
		{
			fileName = NormalizePath(fileName);

			lock (_syncLock)
			{
				if (_xmlModders.ContainsKey(fileName))
					return _xmlModders[fileName];
			}

			var isCompiled = false;
			if (!fileName.EndsWith(".xml") && !(isCompiled = fileName.EndsWith(".xml.compiled")))
				throw new ArgumentException("Expected XML file extension.");

			var packReader = this.GetPackReader();
			if (!packReader.Exists(fileName))
				throw new FileNotFoundException("File not found in packages: '" + fileName + "'");

			var entry = _packReader.GetEntry(fileName);
			var tmpFilePath = Path.GetTempFileName();

			var modder = new XmlModder();

			if (isCompiled)
			{
				var featuresFile = FeaturesFile.CompiledToXml(entry.GetDataAsStream());
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(featuresFile)))
					modder.LoadFromStream(ms);
			}
			else
			{
				using (var stream = entry.GetDataAsStream())
				using (var sr = new StreamReader(stream))
				{
					var xml = sr.ReadToEnd();
					xml = FixXmlCode(fileName, xml);

					modder.LoadFromString(xml);
				}
			}

			lock (_syncLock)
				_xmlModders[fileName] = modder;

			return modder;
		}

		private static Regex _xmlComments = new Regex("<!--(?<comment>.*?)-->", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// Fixes known XML errors in Mabi's files.
		/// </summary>
		/// <param name="referencePath"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static string FixXmlCode(string referencePath, string xml)
		{
			// Fix hyphens in comments
			xml = _xmlComments.Replace(xml, match =>
			{
				var comment = match.Groups["comment"].Value;
				comment = comment.Replace("--", "");
				comment = comment.Trim('-');

				return "<!--" + comment + "-->";
			});

			// Fix incorrect ending tag in gate.xml
			if (referencePath == @"db\gate.xml")
			{
				var halfLength = xml.Length / 2;
				var sb = new StringBuilder(xml);
				sb.Replace("</moongate>", "</moontunnel>", halfLength, halfLength);

				xml = sb.ToString();
			}

			return xml;
		}

		/// <summary>
		/// Saves the modified XML file from the modder to the given path,
		/// either raw or compiled, depending on the extension.
		/// </summary>
		/// <param name="modder"></param>
		/// <param name="filePath"></param>
		private void SaveXmlModder(XmlModder modder, string filePath)
		{
			PrepareFolder(filePath);

			var isCompiled = filePath.EndsWith(".xml.compiled");
			if (isCompiled)
			{
				var xml = modder.GetString();
				FeaturesFile.SaveCompiledFromXml(filePath, xml);
			}
			else
			{
				modder.Save(filePath);
			}
		}

		/// <summary>
		/// Unifies path strings.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string NormalizePath(string path)
		{
			path = path.Replace("/", "\\");
			path = path.Trim().TrimStart('\\');

			return path;
		}

		/// <summary>
		/// Creates folder structure for given path if it doesn't exist yet.
		/// </summary>
		/// <param name="path"></param>
		public static void PrepareFolder(string path)
		{
			string dirPath;
			if (Path.HasExtension(path))
				dirPath = Path.GetDirectoryName(path);
			else
				dirPath = path;

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
		}

		/// <summary>
		/// Removes all files and folders in given directory if it exists.
		/// </summary>
		/// <param name="path"></param>
		public static void EmptyDirectory(string path)
		{
			if (!Directory.Exists(path))
				return;

			var di = new DirectoryInfo(path);

			foreach (var file in di.GetFiles())
				file.Delete();

			foreach (var dir in di.GetDirectories())
				dir.Delete(true);
		}
	}
}
