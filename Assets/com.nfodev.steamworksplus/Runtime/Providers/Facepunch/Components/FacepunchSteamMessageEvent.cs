using Steamworks.Data;
using Steamworks;
using UnityEngine.Events;
using System;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
	[Serializable]
	public class FacepunchSteamMessageEvent : UnityEvent<Lobby, Friend, string>
	{
	}
}
