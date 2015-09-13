// Guids.cs
// MUST match guids.h
using System;

namespace PhraseApp.PhraseApp
{
    static class GuidList
    {
        public const string guidPhraseAppPkgString = "a4ed2e67-c10d-4858-bd22-bf9e2255aa46";
        public const string guidPhraseAppCmdSetString = "7847c596-00e1-44a6-acb9-afe4c700313d";

        public static readonly Guid guidPhraseAppCmdSet = new Guid(guidPhraseAppCmdSetString);
    };
}