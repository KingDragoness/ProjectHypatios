using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SceneIndexer", order = 1)]

public class SceneIndexer : ScriptableObject
{

    public List<Scene> allScenes = new List<Scene>();

}
