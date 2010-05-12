// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Keystore
{

	public abstract class TPMKeystoreProvider : IDisposable
	{
		public TPMKeystoreProvider ()
		{
		}
		
		
		/// <summary>
		/// Gets the overal count of recorded keys
		/// </summary>
		public abstract Int64 KeyCount{ get; }
		/// <summary>
		/// Lists all unique identifiers of all stored keys
		/// </summary>
		/// <remarks>
		/// The unique identifiers get generated by the tpm server. This
		/// identifiers are unique for all keys generated by a single tpm
		/// </remarks>
		/// <returns></returns>
		public abstract string[] EnumerateIdentifiers();
		
		/// <summary>
		/// Lists all unique friendly names of all stored keys
		/// </summary>
		/// <remarks>
		/// The friendly names are also unique, but only in the scope of
		/// this tpm keystore
		/// </remarks>
		/// <returns></returns>
		public abstract string[] EnumerateFriendlyNames();
		
		/// <summary>
		/// Looks for the parent key of the key with the specified friendly name
		/// </summary>
		/// <param name="friendlyName"></param>
		/// <returns>@key: friendlyName of parent key, @value: identifier of parent key</returns>
		public abstract KeyValuePair<string, string>? FindParentKeyByFriendlyName(string friendlyName);
		
		/// <summary>
		/// Looks for the parent key of the key with the specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns>@key: friendlyName of parent key, @value: identifier of parent key</returns>
		public abstract KeyValuePair<string, string>? FindParentKeyByIdentifier(string identifier);
		
		/// <summary>
		/// Retrieves the key blob of the key with the specified key identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public abstract byte[] GetKeyBlob(string identifier);
		
		/// <summary>
		/// Gets the identifier for the specified friendly name
		/// </summary>
		/// <param name="friendlyName"></param>
		/// <returns></returns>
		public abstract string FriendlyNameToIdentifier(string friendlyName);
		
		/// <summary>
		/// Gets the friendlyName for the specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public abstract string IdentifierToFriendlyName(string identifier);
		
		
		/// <summary>
		/// Adds the specified key to the key cache
		/// </summary>
		/// <param name="friendlyName">Friendly name of the new key (user assigned but unique)</param>
		/// <param name="identifier">Identifier for the new key (server assigned and unique)</param>
		/// <param name="parentFriendlyName">Friendly name of the parent key</param>
		/// <param name="keyData">Key data to save</param>
		public abstract void AddKey(string friendlyName, string identifier, string parentFriendlyName, byte[] keyData);
		
		/// <summary>
		/// Removes the key with the specified friendly name
		/// </summary>
		/// <param name="friendlyName"></param>
		public abstract void RemoveKey(string friendlyName);
		
		/// <summary>
		/// Returns if the keystore contains a key with the specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public abstract bool ContainsIdentifier(string identifier);
		
		#region IDisposable implementation
		public virtual void  Dispose ()
		{
		}
		#endregion
		
		
		
	}
}
