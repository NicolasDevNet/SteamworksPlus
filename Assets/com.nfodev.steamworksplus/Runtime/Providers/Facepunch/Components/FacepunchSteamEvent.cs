using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
	[Serializable]
	public class FacepunchSteamEvent : UnityEvent<Lobby, Friend>
	{
	}
}
