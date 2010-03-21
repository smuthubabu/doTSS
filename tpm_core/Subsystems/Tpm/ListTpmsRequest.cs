// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Subsystems.Tpm
{


	public class ListTpmsRequest : TypedSubsystemRequest<ListTpmsResponse>
	{
		public override ushort RequestIdentifier {
			get {
				return (ushort)TpmSubsystem.TpmRequestEnum.ListTpmDevices;
			}
		}
		
		public override string Subsystem {
			get {
				return SubsystemConstants.SUBSYSTEM_TPM;
			}
		}
		

		public ListTpmsRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
	}
	
	
	public class ListTpmsResponse  : TpmSubsystemResponseBase
	{
		private string[] _tpmDevices;
		
		/// <summary>
		/// Gets or sets the tpm devices that can be used/selected by the authenticated
		/// user
		/// </summary>
		public string[] TpmDevices
		{
			get { return _tpmDevices; }
			set { _tpmDevices = value; }
		}
		
		
		public ListTpmsResponse (SubsystemRequest request, EndpointContext ctx)
			: base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
		
			_tpmDevices = new string[StreamHelper.ReadInt32 (src)];
			for (int i = 0; i < _tpmDevices.Length; i++)
				_tpmDevices[i] = StreamHelper.ReadString (src);
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			if (_tpmDevices == null)
				StreamHelper.WriteInt32 (0, sink);
			else
			{
				StreamHelper.WriteInt32 (_tpmDevices.Length, sink);
				foreach (string tpmDevice in _tpmDevices)
					StreamHelper.WriteString (tpmDevice, sink);
			}
		
		}


	}
}
