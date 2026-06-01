using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/RestartAlert")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "RestartAlert", message: "Restart the alert branch", category: "Events", id: "a4541366a5aeb1893b3430692cebc25d")]
public sealed partial class RestartAlert : EventChannel { }

