//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Connection.Packets;
using Iaik.Tc.Tpm.Context;
using System.Reflection;

namespace Iaik.Tc.Tpm.Subsystems.Debug
{

	/// <summary>
	/// Implements a Subsystem only for testing 
	/// </summary>
	public class DebugSubsystem : BaseSubsystem<DebugSubsystem.DebugRequestsEnum>
	{
		public enum DebugRequestsEnum : ushort
		{
			/// <summary>
			/// Prints the transmitted text on the server console (log)
			/// 
			/// No Response 
			/// </summary>
			PrintOnServerConsole		= 0x0001
		}
		
		public DebugSubsystem(EndpointContext context)
			:base (context)
		{
			_requestExecutionInfos.Add(
			     DebugRequestsEnum.PrintOnServerConsole, 
			     new RequestExecutionInfo(typeof(RequestPrintOnServerConsole), 
			          new Action<RequestPrintOnServerConsole>(HandlePrintOnServerConsoleRequest))
			     );
		}
		
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_DEBUG; }
		}

		
		
		
		protected override SubsystemRequest CreateRequestFromIdentifier (DebugSubsystem.DebugRequestsEnum requestType)
		{
			if(_requestExecutionInfos.ContainsKey(requestType))
			{
				Type t = _requestExecutionInfos[requestType].RequestType;
				ConstructorInfo ctor = t.GetConstructor(new Type[]{});
				
				if(ctor == null)
					throw new NotSupportedException(string.Format("'{0}' does not have a default ctor!",t));
				
				return (SubsystemRequest)ctor.Invoke(new object[]{});
			}
			else
				throw new NotImplementedException(string.Format("Request type '{0}' not implemented", requestType));
			//switch(requestType)
			//{
			//case DebugRequestsEnum.PrintOnServerConsole:
			//	return new RequestPrintOnServerConsole();
			//default:
			//	throw new NotImplementedException(string.Format("Request type '{0}' not implemented", requestType));
			//}
		}

			
		private void HandlePrintOnServerConsoleRequest(RequestPrintOnServerConsole request)
		{
			Console.WriteLine(request.Text);
		}

	}
}