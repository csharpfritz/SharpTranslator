using Microsoft.Extensions.Configuration;
using SharpTranslator;

var config = new ConfigurationBuilder()
	.AddUserSecrets<Program>()
	.Build();

var tx = new Translator(config);

