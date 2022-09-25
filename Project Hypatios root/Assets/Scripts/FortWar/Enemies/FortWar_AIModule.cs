using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class FortWar_AIModule : MonoBehaviour
{

    public Enemy_FW_Bot BotScript;

    public abstract void Run();

}
