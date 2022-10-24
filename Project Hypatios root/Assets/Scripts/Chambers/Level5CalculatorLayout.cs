using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Level5CalculatorLayout : MonoBehaviour
{


    public ChamberText inputText; //display input
    public Level5Chamber chamberScript;
    public Level5CalculatorButton button;
    public int answer = 0;
    public Vector3 startPos;
    public float distX = 4;
    public float distY = -4;

    private List<string> functions = new List<string>();


    private void Start()
    {

        functions.Add("1");
        functions.Add("2");
        functions.Add("3");
        functions.Add("4");
        functions.Add("5");
        functions.Add("6");
        functions.Add("7");
        functions.Add("8");
        functions.Add("9");
        functions.Add("0");
        functions.Add("CLR");
        functions.Add("OK");

        int i = 0;

        //layout

        for (int y = 0; y < 4; y++)
        {
            float posY = startPos.y + (distY * y);

            for (int x = 0; x < 3; x++)
            {
                float posX = startPos.x + (distX * x);

                var buttonNew = Instantiate(button, transform);
                buttonNew.gameObject.SetActive(true);
                buttonNew.transform.localPosition = new Vector3(posX, posY, buttonNew.transform.localPosition.z);
                buttonNew.textMesh.text = functions[i];
                buttonNew.operationName = functions[i];
                i++;
            }
        }

    }

    private void Update()
    {
 

        inputText.SetTextContent(answer.ToString());
    }

    public void InputOperation(string functionName)
    {
        int val = 0;

 

        if (functionName == "CLR")
        {
            answer = 0;
        }
        else if (functionName == "OK")
        {
            if (chamberScript.HasStarted == false)
            {
                DialogueSubtitleUI.instance.QueueDialogue("Press shoot the start button to commence the game.", "ANNOUNCER", 3f);
                return;
            }

            chamberScript.AnswerCheck(answer);
            answer = 0;
        }
        else if (int.TryParse(functionName, out val) == true)
        {
            if (answer > 9999)
            {
                return;
            }

            string result = answer.ToString() + val.ToString(); // or, "1" + "1"
            int.TryParse(result, out val);

            answer = val;
        }

    }

}
