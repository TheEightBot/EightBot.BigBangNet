using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EightBot.BigBang.Interfaces
{
	public interface ILocalResource
	{
		Task<List<string>> AssetsInFolder (string folderName);

		Task<Stream> GetAsset (string assetPath);
	}
}

