using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

namespace SharpTranslator;

internal class Translator
{
	private readonly string model;
	private readonly string key;
	private readonly string endpoint;

	public const string PromptTemplate = @"
	don't include original line in result.
	don't explain.
	don't add language prefix.
	keep templates.
	Translate whole line as is.
	Keep case.
	from language ISO 639-1 '{0}' to language ISO 639-1 '{1}'
	SharpSite is an open source content management system website built with C# and Blazor
	you are translating text to be presented on a website to a user
	{2}
	Text to translate: {3}
	";

	public Translator(IConfiguration config)
	{
		model = "gpt-35-turbo-2";
		key = config["key"]; ;
		endpoint = config["endpoint"];
	}

	public async Task<string> TranslateAsync(
		string fromLanguage,
		string toLanguage,
		string resourceDescription,
		string text,
		int maxOutputTokens = 400)
	{

		var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key))
				.AsChatClient(model);

		resourceDescription = string.IsNullOrEmpty(resourceDescription) ? string.Empty : $"the text to translate will be used as {resourceDescription}\n";
		var prompt = string.Format(PromptTemplate, fromLanguage, toLanguage, resourceDescription, text);
		var response = await client.CompleteAsync(prompt, new ChatOptions { MaxOutputTokens = maxOutputTokens });
		return response.Message.Text;
	}

}
