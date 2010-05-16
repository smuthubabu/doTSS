// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using Iaik.Utils.Hash;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

namespace Iaik.Tc.TPM.Context
{

	/// <summary>
	/// Performs operations with tp keys
	/// </summary>
	public class TPMKeyClient
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private TPMSession _tpmSession;

		public TPMKeyClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}		
		
		
		/// <summary>
		/// Gets the Key handle by friendly Name
		/// </summary>
		/// <param name="friendlyName"></param>
		/// <returns></returns>
		public ClientKeyHandle GetKeyHandleByFriendlyName(string friendlyName)
		{
			string identifier = _tpmSession.Keystore.FriendlyNameToIdentifier(friendlyName);
			
			if(friendlyName == null || identifier == null)
				return null;
			
			return new ClientKeyHandle(friendlyName, identifier, _tpmSession);
		}
		
		/// <summary>
		/// Gets the key handle by global identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public ClientKeyHandle GetKeyHandleByIdentifier(string identifier)
		{
			string friendlyName = _tpmSession.Keystore.IdentifierToFriendlyName(identifier);
			
			if(friendlyName == null || identifier == null)
				return null;
			
			return new ClientKeyHandle(friendlyName, identifier, _tpmSession);
		}
		
		
		/// <summary>
		/// Returns the SRK key handle, it is not checked at this point if the
		/// authorized user is allowed to use the SRK
		/// </summary>
		/// <returns>
		/// A <see cref="ClientKeyHandle"/>
		/// </returns>
		public ClientKeyHandle GetSrkKeyHandle()
		{
			return new ClientKeyHandle("srk", "srk", _tpmSession);
		}
		
		
	}
	
	public class ClientKeyHandle
	{
		/// <summary>
		/// The session this key handle belongs to
		/// </summary>
		private TPMSession _tpmSession;
		
	    /// <summary>
		/// Unique identifier of the TPM key
		/// </summary>
		private string _keyIdentifier;
		
		/// <summary>
		/// Gets the unique key identifier, generated by the server
		/// (hash of the generated TPM_KEY structure)
		/// </summary>
		public string KeyIdentifier
		{
			get{ return _keyIdentifier; }
		}
		
		private string _friendlyName;
		
		/// <summary>
		/// Gets the friendly name of the key
		/// </summary>
		/// <remarks>
		/// The friendly name of a key is also a unique name, but only in the scope 
		/// of the used key storage. 
		///</remarks>
		public string FriendlyName
		{
			get{ return _friendlyName; }
		}
		
		public ClientKeyHandle(string friendlyName, string identifier, TPMSession tpmSession)
		{
			_friendlyName = friendlyName;
			_keyIdentifier = identifier;
			_tpmSession = tpmSession;
		}
		
		/// <summary>
		/// Creates a new key with the specified parameters and adds it to the key storage
		/// </summary>
		/// <param name="friendlyName">Unique name in the current keystore</param>
		/// <param name="keyUsage">Specifies the keyUsage of the new key</param>
		/// <param name="keyFlags">Some additional flags</param>
		/// <returns>Returns the newly created key</returns>
		public ClientKeyHandle CreateKey(string friendlyName, TPMKeyUsage keyUsage)
		{
			return CreateKey(friendlyName, keyUsage, TPMKeyFlags.None);
		}
		
		public ClientKeyHandle CreateKey(string friendlyName, TPMKeyUsage keyUsage, TPMKeyFlags keyFlags)
		{
			return CreateKey(friendlyName, 2048, keyUsage, keyFlags);
		}
		
		public ClientKeyHandle CreateKey(string friendlyName, uint keyLength, TPMKeyUsage keyUsage)
		{
			return CreateKey(friendlyName, keyLength, keyUsage, TPMKeyFlags.None);
		}
		
		public ClientKeyHandle CreateKey(string friendlyName, uint keyLength, TPMKeyUsage keyUsage, TPMKeyFlags keyFlags)
		{
			Parameters paramsCreateWrapKey = new Parameters();
			paramsCreateWrapKey.AddPrimitiveType("parent", KeyIdentifier);
			paramsCreateWrapKey.AddPrimitiveType("key_usage", keyUsage);
			paramsCreateWrapKey.AddPrimitiveType("key_flags", keyFlags);
			paramsCreateWrapKey.AddPrimitiveType("key_length", keyLength);
			paramsCreateWrapKey.AddPrimitiveType("exponent", new byte[0]);
			paramsCreateWrapKey.AddPrimitiveType("num_primes", (uint)0);
			
			Parameters parameters = new Parameters();
			parameters.AddPrimitiveType("identifier", friendlyName);
			
			ProtectedPasswordStorage authUsage = _tpmSession.RequestSecret(
				new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters));
			authUsage.DecryptHash();
			paramsCreateWrapKey.AddPrimitiveType("usage_auth", authUsage.HashValue);
			
			ProtectedPasswordStorage authMigration = null;				
			
			if((keyFlags & TPMKeyFlags.Migratable) == TPMKeyFlags.Migratable)
			{
				authMigration = _tpmSession.RequestSecret(
					new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyMigrationSecret, parameters));
				authMigration.DecryptHash();
				paramsCreateWrapKey.AddPrimitiveType("migration_auth", authMigration.HashValue);
			}	
			else
				paramsCreateWrapKey.AddPrimitiveType("migration_auth", new byte[20]);
			
			try
			{
				TPMCommandResponse responseCreateWrapKey = 
					BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_CreateWrapKey, paramsCreateWrapKey);
				
				_tpmSession.Keystore.AddKey(
				            friendlyName,
				            responseCreateWrapKey.Parameters.GetValueOf<string>("key_identifier"),
				            this.FriendlyName,
				            responseCreateWrapKey.Parameters.GetValueOf<byte[]>("key_data"));
				                            
				return new ClientKeyHandle(friendlyName, responseCreateWrapKey.Parameters.GetValueOf<string>("key_identifier"), _tpmSession);
			}
			finally
			{
				if(authMigration != null)
					authMigration.ClearHash();
					
				if(authUsage != null)
					authUsage.ClearHash();
			}
		}
		
		private TPMCommandResponse BuildDoVerifyRequest (string commandIdentifier, Parameters parameters)
		{
			TPMCommandRequest versionRequest = new TPMCommandRequest (commandIdentifier, parameters);
			TPMCommandResponse response = _tpmSession.DoTPMCommandRequest (versionRequest);
			
			if (response.Status == false)
				throw new TPMRequestException ("An unknown tpm error occured");
			
			return response;
		}
	}
}