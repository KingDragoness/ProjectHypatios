using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chamber_Tutorial : MonoBehaviour
{



    public List<GameObject> TutorialSteps;

    public int currentStage;
    private float cooldownTip = 10f;

    private void Start()
    {
        int index = 0;

        foreach(var step in TutorialSteps)
        {
            if (index != 0)
                ActivateChamber(index, false);
            
            index++;

        }
        ActivateChamber(0, true);

    }

    public void UpdateStage(int stage)
    {
        currentStage = stage;
    }

    private void Update()
    {
        {
            int currentIndex = currentStage;
            List<int> tutorialDeactivates = new List<int>();

            for (int x = 0; x < TutorialSteps.Count; x++)
            {
                if (currentIndex == x)
                {
                    continue;
                }

                tutorialDeactivates.Add(x);
            }

            ActivateChamber(currentIndex, true);
            ActivateChamber(tutorialDeactivates.ToArray(), false);
        }
    }

    public void ActivateChamber(int step, bool enable = true)
    {
        if (TutorialSteps[step].activeSelf != enable)
        {
            TutorialSteps[step].SetActive(enable);
        }
    }

    public void ActivateChamber(int[] step, bool enable = true)
    {
        foreach (var s in step)
        {
            if (TutorialSteps[s].activeSelf != enable)
            {
                TutorialSteps[s].SetActive(enable);
            }
        }
    }

}
