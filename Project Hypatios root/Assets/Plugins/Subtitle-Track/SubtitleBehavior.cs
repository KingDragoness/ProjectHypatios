using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleBehavior : PlayableBehaviour
{
    [TextArea(3, 5)]
    public string subtitleText;

}
