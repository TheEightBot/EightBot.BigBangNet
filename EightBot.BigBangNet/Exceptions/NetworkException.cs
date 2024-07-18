﻿using System;

namespace EightBot.BigBang.Exceptions
{
	public class NetworkException : Exception
	{
		public NetworkException () :base() { }

		public NetworkException (string message) :base(message) { }

		public NetworkException (string message, Exception innerException) :base(message, innerException) { }
	}
}

