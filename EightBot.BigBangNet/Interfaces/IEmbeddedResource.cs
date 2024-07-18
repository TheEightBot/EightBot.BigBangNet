using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace EightBot.BigBang.Interfaces
{
	public interface IEmbeddedResource
	{
		IEnumerable<String> EmbeddedResourceNames (Assembly assembly);

		IEnumerable<Stream> GetMatchingEmbeddedResourceStreams (Assembly assembly, Func<string, bool> resourceMatch);

		Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName);
	}
}

