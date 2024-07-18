using System;
using System.Threading.Tasks;
using System.Reflection;

namespace EightBot.BigBang.Interfaces
{
	public interface IAudioPlayer
	{
		Task<bool> PlayAudioFile (string file);
	}
}

