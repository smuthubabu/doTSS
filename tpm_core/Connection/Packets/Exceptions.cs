
using System;

namespace Iaik.Tc.Tpm.Connection.Packets
{
	public class PacketException : Exception
	{
		public PacketException(string message)
		{
		}
	}
	
	public class NoPacketNumberException : PacketException
	{
		public NoPacketNumberException()
			:base("No free packet number available")
		{
		}
	}
}
