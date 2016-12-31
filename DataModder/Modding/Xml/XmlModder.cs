using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DataModder.Modding.Xml
{
	/// <summary>
	/// Wrapper around XDocument to easily modify an XML document.
	/// </summary>
	public class XmlModder
	{
		private XDocument _doc;

		/// <summary>
		/// Loads given XML file.
		/// </summary>
		/// <param name="filePath"></param>
		public void LoadFromFile(string filePath)
		{
			_doc = XDocument.Load(filePath);
		}

		/// <summary>
		/// Loads given XML string.
		/// </summary>
		/// <param name="str"></param>
		public void LoadFromString(string str)
		{
			_doc = XDocument.Parse(str);
		}

		/// <summary>
		/// Load XML code from given stream.
		/// </summary>
		/// <param name="stream"></param>
		public void LoadFromStream(Stream stream)
		{
			_doc = XDocument.Load(stream);
		}

		/// <summary>
		/// Throws InvalidOperationException if no document was loaded yet.
		/// </summary>
		private void AssertLoaded()
		{
			if (_doc == null)
				throw new InvalidOperationException("No XML document has been loaded.");
		}

		/// <summary>
		/// Saves modified XML document to the given path. Overwrites file
		/// if it already exists.
		/// </summary>
		/// <param name="filePath"></param>
		public void Save(string filePath)
		{
			this.Save(filePath, Encoding.UTF8);
		}

		/// <summary>
		/// Saves modified XML document to the given path, using a specific
		/// encoding. Overwrites file if it already exists.
		/// </summary>
		/// <param name="filePath"></param>
		public void Save(string filePath, Encoding encoding)
		{
			this.AssertLoaded();

			var settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			settings.Encoding = encoding;

			using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			using (var writer = XmlTextWriter.Create(fs, settings))
			{
				_doc.Save(writer);
			}
		}

		/// <summary>
		/// Returns modified XML document as string.
		/// </summary>
		/// <returns></returns>
		public string GetString()
		{
			this.AssertLoaded();

			return _doc.ToString();
		}

		/// <summary>
		/// Sets the given attribute on the paths matching the given one.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <example>
		/// // Change name to test
		/// SetAttribute("//Item[@Id=122]", "Name", 'test');
		/// </example>
		public void SetAttribute(string path, string name, object value)
		{
			this.AssertLoaded();

			var elements = _doc.XPathSelectElements(path);
			foreach (var element in elements)
			{
				element.SetAttributeValue(name, value);
			}
		}

		/// <summary>
		/// Adds element at path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="element"></param>
		/// <example>
		/// // Add an item
		/// AddElement("/Items", "<Item Id='123' Name='test' />");
		/// </example>
		public void AddElement(string path, XElement addElement)
		{
			this.AssertLoaded();

			var elements = _doc.XPathSelectElements(path);
			foreach (var element in elements)
			{
				element.Add(addElement);
			}
		}

		/// <summary>
		/// Removes given elements.
		/// </summary>
		/// <param name="path"></param>
		/// <example>
		/// RemoveElements("//Item[@Id=123]");
		/// </example>
		public void RemoveElements(string path)
		{
			this.AssertLoaded();

			var elements = _doc.XPathSelectElements(path);
			elements.Remove();
		}

		/// <summary>
		/// Executes Regex.Replace on modified XML. (slow!)
		/// </summary>
		/// <param name="search"></param>
		/// <param name="replace"></param>
		public void RegexReplace(string search, string replace)
		{
			this.AssertLoaded();

			var str = this.GetString();
			str = System.Text.RegularExpressions.Regex.Replace(str, search, replace);

			// Parsing again is obviously pretty slow, this could be improved
			// by caching the string and only parsing it again once the
			// tree is needed.
			this.LoadFromString(str);
		}
	}
}
