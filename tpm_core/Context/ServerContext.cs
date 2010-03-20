//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>


using System;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;
using Iaik.Tc.Tpm.Subsystems.Debug;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Subsystems.Authentication;
using Iaik.Tc.Tpm.Authentication;
using Iaik.Tc.Tpm.Subsystems.Tpm;
using Iaik.Tc.Tpm.library;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.Context
{

	/// <summary>
	/// Adds server specific subsystems to the EndpointContext
	/// </summary>
	public class ServerContext : EndpointContext
	{

		protected ServerAuthenticationContext _serverAuthenticationContext = null;
		
		/// <summary>
		///Saves the authentication state of the connection client 
		/// </summary>				
		public ServerAuthenticationContext ServerAuthenticationContext
		{
			get { return _serverAuthenticationContext; }
			set { _serverAuthenticationContext = value; }
		}
		
		protected AccessControlList _accessControlList;
		
		/// <summary>
		///Provides access to users, groups and permissions
		/// </summary>
		public AccessControlList AccessControlList
		{
			get { return _accessControlList; }
		}
		
		protected IDictionary<string, TpmContext> _tpmContexts;
		
		/// <summary>
		///Provides access to all defined tpm devices 
		/// </summary>
		public IDictionary<string, TpmContext> TpmContexts
		{
			get { return _tpmContexts;}
		}
		
		public ServerContext (FrontEndConnection connection, PacketTransmitter packetTransmitter, IConnectionsConfiguration connectionConfig,
			AccessControlList acl, IDictionary<string, TpmContext> tpmContexts)
			: base(connection, packetTransmitter)
		{
			_accessControlList = acl;
			_tpmContexts = tpmContexts;

			RegisterSubsystem(new DebugSubsystem(this, connectionConfig));
            RegisterSubsystem(new AuthenticationSubsystem(this, connectionConfig));
			RegisterSubsystem(new TpmSubsystem(this, connectionConfig));
			_configured = true;
			_configuredEvent.Set();
		}
	}
	
	/// <summary>
	///Saves the authentication state of the connected clients 
	/// </summary>
	public class ServerAuthenticationContext
	{
		
		protected AuthenticationMechanism _authenticationMechanism = null;
		
		/// <summary>
		///Gets or sets the authentication mechanism used by the connected client 
		/// </summary>
		public AuthenticationMechanism AuthenticationMechanism
		{
			get{ return _authenticationMechanism; }
			set{ _authenticationMechanism = value;}
		}
		
		protected IPermissionMember _authenticatedPermissionMember = null;
		
		/// <summary>
		///Sets or Gets the authenticated permission member. This is used to check permissions. 
		/// </summary>
		public IPermissionMember AuthenticatedPermissionMember
		{
			get{ return _authenticatedPermissionMember;}
			set{ _authenticatedPermissionMember = value;}
		}
	}
}
