
using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.Tpm.library.common
{	
	
	public interface ITypedParameter : ITypedStreamSerializable
	{		
	}
	
	[TypedStreamSerializable("p")]
	public class TypedPrimitiveParameter : TypedPrimitive, ITypedParameter
	{
		public TypedPrimitiveParameter (Stream src)
			:base(src)
		{
		}
		
		public TypedPrimitiveParameter (object value)
			:base(value)
		{
		}
	}
}