using UnityEngine;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Scriptable configuration class for the Steam lobby
    /// </summary>
    [CreateAssetMenu(fileName = "LobbySettings", menuName = "SteamworksPlus/LobbySettings")]
	public class LobbySettings : ScriptableObject
	{
        /// <summary>
        /// Indicates whether the lobby is public or not
        /// </summary>
        [Tooltip("Indicates whether the lobby is public or not")]
        public bool IsPublic;

        /// <summary>
        /// Indicates whether the lobby is reachable or not
        /// </summary>
        [Tooltip("Indicates whether the lobby is reachable or not")]
        public bool IsJoinable;

        /// <summary>
        /// Indicates whether the lobby is for friends only or not
        /// </summary>
        [Tooltip("Indicates whether the lobby is for friends only or not")]
        public bool IsFriendsOnly;

        /// <summary>
        /// Separator used to interpret a steam chat message.
        /// </summary>
        [Tooltip("Separator used to interpret a steam chat message.")]
		public string ChatDataMessageSeparator;

        /// <summary>
        /// Unity Friendly method for defining whether the lobby is reachable or not
        /// </summary>
        public void SetIsJoinable(bool isJoinable)
		{
			IsJoinable = isJoinable;
		}

        /// <summary>
        /// Unity Friendly method for defining whether the lobby is public or not
        /// </summary>
        public void SetIsPublic(bool isPublic)
		{
			IsPublic = isPublic;
		}

        /// <summary>
        /// Unity Friendly method for defining whether the lobby is for friends only or not
        /// </summary>
        public void SetIsFriendsOnly(bool isFriendsOnly)
		{
			IsFriendsOnly = isFriendsOnly;
		}
	}
}
