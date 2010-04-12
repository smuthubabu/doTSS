// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>


using System;

namespace Iaik.Tc.TPM.Library.Common
{
	public static class TPMCommandNames
	{
		#region Admin Ownership
		public const string TPM_CMD_TakeOwnership = "TPM_CMD_TakeOwnership";
		#endregion
		
		#region Integrity Collection and Reporting
		public const String TPM_CMD_PCRRead = "TPM_CMD_PCRRead";
		#endregion
		
		#region Capability Commands
		public const String TPM_CMD_GetCapability = "TPM_CMD_GetCapability";
		#endregion
		
		#region Endorsement key handling
		public const string TPM_CMD_ReadPubek = "TPM_CMD_ReadPubek";
		#endregion
		
		#region Authorization sessions
		public const string TPM_CMD_OIAP = "TPM_CMD_OIAP";
		#endregion
		
		#region Eviction
		public const string TPM_CMD_FlushSpecific = "TPM_CMD_FlushSpecific";
		#endregion
		
		#region SessionManagement
		public const string TPM_CMD_SaveContext = "TPM_CMD_SaveContext";
		public const string TPM_CMD_LoadContext = "TPM_CMD_LoadContext";
		#endregion
		
		// TODO: tbd. DELETE!!!
#region balbla
		public const String r ="t";
#endregion	
		
	}
}