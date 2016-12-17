using DataModder.Modding;
using DataModder.Modding.Xml;
using MackLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;

namespace DataModder
{
	public partial class FrmMain : Form
	{
		private const string HashFileName = "hash";
		private const string FileListFileName = "list";
		private const string ToolFolderName = "DataModder";

		private StringWriter _trace;

		public FrmMain()
		{
			InitializeComponent();

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture;
		}

		private void FrmMain_Load(object sender, EventArgs e)
		{
			Trace.Listeners.Add(new TextWriterTraceListener(_trace = new StringWriter()));
			Trace.Listeners.Add(new TextBoxAppendTraceListener(this.TxtTrace));
		}

		private static bool DataPackerInUse()
		{
			if (!File.Exists("UpPack.inf"))
				return false;

			var fi = new FileInfo("UpPack.inf");
			var filesModded = (fi.Length != 0);

			return filesModded;
		}

		private static bool InsideMabiFolder()
		{
			var patcherExists = File.Exists("Mabinogi.exe");
			var clientExists = File.Exists("Client.exe");

			return (patcherExists && clientExists);
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

		private void LoadAndApplyMods()
		{
			var dataPath = "data";
			var packagesPath = ModPack.GetPackagePath();
			var modsPath = Path.Combine(ToolFolderName, "mods");
			var timer = Stopwatch.StartNew();

			Trace.WriteLine("Mods:   " + Path.GetFullPath(modsPath));
			Trace.WriteLine("Data:   " + Path.GetFullPath(dataPath));
			Trace.WriteLine("Pckgs:  " + Path.GetFullPath(packagesPath));

			var storedHash = GetStoredHash();
			var actualHash = FileSystemHasher.HashFilesAndFolders(dataPath, modsPath);

			Trace.WriteLine("Stored: " + storedHash);
			Trace.WriteLine("Actual: " + actualHash);

			if (actualHash == storedHash)
			{
				Trace.WriteLine("Data folder is up-to-date.");
				return;
			}

			if (!Directory.Exists(modsPath))
			{
				Trace.WriteLine("Mods folder not found.");
				return;
			}

			if (!Directory.Exists(packagesPath))
			{
				Trace.WriteLine("Packages folder not found.");
				return;
			}

			using (var modPack = new ModPack(dataPath, packagesPath))
			{
				try
				{
					modPack.GetPackReader();
				}
				catch (IOException)
				{
					Trace.WriteLine("Failed to read pack files.");
					return;
				}

				var modScripts = new ModScripts(modPack);

				Trace.WriteLine("Loading mods...");

				timer.Restart();

				modScripts.LoadLua();
				var errors = modScripts.LoadMods(modsPath);

				timer.Stop();
				Trace.WriteLine(string.Format("Loaded mods in {0}.", timer.Elapsed));

				if (modScripts.ModCount == 0)
				{
					Trace.WriteLine("No mods found.");
					return;
				}

				Trace.WriteLine("Applying mods...");

				timer.Restart();

				try
				{
					modPack.Apply(true);
				}
				catch (Exception ex)
				{
					Trace.WriteLine("Error while applying mods: " + ex);
				}

				timer.Stop();
				Trace.WriteLine(string.Format("Applied mods in {0}.", timer.Elapsed));

				actualHash = FileSystemHasher.HashFilesAndFolders(dataPath, modsPath);
				Trace.WriteLine("New:    " + actualHash);

				if (!errors)
					StoreHash(actualHash);
			}
		}

		private void BtnCreateData_Click(object sender, EventArgs e)
		{
			this.BtnCreateData.Enabled = false;
			this.LoadAndApplyMods();
			this.BtnCreateData.Enabled = true;
		}

		private void BtnEnableDataFiles_Click(object sender, EventArgs e)
		{
			//if (!InsideMabiFolder())
			//{
			//	Trace.WriteLine("Move DataModder to Mabinogi folder to enable data files.");
			//	return;
			//}

			//if (DataPackerInUse())
			//{
			//	Trace.WriteLine("Active DATA packer mods detected, please use a clean client to enable data files with DataModder.");
			//	return;
			//}

			this.Modify();
		}

		private void BtnRemovMods_Click(object sender, EventArgs e)
		{
			//if (!InsideMabiFolder())
			//{
			//	Trace.WriteLine("Move DataModder to Mabinogi folder to enable data files.");
			//	return;
			//}

			//if (DataPackerInUse())
			//{
			//	Trace.WriteLine("Active DATA packer mods detected, please use a clean client to enable data files with DataModder.");
			//	return;
			//}

			this.RemoveMods();
		}

		private void Modify()
		{
			try
			{
				var dataPath = "data";
				var packagesPath = ModPack.GetPackagePath();
				var fileListPath = Path.Combine(ToolFolderName, FileListFileName);
				var fileNames = Directory.EnumerateFiles(dataPath, "*", SearchOption.AllDirectories).Select(a => ModPack.NormalizePath(a).Replace(ModPack.NormalizePath(dataPath + "/"), "")).ToArray();

				if (fileNames == null || !fileNames.Any())
				{
					Trace.WriteLine("Void file name list empty.");
					return;
				}

				foreach (var path in Directory.EnumerateFiles(packagesPath, "*.pack", SearchOption.TopDirectoryOnly).OrderBy(a => a))
				{
					Trace.WriteLine(string.Format("Voiding {0}...", Path.GetFileName(path)));
					PackVoider.ModifyFileNamesInPack(path, fileNames, true);
				}

				Trace.WriteLine("Done voiding file names in pack files, to make client use the files in the data folder.");

				File.WriteAllLines(fileListPath, fileNames);
			}
			catch (IOException ex)
			{
				Trace.WriteLine("Error while modifying pack file: " + ex.Message);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Error while enabling files: " + ex.GetType().Name + Environment.NewLine + ex);
			}
		}

		private void RemoveMods()
		{
			try
			{
				var packagesPath = ModPack.GetPackagePath();

				var fileListPath = Path.Combine(ToolFolderName, FileListFileName);
				string[] fileNames;
				if (File.Exists(fileListPath))
					fileNames = File.ReadAllLines(fileListPath);
				else
					fileNames = new string[0];

				if (fileNames == null || !fileNames.Any())
				{
					Trace.WriteLine("Unvoid file name list empty.");
					return;
				}

				foreach (var path in Directory.EnumerateFiles(packagesPath, "*.pack", SearchOption.TopDirectoryOnly).OrderBy(a => a))
				{
					Trace.WriteLine(string.Format("Unvoiding {0}...", Path.GetFileName(path)));
					PackVoider.ModifyFileNamesInPack(path, fileNames, false);
				}

				Trace.WriteLine("Done unvoiding files.");
			}
			catch (IOException ex)
			{
				Trace.WriteLine("Error while modifying pack file: " + ex.Message);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Error while enabling files: " + ex.GetType().Name + Environment.NewLine + ex);
			}
		}
	}
}
