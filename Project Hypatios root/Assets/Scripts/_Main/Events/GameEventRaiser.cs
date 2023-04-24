using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventRaiser : MonoBehaviour
{
	public GameEvent Event;

	public void RaiseEvent()
	{
		Event.Raise();
	}
}