/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;
using Iaik.Tc.TPM.Keystore;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.PCRData;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_key")]
    public class TPMSessionKeyCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_key Args: [local_session_alias] [command] {args}
    Specify the tpm to use by [local_session_alias]. These aliases can be defined using the tpm_select command.
    Available commands:
        keystore_open    args: <keystore identifier> always first argument (SQLiteKeystore)
                               keystore specific arguments name1=value1,name2=value2,...    
    
                         Opens a keystore with the specified parameters
    
    	keystore_close   closes an open keystore
    	
    	keystore_list    lists all keys saved in the keystore

        create           args: name=<name>,
                               parent=<parent friendly name>,
                               key_length={...,512,1024,2048,...},
                               key_usage={signing,storage,bind},
                               [key_flags={none,migratable}]
                               
    	";    	
    
    
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        	{
        		_console.Out.WriteLine ("Error: [local_session_alias] not specified");
        		return;
        	}
        	else if (commandline.Length < 3)
        	{
        		_console.Out.WriteLine ("Error: [command] not specified");
        		return;
        	}

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string keyCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}    
			
			
			if (keyCommand == "keystore_open")
			{
        		if(commandline.Length < 4)
    			{
    				_console.Out.WriteLine("Error: keystore_open requires some arguments, check help for further information");
    				return;
    			}				
				
    			if(tpmSessions[localAlias].Keystore != null)
    			{
    				_console.Out.WriteLine("Error: There is already an open keystore!");
    				return;
    			}
    			
    			string[] sArguments = commandline[3].Split(',');
    			IDictionary<string, string> arguments = _console.SplitArguments(commandline[3], 1);    			
    			
    			TPMKeystoreProvider keystore = TPMKeystoreProviders.Create(sArguments[0], arguments);	
    			tpmSessions[localAlias].Keystore = keystore;
    			
        	}
        	else if (keyCommand == "keystore_close")
			{
				TPMKeystoreProvider keystore = tpmSessions[localAlias].Keystore;
				
				if(keystore != null)
				{
					keystore.Dispose();
					tpmSessions[localAlias].Keystore = keystore;
				}
				
			}
			else if(keyCommand == "keystore_list")
			{
				TPMKeystoreProvider keystore = tpmSessions[localAlias].Keystore;
				
				if(keystore == null)
				{
					_console.Out.WriteLine("Error: No keystore opened");
					return;
				}
				
				_console.Out.WriteLine("The keystore contains #{0} keys", keystore.EnumerateFriendlyNames().Length);
				_console.Out.WriteLine();
				foreach(string friendlyName in keystore.EnumerateFriendlyNames())
				{
					string parent = "<SRK>";
					
					KeyValuePair<string, string>? parentKey = keystore.FindParentKeyByFriendlyName(friendlyName);
					if(parentKey != null)
						parent = parentKey.Value.Key;
					
					_console.Out.WriteLine("{0}   ({1})   parent: {2}", friendlyName, 
						keystore.FriendlyNameToIdentifier(friendlyName), parent);
						
				}
			}
			else if(keyCommand == "create")
			{
				if(tpmSessions[localAlias].Keystore == null)
    			{
    				_console.Out.WriteLine("Error: No keystore was opened");
    				return;
    			}
				
				IDictionary<string, string> arguments = _console.SplitArguments(commandline[3], 0);
				
				if(arguments.ContainsKey("name") == false)
				{
					_console.Out.WriteLine("Error: No name specified");
					return;
				}
				
				if(arguments.ContainsKey("parent") == false)
				{
					_console.Out.WriteLine("Error: No parent key specified");
					return;
				}
				
				ClientKeyHandle keyHandle;
				if(arguments["parent"] == "srk")
					keyHandle = tpmSessions[localAlias].KeyClient.GetSrkKeyHandle();
				else
					keyHandle = tpmSessions[localAlias].KeyClient.GetKeyHandleByFriendlyName(arguments["parent"]);
				
				if(keyHandle == null)
				{
					_console.Out.WriteLine("Error: Key with name '{0}' not found", arguments["parent"]);
					return;
				}
					
				
				if(arguments.ContainsKey("key_usage") == false)
				{
					_console.Out.WriteLine("Error: key_usage not defined");
					return;
				}
				
				TPMKeyUsage keyUsage;
				
				switch(arguments["key_usage"])
				{
				case "sign":
					keyUsage = TPMKeyUsage.TPM_KEY_SIGNING;
					break;
					
				case "bind":
					keyUsage = TPMKeyUsage.TPM_KEY_BIND;
					break;
					
				case "storage":
					keyUsage = TPMKeyUsage.TPM_KEY_STORAGE;
					break;
					
				default:
					_console.Out.WriteLine("Error: Invalid key_usage '{0}'", arguments["key_usage"]);
					return;
				}
				
				if(arguments.ContainsKey("key_length") == false)
				{
					_console.Out.WriteLine("Error: key_length not defined!");
					return;
				}
				
				uint keyLength = 0;
				if(uint.TryParse(arguments["key_length"], out keyLength) == false)
				{
					_console.Out.WriteLine("Error: Could not parse key_length");
					return;
				}				
				
				TPMKeyFlags keyFlags = TPMKeyFlags.None;
				
				if(arguments.ContainsKey("key_flags"))
				{
					switch(arguments["key_flags"])
					{
					case "none":
						keyFlags = TPMKeyFlags.None;
						break;
					
					case "migratable":
						keyFlags = TPMKeyFlags.Migratable;
						break;
					}
				}
				
				//Make sure that key usage auth and if required migration auth is in the secret cache
				Parameters hmacParams = new Parameters();
				hmacParams.AddPrimitiveType("identifierIsFriendlyName", true);
				hmacParams.AddPrimitiveType("identifier", arguments["name"]);
				ProtectedPasswordStorage usageAuth = tpmSessions[localAlias].RequestSecret(
					new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, hmacParams));
					
				if(usageAuth != null)
					tpmSessions[localAlias].SetValue("secret_usage_" + arguments["name"], usageAuth);
					
				if((keyFlags & TPMKeyFlags.Migratable) == TPMKeyFlags.Migratable)
				{
					ProtectedPasswordStorage migrationAuth = tpmSessions[localAlias].RequestSecret(
						new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyMigrationSecret, hmacParams));
						
					if(migrationAuth != null)
						tpmSessions[localAlias].SetValue("secret_migration_" + arguments["name"], migrationAuth);					
				}
				
				
				/*ClientKeyHandle newKey =*/ keyHandle.CreateKey(arguments["name"], keyLength, keyUsage, keyFlags);
			}
			else
        		_console.Out.WriteLine ("Error, unknown command '{0}'", commandline[2]);
        }
        
    }
}
