namespace DataModder.Modding.Xml
{
	/// <summary>
	/// Replacer mod for XML files.
	/// </summary>
	public class XmlReplacer : IMod
	{
		/// <summary>
		/// The modder to execute the replace on.
		/// </summary>
		public XmlModder XmlModder { get; private set; }

		/// <summary>
		/// Regular expression string to search for.
		/// </summary>
		public string Search { get; private set; }

		/// <summary>
		/// String to replace with.
		/// </summary>
		public string Replace { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="modder">Modder to execute the replace on.</param>
		/// <param name="search">Regular expression to search for.</param>
		/// <param name="replace">String to replace matches with.</param>
		public XmlReplacer(XmlModder modder, string search, string replace)
		{
			this.XmlModder = modder;
			this.Search = search;
			this.Replace = replace;
		}

		/// <summary>
		/// Executes replace.
		/// </summary>
		public void Process()
		{
			this.XmlModder.RegexReplace(this.Replace, this.Search);
		}
	}
}
