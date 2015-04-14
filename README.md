# PhraseApp Plugin for Visual Studio

This Plugin integrates [PhraseApp](https://phraseapp.com) into Visual Studio. The Plugin works with any Windows Store app, including: Windows Phone 8.1 apps, Windows Grid and Hub apps ("Metro").

## How To Build

Requirements:
  * Windows 8.1(!)
  * Visual Studio 2013 Community Edition(!) Update 4(!) or higher
  * Visual Studio SDK
  * .NET 4.5 or higher

Building: 
 1. Open PhraseApp.sln file in VS
 2. Tools -> Options -> Manage NuGet Packages -> Install RestSharp and JSON.NET
 3. Compile
 
Releasing:
 1. Use blank VSIX Project template as described here: https://msdn.microsoft.com/en-us/library/dd393742.aspx
 2. Open VSIX Project template, add PhraseApp Project to the Solution
 3. Export to ".zip" (VSIX) as described here: https://msdn.microsoft.com/en-us/library/ff407026.aspx
 4. Upload to Visual Studio Gallery https://visualstudiogallery.msdn.microsoft.com/ (Get Login credentials from Manuel)
 
Commons Bugs:
* **Bug:** Signing Error "Assembly Name to large", **Solution:** Remove your Signing Key (.skn) from third-party packages (RestSharp, JSON.NET)

## Requirements

Requirements:
  * Visual Studio 2013 or higher
  * .NET 4.5 or higher
  * RestSharp, JSON.NET (via NuGet)

## Install

Download the Plugin from the Visual Studio Gallery and folllow the instructions.

## Usage

Visual Studio uses ISO 639-1 language codes, for example: "en-US", "en-GB", "de-DE"... The locales created in your PhraseApp project must be named in the same format.

Set the PhraseApp ProjectID and API Access Token via *Tools -> Options -> PhraseApp*.

Upload locale files via *Tools -> PhraseApp Upload*

Download locales via *Tools -> PhraseApp Download*
