using Steamworks.Data;
using System;
using UnityEngine;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Extentions
{
	/// <summary>
	/// Extension class dedicated to adding behaviors for the Lobby structure provided by Facepunch
	/// </summary>
	public static class FacepunchLobbyExtentions
	{
		/// <summary>
		/// Method for retrieving an Ulong value from data in a lobby structure.
		/// Typically used to retrieve a SteamId
		/// </summary>
		/// <param name="lobby">Target lobby</param>
		/// <param name="key">The key to be used to retrieve the data</param>
		/// <param name="result">The result returned a default value</param>
		/// <returns>The success of the operation</returns>
		public static bool GetUlongValueFromLobbyData(this Lobby lobby, string key, out ulong result)
		{
			return ulong.TryParse(lobby.GetData(key), out result);
		}

		/// <summary>
		/// Method for retrieving a generic value from the data of a lobby structure.
		/// The data is parsed from JSON content, so the T class must be serializable.
		/// </summary>
		/// <typeparam name="T">The type used for serialization</typeparam>
		/// <param name="lobby">Target lobby</param>
		/// <param name="key">The key to be used to retrieve the data</param>
		/// <param name="result">The result returned or a null value</param>
		/// <returns>The success of the operation</returns>
		public static bool GetTValueFromLobbyData<T>(this Lobby lobby, string key, out T? result)
			where T : class
		{
			try
			{
				result = JsonUtility.FromJson(lobby.GetData(key), typeof(T)) as T;
				return true;
			}
			catch (Exception)
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Method for saving a generic value to the data of a lobby structure.
		/// The data is parsed to JSON content, so the T class must be serializable.
		/// </summary>
		/// <typeparam name="T">The type used for serialization</typeparam>
		/// <param name="lobby">Target lobby</param>
		/// <param name="key">The key to be used to save the data</param>
		/// <param name="input">The data structure to be serialized</param>
		/// <returns>The success of the operation</returns>
		public static bool SetTValueToLobbyData<T>(this Lobby lobby, string key, T input)
			where T : class
		{
			try
			{
				return lobby.SetData(key, JsonUtility.ToJson(input));
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
