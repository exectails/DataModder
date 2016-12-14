namespace DataModder.Modding.Xml
{
	/// <summary>
	/// Attribute setter mod for XML files.
	/// </summary>
	public class XmlAttributeSetter : IMod
	{
		/// <summary>
		/// Modder to execute the setting on.
		/// </summary>
		public XmlModder XmlModder { get; private set; }

		/// <summary>
		/// Path for the elements to change attribute on.
		/// </summary>
		public string Selector { get; private set; }

		/// <summary>
		/// Name of the attribute to set.
		/// </summary>
		public string AttributeName { get; private set; }

		/// <summary>
		/// New value of the attribute.
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="modder">Modder to use.</param>
		/// <param name="selector">Path of the elements to modify.</param>
		/// <param name="attributeName">Name of the attribute to set.</param>
		/// <param name="value">New value of the attribute, use null to remove it.</param>
		public XmlAttributeSetter(XmlModder modder, string selector, string attributeName, string value)
		{
			this.XmlModder = modder;
			this.Selector = selector;
			this.AttributeName = attributeName;
			this.Value = value;
		}

		/// <summary>
		/// Executes setting.
		/// </summary>
		public void Process()
		{
			this.XmlModder.SetAttribute(this.Selector, this.AttributeName, this.Value);
		}
	}
}
