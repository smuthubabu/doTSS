// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.Tpm.Context;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.Subsystems.Authentication
{
	/// <summary>
	/// Requests all available authentication modes for the current connection
	/// from the server
	/// </summary>
	public class ListAuthenticationMechanismsRequest : TypedSubsystemRequest<ListAuthenticationMechanismsResponse>
	{
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}

		public override ushort RequestIdentifier
		{
			get
			{
				return (ushort)AuthenticationSubsystem.AuthenticationRequests.
					ListAuthenticationMechanisms;
			}
		}
					

		public ListAuthenticationMechanismsRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
	}
	
	
	/// <summary>
	/// Response to a <see>ListAuthenticationModeRequest</see>,
	/// sends all the available authentication mode identifiers for the current connection
	/// back to the requesting client
	/// </summary>
	public class ListAuthenticationMechanismsResponse : SubsystemResponse
	{
		private string[] _authenticationModes;
		
		/// <summary>
		/// Gets the valid authentication modes for the current connection
		/// </summary>
		public string[] AuthenticationModes
		{
			get{ return _authenticationModes; }
		}
		
		public ListAuthenticationMechanismsResponse(SubsystemRequest request, EndpointContext ctx, 
		                                       string[] authenticationModes)
			:base(request, ctx)
		{
			_authenticationModes = authenticationModes;
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			int count = StreamHelper.ReadInt32(src);
			_authenticationModes = new string[count];
			
			for(int i = 0; i<count; i++)
				_authenticationModes[i] = StreamHelper.ReadString(src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			if(_authenticationModes == null)
				StreamHelper.WriteInt32(0, sink);
			else
			{
				StreamHelper.WriteInt32(_authenticationModes.Length, sink);
			
				foreach(string authenticationMode in _authenticationModes)
					StreamHelper.WriteString(authenticationMode, sink);
			}
			
		}

		
	}
}
