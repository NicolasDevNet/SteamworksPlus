using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
    /// <summary>
    /// Classe d'évènement contenant les paramètres <see cref="Lobby"/> pour le lobby d'origine, <see cref="Friend"/> pour le contact d'origine
    /// </summary>
    [Serializable]
	public class FacepunchSteamEvent : UnityEvent<Lobby, Friend>
	{
	}
}
