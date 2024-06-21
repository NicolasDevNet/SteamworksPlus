using Steamworks.Data;
using UnityEngine;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Extentions
{
	/// <summary>
	/// Extension class dedicated to adding behaviors for the Image structure provided by Facepunch
	/// </summary>
	public static class SPImageExtentions
	{
		/// <summary>
		/// Method for converting information from a facepunch image structure to a Texture2D class
		/// </summary>
		public static Texture2D ConvertToTexture2D(this Image? image)
		{
			if (image == null)
			{
				return null;
			}

			// Create a new Texture2D
			var avatar = new Texture2D((int)image.Value.Width, (int)image.Value.Height, TextureFormat.ARGB32, false);

			// Set filter type, or else its really blury
			avatar.filterMode = FilterMode.Trilinear;

			// Flip image
			for (int x = 0; x < image.Value.Width; x++)
			{
				for (int y = 0; y < image.Value.Height; y++)
				{
					var p = image.Value.GetPixel(x, y);
					avatar.SetPixel(x, (int)image.Value.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
				}
			}

			avatar.Apply();
			return avatar;
		}
	}
}
