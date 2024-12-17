using Microsoft.Extensions.Configuration;
using SharpTranslator;

var config = new ConfigurationBuilder()
	.AddUserSecrets<Program>()
	.Build();

var tx = new Translator(config);

var fromFile = args[0];
var toFile = args[1];

// capture a variable called 'fromLanguage' from the fromFile using standard resx file naming
var fromLanguage = Path.GetFileNameWithoutExtension(fromFile).Split('.').Last();
var toLanguage = Path.GetFileNameWithoutExtension(toFile).Split('.').Last();

var resources = ResxManager.Read(fromFile);
var toResources = ResxManager.Read(toFile);

var missingResources = resources.Where(r => !toResources.Any(tr => tr.Name.Equals(r.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();

var newResources = new List<Resource>();
foreach (var resource in missingResources)
{
	var translatedValue = await tx.TranslateAsync(fromLanguage, toLanguage, "SharpSite", resource.Value);
	newResources.Add(new Resource(resource.Name, translatedValue, string.Empty));
	Console.WriteLine($"Translated {resource.Name} from {fromLanguage} to {toLanguage}");
}

ResxManager.Write(toFile, newResources);