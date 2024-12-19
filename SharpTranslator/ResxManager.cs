using System.Xml;

namespace SharpTranslator;

internal class ResxManager
{

	public static void Write(string filePath, List<Resource> resources)
	{
		if (!resources.Any()) return;

		var xmlDocument = new XmlDocument();
		xmlDocument.Load(filePath);

		var nodes = xmlDocument.SelectNodes("/root/data");
		foreach (XmlNode node in nodes)
		{
			var key = node.Attributes["name"].Value;
			if (resources.Any(r => r.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
			{
				var resource = resources.First(r => r.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
				var valueNode = node.SelectSingleNode("value");
				valueNode.InnerText = resource.Value;
				resources.Remove(resource);

				// clean up any garbage from the ResourceEditor
				if (node.Attributes["type"] != null)
				{
					node.Attributes.Remove(node.Attributes["type"]);
				}

				// add a xml:space attribute to the value node if missing
				if (valueNode.Attributes["xml:space"] == null)
				{
					var xmlSpaceAttr = xmlDocument.CreateAttribute("xml:space");
					xmlSpaceAttr.Value = "preserve";
					valueNode.Attributes.Append(xmlSpaceAttr);
				}

				continue;
			}
		}

		foreach (var pair in resources)
		{
			var dataNode = xmlDocument.CreateElement("data");
			var nameAttr = xmlDocument.CreateAttribute("name");
			var xmlSpaceAttr = xmlDocument.CreateAttribute("xml:space");

			xmlSpaceAttr.Value = "preserve";
			nameAttr.Value = pair.Name;
			dataNode.Attributes.Append(nameAttr);
			dataNode.Attributes.Append(xmlSpaceAttr);

			var valueNode = xmlDocument.CreateElement("value");
			valueNode.InnerText = pair.Value;
			dataNode.AppendChild(valueNode);

			// add a comment element to indicate that this is an AI generated translation
			var commentNode = xmlDocument.CreateElement("comment");
			commentNode.InnerText = "AI generated translation";
			dataNode.AppendChild(commentNode);

			xmlDocument.DocumentElement.AppendChild(dataNode);
		}

		xmlDocument.Save(filePath);
	}

	public static List<Resource> Read(string filePath)
	{
		var outList = new List<Resource>();

		var xmlDocument = new XmlDocument();
		xmlDocument.Load(filePath);

		var nodes = xmlDocument.SelectNodes("/root/data");
		foreach (XmlNode node in nodes)
		{
			var key = node.Attributes["name"].Value;
			var value = node.SelectSingleNode("value").InnerText;
			var comment = node.SelectSingleNode("comment")?.InnerText;
			outList.Add(new Resource(key, value, comment));
		}

		return outList;
	}
}

public record Resource(string Name, string Value, string Comment);