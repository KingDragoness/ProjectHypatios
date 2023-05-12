using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DebugFreeScriptModify : MonoBehaviour
{
    void Start()
    {
        MyTaskAsync();
        Debug.Log("Test start");
    }

    async void MyTaskAsync()
    {
        // This task will finish, even though it's object is destroyed
        Debug.Log("Async Task Started");
        await Task.Delay(5000);
        Debug.Log("Async Task Ended");
    }

}
