using UnityEngine;

namespace SteamworksPlus.Runtime.Serializables
{
	[CreateAssetMenu(fileName = "LobbySettings", menuName = "NotTwice/Multi/LobbySettings")]
	public class LobbySettings : ScriptableObject
	{
		public bool IsPublic;

		public bool IsJoinable;

		public bool IsFriendsOnly;

		[Tooltip("Separator used to interpret a steam chat message.")]
		public string ChatDataMessageSeparator;

		public void SetIsJoinable(bool isJoinable)
		{
			IsJoinable = isJoinable;
		}

		public void SetIsPublic(bool isPublic)
		{
			IsPublic = isPublic;
		}

		public void SetIsFriendsOnly(bool isFriendsOnly)
		{
			IsFriendsOnly = isFriendsOnly;
		}
	}
}
