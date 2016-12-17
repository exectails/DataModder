using DataModder.Modding;
using DataModder.Modding.Files;
using DataModder.Modding.Xml;
using MeluaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DataModder
{
	public class ModScripts
	{
		private IntPtr L;

		private ModPack _modPack;
		private string _cwd;
		private string _loadedXmlFile;
		private XmlModder _loadedXmlModder;

		public int ModCount { get; private set; }

		public ModScripts(ModPack modPack)
		{
			_modPack = modPack;
		}

		private bool IsInsideCwd(string path)
		{
			var fullPath = Path.GetFullPath(Path.Combine(_cwd, path));
			var cwd = Path.GetFullPath(_cwd);

			fullPath = ModPack.NormalizePath(fullPath);
			cwd = ModPack.NormalizePath(cwd);

			return (fullPath.StartsWith(cwd));
		}

		private bool FileExistsInPackages(string filePath)
		{
			var packReader = _modPack.GetPackReader();
			return packReader.Exists(filePath);
		}

		private bool IsXmlFileLoaded()
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

		public void LoadLua()
		{
			if (L != IntPtr.Zero)
				Melua.lua_close(L);

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

		public bool LoadMods(string modPath)
		{
			var errors = false;

			var mainModFiles = Directory.EnumerateFiles(modPath, "main.lua", SearchOption.AllDirectories);
			if (!mainModFiles.Any())
				return false;

			Trace.WriteLine("Mods found: " + mainModFiles.Count());

			foreach (var filePath in mainModFiles)
			{
				var folderPath = (Path.GetDirectoryName(filePath));
				var folderName = Path.GetFileName(folderPath);

				_cwd = Path.GetFullPath(folderPath);

				Trace.WriteLine(string.Format("Loading '{0}'...", folderName));

				if (Melua.luaL_dofile(L, filePath) != 0)
				{
					Trace.WriteLine(string.Format("Error in {1}", folderName, Melua.lua_tostring(L, -1)));
					errors = true;
				}
				else
				{
					this.ModCount++;
				}
			}

			return errors;
		}

		private int loadxmlfile(IntPtr L)
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

		private int setattributes(IntPtr L)
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

		private int addelement(IntPtr L)
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

		private int removeelements(IntPtr L)
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

		private int replacefile(IntPtr L)
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

		private int replace(IntPtr L)
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

		private int include(IntPtr L)
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
