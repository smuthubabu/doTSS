
using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{

	[TypedStreamSerializable(TPMPubkey.PARAM_TPM_PUBKEY)]
	public class TPMPubkeyCore : TPMPubkey, ITPMBlobReadable
	{
		public TPMPubkeyCore (TPMBlob src)
		{
			ReadFromTpmBlob (src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_keyParams = new TPMKeyParamsCore (blob);
			
			uint keyLength = blob.ReadUInt32 ();			
			_publicKey = new byte[keyLength];
			blob.Read (_publicKey, 0, _publicKey.Length);
			
		}
		
		#endregion
	

	}
}