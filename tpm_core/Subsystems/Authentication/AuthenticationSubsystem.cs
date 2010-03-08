// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Authentication;
using System.Collections.Generic;
using Iaik.Utils.CommonFactories;

namespace Iaik.Tc.Tpm.Subsystems.Authentication
{

	/// <summary>
	/// Implements the subsystem the client uses to authenticate to the server
	/// </summary>
	public class AuthenticationSubsystem : BaseSubsystem<AuthenticationSubsystem.AuthenticationRequests>
	{
        /// <summary>
        /// Contains the selected and initialized authentication mechanism
        /// </summary>
        private AuthenticationMechanism _selectedAuthenticationMechanism = null;

		public enum AuthenticationRequests : ushort
		{
			/// <summary>
			/// Sends all available authentication mechanisms to the requesting endpoint
			/// </summary>
			ListAuthenticationMechanisms	= 0x0001,

            /// <summary>
            /// Selects the chosen AuthenticationMechanism and responds with success or failure 
            /// if the specified mechanism cannot be selected (not compatible,...)
            /// </summary>
            SelectAuthenticationMechanism,
			
			/// <summary>
			/// Starts the authentication process using the selected authentication mechanism
			/// This request returns once the authentication has completed (successfully or not)
			/// During this request is active other, authentication mechanism-dependent will be sent.
			/// </summary>
			Authenticate,
			
			/// <summary>
			/// Requests informations about the current status of the authentication process from the server
			/// </summary>
			AuthenticationInfo
		}
		
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}
		
        public AuthenticationSubsystem(EndpointContext ctx, IConnectionsConfiguration config)
			:base(ctx, config)
		{
			_requestExecutionInfos.Add(AuthenticationRequests.ListAuthenticationMechanisms,
                  BuildRequestExecutionInfo<AuthenticationSubsystem, ListAuthenticationMechanismsRequest, ListAuthenticationMechanismsResponse>
                                       (HandleListAuthenticationMechanisms));

            _requestExecutionInfos.Add(AuthenticationRequests.SelectAuthenticationMechanism,
                  BuildRequestExecutionInfo<AuthenticationSubsystem, SelectAuthenticationMechanismsRequest, SelectAuthenticationMechanismsResponse>
                                       (HandleSelectAuthenticationMechanismsRequest));
        }


        /// <summary>
        /// Returns the available and compatible authentication mechanisms
        /// </summary>
        /// <returns></returns>
        private List<string> ListCompatibleAuthenticationMechanisms()
        {
            List<string> compatibleAuthenticationMethods = new List<string>();

            foreach (IAuthenticationMethod authMethod in ConnectionsConfig.AuthenticationMethods)
            {
                AuthenticationMechanismChecker checker = authMethod.AuthChecker;

                if (checker.IsCompatibleWith(EndpointContext.Connection))
                    compatibleAuthenticationMethods.Add(authMethod.AuthIdentifier);
            }

            return compatibleAuthenticationMethods;
        }

        #region Request handlers
        /// <summary>
        /// Requesthandler
        /// 
        /// Looks for all configured and compatible authentication methods for the requesting client
        /// </summary>
        /// <param name="subsystem"></param>
        /// <param name="requestCtx"></param>
        private void HandleListAuthenticationMechanisms(AuthenticationSubsystem subsystem,
                RequestContext<ListAuthenticationMechanismsRequest, ListAuthenticationMechanismsResponse> requestCtx)
        {      
            ListAuthenticationMechanismsResponse response = requestCtx.CreateResponse();
            response.AuthenticationModes = ListCompatibleAuthenticationMechanisms().ToArray();
            response.Execute();
        }

        /// <summary>
        /// Requesthandler
        /// 
        /// Trys to select the client desired authentication mechanism and responds with the selection status
        /// </summary>
        /// <param name="subsystem"></param>
        /// <param name="requestCtx"></param>
        private void HandleSelectAuthenticationMechanismsRequest(AuthenticationSubsystem subsystem,
                RequestContext<SelectAuthenticationMechanismsRequest, SelectAuthenticationMechanismsResponse> requestCtx)
        {
            List<string> compatibleAuthenticationMechanisms = ListCompatibleAuthenticationMechanisms();

            try
            {
                if (compatibleAuthenticationMechanisms.Contains(requestCtx.Request.AuthMechanismToSelect))
                {
                    _selectedAuthenticationMechanism = GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<AuthenticationMechanism>(requestCtx.Request.AuthMechanismToSelect);
                    _selectedAuthenticationMechanism.Initialize(_context);
                    SelectAuthenticationMechanismsResponse response = requestCtx.CreateResponse();
                    response.Succeeded = true;
                    response.Execute();
                }
                else
                {
                    SelectAuthenticationMechanismsResponse response = requestCtx.CreateResponse();
                    response.Succeeded = false;
                    response.SetKnownErrorCode(AuthenticationSubsystemResponseBase.ErrorCodeEnum.AuthenticationMechanismNotAvailable);
                    response.Execute();
                }
            }
            catch (Exception ex)
            {
                SelectAuthenticationMechanismsResponse response = requestCtx.CreateResponse();
                response.Succeeded = false;
                response.CustomErrorMessage = string.Format("An unexpected error occured: {0}", ex.Message);
                response.Execute();                
            }
        }

        #endregion

    }
}
