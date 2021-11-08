using System;
using System.Security.Cryptography;
using System.Text;

namespace DIG.GBLXAPI
{
    public static class GBLUtils
    {
		// ------------------------------------------------------------------------
		// Generate random UUID based on a unique name
		// ------------------------------------------------------------------------
		public static string GenerateActorUUID(string name)
		{
			var saltedName = new StringBuilder();
			string salt = UnityEngine.Random.Range(0, 99999).ToString();
			saltedName.Append(salt);
			saltedName.Append(name);

			byte[] bytes = Encoding.UTF8.GetBytes(saltedName.ToString());
			var sha1 = SHA1.Create();
			byte[] hashBytes = sha1.ComputeHash(bytes);

			// convert to hex string
			var sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				var hex = b.ToString("x2");
				sb.Append(hex);
			}

			return sb.ToString();
		}

		// ------------------------------------------------------------------------
		// Catch-all function for getting the IRI associated with any vocabulary term
		//
		// "https://w3id.org/xapi/seriousgames/extensions/progress"
		//
		// type: extension
		// name: progress
		//
		// This is especially useful for adding extensions that are not tracked learning standards and/or extensions that only hold single values,
		// such as the number of collectibles a player got in a level or their progress through the game.
		// ------------------------------------------------------------------------
		public static string GetVocabIRI(string type, string name)
		{
			try
			{
				return (string)GBLXAPI.StandardsJson[type][name]["id"];
			}
			catch (NullReferenceException)
			{
				if (GBLXAPI.StandardsJson[type] == null) { throw new VocabMissingException("type", type); }
				else if (GBLXAPI.StandardsJson[type][name] == null) { throw new VocabMissingException("vocab term", name); }
				return null;
			}
		}
	}
}
