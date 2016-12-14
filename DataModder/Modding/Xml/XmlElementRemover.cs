namespace DataModder.Modding.Xml
{
	/// <summary>
	/// Element remover for XML files.
	/// </summary>
	public class XmlElementRemover : IMod
	{
		/// <summary>
		/// Modder to execute the removal on.
		/// </summary>
		public XmlModder XmlModder { get; private set; }

		/// <summary>
		/// Path of the elements to remove.
		/// </summary>
		public string Selector { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="modder">Modder to execute the removal on.</param>
		/// <param name="selector">Path of the elements to remove.</param>
		public XmlElementRemover(XmlModder modder, string selector)
		{
			this.XmlModder = modder;
			this.Selector = selector;
		}

		/// <summary>
		/// Executes removal.
		/// </summary>
		public void Process()
		{
			this.XmlModder.RemoveElements(this.Selector);
		}
	}
}
