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


// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Subsystems;

namespace Iaik.Tc.TPM.Subsystems
{


	public class SubsystemResponseException : Exception
	{
		private SubsystemResponse _response;
		
		public SubsystemResponse Response
		{
			get{ return _response; }
		}
		
		private int _errorCode;
		
		public int ErrorCode
		{
			get{ return _errorCode; }
		}
		
		public SubsystemResponseException (SubsystemResponse response, int errorCode, string message)
			:base(message)
		{
			_response = response;
			_errorCode = errorCode;
		}
		
		
		public override string ToString ()
		{
			return string.Format("{0}-{1} ({2})", ErrorCode, Message, StackTrace);
		}

	}
}
