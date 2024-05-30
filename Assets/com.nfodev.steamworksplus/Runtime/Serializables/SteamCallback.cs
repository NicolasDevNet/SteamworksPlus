using SteamworksPlus.Runtime.Providers.Facepunch.Components;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Serializables
{
	[Serializable]
	public class SteamCallback
	{
		[Tooltip("Réponse Unity utilisée quand le callback steam associé est aussi déclenché")]
		public FacepunchSteamEvent Response;

		[Tooltip("Réponse Unity utilisée quand une erreur est levée")]
		public UnityEvent ErrorHandler;
	}
}
