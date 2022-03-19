using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level2MinigameCasino : MonoBehaviour
{

    public float casinoTimerLimit = 90;
    public int score = 0;

    [Space]
    public UnityEvent OnGameStarted;
    public UnityEvent OnGameEnded;

    [Space]
    public TextMesh text_roundTimer;
    public TextMesh text_score;
    public Lv2MinigameCasino_Shootable ShootableCivilian;
    public Lv2MinigameCasino_Shootable ShootableTerrorist;
    public RandomSpawnArea randomSpawnArea;

    private float f_timerCasino = 0;
    private float f_timerSpawnShootable = 0;
    private bool hasStartedGame = false;


    public void StartGame()
    {
        hasStartedGame = true;
        DialogueSubtitleUI.instance.QueueDialogue("Shoot Em' Up has started. Shoot those terrorists and avoid shooting civilians.", "SYSTEM", 10f);
    }

    private void Update()
    {
        if (hasStartedGame == true)
        {
            f_timerCasino += Time.deltaTime;
            f_timerSpawnShootable += Time.deltaTime;
            text_score.text = score.ToString();
            text_roundTimer.text = $"[{Mathf.FloorToInt(f_timerCasino)}/{casinoTimerLimit}]s".ToString();
            OnGameStarted?.Invoke();

            if (f_timerCasino < casinoTimerLimit - 1)
            {
                if (f_timerSpawnShootable > 3)
                {
                    TrySpawnShootable();
                }
            }

            if (f_timerCasino >= casinoTimerLimit)
            {
                GameEnded();
            }

        }
    }

    private void TrySpawnShootable()
    {
        float chance = Random.Range(0, 1f);

        if (chance > 0.1f)
        {
            var terrorist1 = Instantiate(ShootableTerrorist, randomSpawnArea.GetAnyPositionInsideBox(), ShootableTerrorist.transform.rotation);
            terrorist1.gameObject.SetActive(true);
        }
        else
        {
            var civilian1 = Instantiate(ShootableCivilian, randomSpawnArea.GetAnyPositionInsideBox(), ShootableCivilian.transform.rotation);
            civilian1.gameObject.SetActive(true);
        }

        f_timerSpawnShootable = 0;
    }

    public void GameEnded()
    {
        OnGameEnded?.Invoke();

        int collectedSoul = Mathf.CeilToInt(score);
        int count = collectedSoul;

        for(int i = 0; i < count; i++)
        {
            collectedSoul += PlayerPerk.GetBonusSouls();
        }

        DialogueSubtitleUI.instance.QueueDialogue($"Shoot Em' Up has ended! You have been rewarded: {collectedSoul} souls", "SYSTEM", 10f);
        FPSMainScript.instance.SoulPoint += collectedSoul;
        hasStartedGame = false;
        score = 0;
        f_timerSpawnShootable = 0;
        f_timerCasino = 0;
    }

    public void RewardPlay(int score1)
    {
        score += score1;

        if (score <= 0)
        {
            score = 0;
        }
    }

    public void PenalizedPlay(int score1)
    {
        score -= score1;

        if (score <= 0)
        {
            score = 0;
        }
    }

}
