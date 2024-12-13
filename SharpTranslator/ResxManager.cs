using System.Xml;

namespace SharpTranslator;

internal class ResxManager
{

	public static void Write(string filePath, Dictionary<string, string> dic)
	{
		if (dic.Count == 0)
		{
			return;
		}

		var xmlDocument = new XmlDocument();
		xmlDocument.Load(filePath);

		var nodes = xmlDocument.SelectNodes("/root/data");
		foreach (XmlNode node in nodes)
		{
			var key = node.Attributes["name"].Value;
			if (dic.TryGetValue(key, out var value))
			{
				var valueNode = node.SelectSingleNode("value");
				valueNode.InnerText = value;
				dic.Remove(key);
				continue;
			}
		}

		foreach (var pair in dic)
		{
			var dataNode = xmlDocument.CreateElement("data");
			var nameAttr = xmlDocument.CreateAttribute("name");
			var xmlSpaceAttr = xmlDocument.CreateAttribute("xml:space");

			xmlSpaceAttr.Value = "preserve";
			nameAttr.Value = pair.Key;
			dataNode.Attributes.Append(nameAttr);
			dataNode.Attributes.Append(xmlSpaceAttr);

			var valueNode = xmlDocument.CreateElement("value");
			valueNode.InnerText = pair.Value;
			dataNode.AppendChild(valueNode);

			xmlDocument.DocumentElement.AppendChild(dataNode);
		}

		xmlDocument.Save(filePath);
	}

	public static Dictionary<string, string> Read(string filePath)
	{
		var dict = new Dictionary<string, string>();

		var xmlDocument = new XmlDocument();
		xmlDocument.Load(filePath);

		var nodes = xmlDocument.SelectNodes("/root/data");
		foreach (XmlNode node in nodes)
		{
			var key = node.Attributes["name"].Value;
			var value = node.SelectSingleNode("value").InnerText;
			dict.Add(key, value);
		}

		return dict;
	}
}

}
