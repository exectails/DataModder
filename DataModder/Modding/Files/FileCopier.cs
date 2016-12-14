using System.IO;

namespace DataModder.Modding.Files
{
	/// <summary>
	/// File copy mod.
	/// </summary>
	public class FileCopier : IMod
	{
		/// <summary>
		/// Path of the file to copy.
		/// </summary>
		public string SourceFilePath { get; private set; }

		/// <summary>
		/// Path the file is to be copied to.
		/// </summary>
		public string DestinationFilePath { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="sourceFilePath">Path of the file to copy.</param>
		/// <param name="destinationFilePath">Path the file is to be copied to.</param>
		public FileCopier(string sourceFilePath, string destinationFilePath)
		{
			this.SourceFilePath = sourceFilePath;
			this.DestinationFilePath = destinationFilePath;
		}

		/// <summary>
		/// Executes mod, copying the file to its destination. Overwrites
		/// existing files.
		/// </summary>
		public void Process()
		{
			ModPack.PrepareFolder(this.DestinationFilePath);

			File.Copy(this.SourceFilePath, this.DestinationFilePath, true);
		}
	}
}
