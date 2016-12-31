using System.Xml.Linq;
namespace DataModder.Modding.Xml
{
	/// <summary>
	/// Element adder mod for XML files.
	/// </summary>
	public class XmlElementAdder : IMod
	{
		/// <summary>
		/// Modder to execute the adding on.
		/// </summary>
		public XmlModder XmlModder { get; private set; }

		/// <summary>
		/// Path to insert at.
		/// </summary>
		public string Selector { get; private set; }

		/// <summary>
		/// XML node to add.
		/// </summary>
		public XElement XmlElement { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="modder">Modder to execute the adding on.</param>
		/// <param name="selector">Path to insert at.</param>
		/// <param name="xml">XML element to add.</param>
		public XmlElementAdder(XmlModder modder, string selector, XElement xml)
		{
			this.XmlModder = modder;
			this.Selector = selector;
			this.XmlElement = xml;
		}

		/// <summary>
		/// Executes adding.
		/// </summary>
		public void Process()
		{
			this.XmlModder.AddElement(this.Selector, this.XmlElement);
		}
	}
}
