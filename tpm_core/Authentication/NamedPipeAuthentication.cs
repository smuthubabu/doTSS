// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems;
using Iaik.Tc.TPM.Subsystems.Authentication;
using System.Security.Principal;
using System.IO.Pipes;
using Iaik.Tc.TPM.Configuration;

namespace Iaik.Tc.TPM.Authentication
{

	/// <summary>
	/// Defines the implicit authentication mechanism for
	/// NamedPipeConnections.
	/// If the supplied connection is not a <see>NamedPipeConnection</see>
	/// an <see>ArgumentException</see> is thrown.
	/// </summary>
    /// <remarks>
    /// The authentication is done via windows user impersonation, so no extra interaction with the user is neccessary
    /// </remarks>
	[AuthenticationSettings("named_pipe_auth", typeof(NamedPipeConnection))]
	public sealed class NamedPipeAuthentication : AuthenticationMechanism
	{
        private ServerContext ServerContext
        {
            get { return (ServerContext)_context; }
        }

		public NamedPipeAuthentication(IAuthenticationMethod authConfig)
		{
		}
		
        public override void Initialize(EndpointContext context)
        {
            if (typeof(NamedPipeConnection).IsAssignableFrom(context.Connection.GetType()) == false)
                throw new ArgumentException("Supplied connection is not a NamedPipeConnection");    
        
            base.Initialize(context);
        }
		
		public override void Authenticate (RequestContext<AuthenticateRequest, AuthenticateResponse> requestContext)
		{
            string remoteUser = ((NamedPipeServerStream)((NamedPipeConnection)requestContext.Context.Connection).PipeStream).GetImpersonationUserName();

            if (remoteUser == null)
            {
                AuthenticateResponse response = requestContext.CreateResponse();
                response.Succeeded = false;
                response.CustomErrorMessage = "Could not retrieve credentials from requesting client!";
                response.Execute();
            }
            else
            {
                ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember =
                    new ExternalUser(remoteUser, null);


                AuthenticateResponse response = requestContext.CreateResponse();
                response.Succeeded = true;
                response.Execute();
            }
		}
		
	}
}
