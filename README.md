# README

This is just a quick app done in a couple of hours, This is just meant to work.
This is an app that just runs queries and then outputs them in a Raylib GUI window.

## Building

`dotnet build --project AzureDevOpsUI/AzureDevOpsUI.csproj -c Release --self-contained true -o c:/tools/AzureDevopsUI`

## Config

You will need to put a config.json in the directory that you put the executable
If you want to use your own font, please put the font in the `resources/` directory and set the name to the ttf file.

```json
{
  "Font": "Font.ttf",
  "FontSize": 18,
  "CollectionUri": "https://dev.azure.com/[your org]",
  "PatToken": "your pat token",
  "DefaultProject": "[Project with the query you want to run]",
  "Queries": ["Path/To/Query"]
}
```

## Mentions

- [Lilex Nerd Font](https://github.com/ryanoasis/nerd-fonts/releases/download/v3.3.0/Lilex.zip)
- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack)
- [Raylib-CSharp-Vinculum](https://www.nuget.org/packages/Raylib-CSharp-Vinculum)
- [Microsoft.TeamFoundationServer.Client](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client)
- [Microsoft.VisualStudio.Services.Client](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.Client)
