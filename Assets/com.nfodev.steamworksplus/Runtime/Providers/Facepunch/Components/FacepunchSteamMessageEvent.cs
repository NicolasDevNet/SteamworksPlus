using Steamworks.Data;
using Steamworks;
using UnityEngine.Events;
using System;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
    /// <summary>
    /// Classe d'évènement contenant les paramètres <see cref="Lobby"/> pour le lobby d'origine, <see cref="Friend"/> pour le contact d'origine et string pour le contenu du message
    /// </summary>
    [Serializable]
	public class FacepunchSteamMessageEvent : UnityEvent<Lobby, Friend, string>
	{
	}
}
