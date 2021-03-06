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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>


using System;

namespace Iaik.Tc.TPM.Library.Common
{
	public static class TPMCommandNames
	{
		#region Admin Ownership
		public const string TPM_CMD_TakeOwnership = "TPM_CMD_TakeOwnership";
		public const string TPM_CMD_OwnerClear = "TPM_CMD_OwnerClear";
		#endregion
		
		#region Integrity Collection and Reporting
		public const String TPM_CMD_PCRRead = "TPM_CMD_PCRRead";
		public const string TPM_CMD_Extend = "TPM_CMD_Extend";
		public const string TPM_CMD_Quote = "TPM_CMD_Quote";
		#endregion
		
		#region Capability Commands
		public const String TPM_CMD_GetCapability = "TPM_CMD_GetCapability";
		#endregion
		
		#region Endorsement key handling
		public const string TPM_CMD_ReadPubek = "TPM_CMD_ReadPubek";
		#endregion
		
		#region Authorization sessions
		public const string TPM_CMD_OIAP = "TPM_CMD_OIAP";
		public const string TPM_CMD_OSAP = "TPM_CMD_OSAP";
		#endregion
		
		#region Eviction
		public const string TPM_CMD_FlushSpecific = "TPM_CMD_FlushSpecific";
		#endregion
		
		#region SessionManagement
		public const string TPM_CMD_SaveContext = "TPM_CMD_SaveContext";
		public const string TPM_CMD_LoadContext = "TPM_CMD_LoadContext";
		#endregion
		
		#region StorageFunctions
		public const string TPM_CMD_CreateWrapKey = "TPM_CMD_CreateWrapKey";
		public const string TPM_CMD_LoadKey2 = "TPM_CMD_LoadKey2";
		public const string TPM_CMD_Seal = "TPM_CMD_Seal";
		public const string TPM_CMD_Unseal = "TPM_CMD_Unseal";
		public const string TPM_CMD_GetPubKey = "TPM_CMD_GetPubKey";
		public const string TPM_CMD_Unbind = "TPM_CMD_Unbind";
		
		/// <summary>
		/// This is only a 'dummy' command where the bind prefix is 
		/// retrieved from the server
		/// </summary>
		public const string TPM_CMD_Bind = "TPM_CMD_Bind";
		#endregion
				
		#region Monotonic Counter
		public const string TPM_CMD_CreateCounter = "TPM_CMD_CreateCounter";
		public const string TPM_CMD_ReadCounter = "TPM_CMD_ReadCounter";
		public const string TPM_CMD_IncrementCounter = "TPM_CMD_IncrementCounter";
		public const string TPM_CMD_ReleaseCounter = "TPM_CMD_ReleaseCounter";
		#endregion
		
		#region Cryptographic functions
		public const string TPM_CMD_GetRandom = "TPM_CMD_GetRandom";
        public const string TPM_CMD_Sign = "TPM_CMD_Sign";
		#endregion
	}
}