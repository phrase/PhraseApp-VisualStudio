# PhraseApp Plugin for Visual Studio

This Plugin integrates [PhraseApp](https://phraseapp.com) into Visual Studio. The Plugin works with any Windows Store app, including: Windows Phone 8.1 apps, Windows Grid and Hub apps ("Metro").

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
