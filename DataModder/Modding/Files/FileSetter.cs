using System.IO;

namespace DataModder.Modding.Files
{
	/// <summary>
	/// File setter mod.
	/// </summary>
	public class FileSetter : IMod
	{
		/// <summary>
		/// Path to write data to.
		/// </summary>
		public string DestinationFilePath { get; private set; }

		/// <summary>
		/// Data to write.
		/// </summary>
		public byte[] Content { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="destinationFilePath">Path to write the data to.</param>
		/// <param name="data">File data to write into destination path.</param>
		public FileSetter(string destinationFilePath, byte[] data)
		{
			this.DestinationFilePath = destinationFilePath;
			this.Content = data;
		}

		/// <summary>
		/// Executes mod, writing the data to the destination file.
		/// Overwrites file if it already exists.
		/// </summary>
		public void Process()
		{
			ModPack.PrepareFolder(this.DestinationFilePath);

			File.WriteAllBytes(this.DestinationFilePath, this.Content);
		}
	}
}
