using System;

namespace EightBot.BigBang.Interfaces
{
	public enum ImageExportFormat {
		JPEG,
		PNG
	}

	public interface IImageProcessing
	{
		byte[] ResizeImage (byte[] imageData, float maxWidth, float maxHeight, ImageExportFormat imageExportFormat, float compressionRatio = 0.5f);
	}
}

