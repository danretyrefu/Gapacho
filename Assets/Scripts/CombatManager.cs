using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CombatManager : MonoBehaviour
{
    public enum BattleState { RedPlayerTurn, BlueBotTurn, Drafting, GameOver }
    
    [Header("")]
    public BattleState currentState = BattleState.Drafting;

    [Header("Dice")]
    public Image uiDiceImage;
    public Sprite[] diceFaces;

    [Header("Sound")]
    public AudioClip clickClip;
    public AudioClip diceClip;
    public AudioClip hitClip;

    private CardController selectedAttacker = null;
    private AudioSource audioSource;

    void Start()
    {
        currentState = BattleState.Drafting;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartBattlePhase()
    {
        StartCoroutine(RollDiceRoutine());
    }

    private System.Collections.IEnumerator RollDiceRoutine()
    {
        if (uiDiceImage == null || diceFaces.Length < 6)
        {
            DetermineFirstTurn(Random.Range(1, 7));
            yield break;
        }

        uiDiceImage.gameObject.SetActive(true);

        PlaySound(diceClip);

        int finalResult = 1;

        for (int i = 0; i < 10; i++)
        {
            finalResult = Random.Range(1, 7);
            uiDiceImage.sprite = diceFaces[finalResult - 1];
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log($"dice: {finalResult}");
        
        yield return new WaitForSeconds(1.0f);
        uiDiceImage.gameObject.SetActive(false);

        DetermineFirstTurn(finalResult);
    }

    void DetermineFirstTurn(int diceResult)
    {
        if (diceResult % 2 != 0)
        {
            currentState = BattleState.RedPlayerTurn;
            Debug.Log("player turn");
        }
        else
        {
            currentState = BattleState.BlueBotTurn;
            Debug.Log("bot turn");
            Invoke("ExecuteBotTurn", 2.0f); 
        }
    }

    public void SelectCard(CardController clickedCard)
    {
        if (currentState != BattleState.RedPlayerTurn) return;

        PlaySound(clickClip);

        if (clickedCard.isRedTeam && clickedCard.currentHp > 0)
        {
            selectedAttacker = clickedCard;
            Debug.Log($"selected card: {clickedCard.cardCode}");
        }
        else if (selectedAttacker != null && !clickedCard.isRedTeam && clickedCard.currentHp > 0)
        {
            ExecuteAttack(selectedAttacker, clickedCard);
            selectedAttacker = null; 
            
            if (currentState != BattleState.GameOver)
            {
                currentState = BattleState.BlueBotTurn;
                Invoke("ExecuteBotTurn", 2.0f); 
            }
        }
    }

    private void ExecuteAttack(CardController attacker, CardController target)
    {
        Debug.Log($"{attacker.cardCode} hit {target.cardCode}!");

        PlaySound(hitClip);

        int targetHpBeforeHit = target.currentHp;
        attacker.CalculateCurrentDamage();
        int damageToDeal = attacker.currentDamage;

        target.TakeDamage(damageToDeal);

        if (target.suit == "c" && target.currentHp <= 0)
        {
            int overkillDamage = damageToDeal - targetHpBeforeHit;
            if (overkillDamage > 0)
            {
                attacker.TakeDamage(overkillDamage);
            }
        }

        CheckGameOver();
    }

    private void ExecuteBotTurn()
    {
        if (currentState == BattleState.GameOver) return;

        Debug.Log("bot think");

        CardController[] allCards = FindObjectsByType<CardController>(FindObjectsInactive.Exclude);
        
        List<CardController> botCards = new List<CardController>();
        List<CardController> playerCards = new List<CardController>();

        foreach (var card in allCards)
        {
            if (card != null && !string.IsNullOrEmpty(card.cardCode) && card.currentHp > 0)
            {
                if (card.isRedTeam) playerCards.Add(card);
                else botCards.Add(card);
            }
        }

        if (botCards.Count == 0 || playerCards.Count == 0)
        {
            CheckGameOver();
            return;
        }

        CardController bestBotAttacker = botCards[0];
        foreach (var card in botCards)
        {
            card.CalculateCurrentDamage();
            if (card.currentDamage > bestBotAttacker.currentDamage)
            {
                bestBotAttacker = card;
            }
        }

        CardController bestTarget = playerCards[0];
        foreach (var card in playerCards)
        {
            if (card.currentHp < bestTarget.currentHp)
            {
                bestTarget = card;
            }
        }

        Debug.Log($"bot chose a card {bestBotAttacker.cardCode}. attack your {bestTarget.cardCode}!");
        ExecuteAttack(bestBotAttacker, bestTarget);

        if (currentState != BattleState.GameOver)
        {
            currentState = BattleState.RedPlayerTurn;
            Debug.Log("player turn");
        }
    }

    private void CheckGameOver()
    {
        if (currentState == BattleState.Drafting) return;

        CardController[] allCards = FindObjectsByType<CardController>(FindObjectsInactive.Exclude);
        int realRedAlive = 0; int realBlueAlive = 0;
        int playerWinPoints = 0; int botWinPoints = 0;

        foreach (var card in allCards)
        {
            if (card != null && !string.IsNullOrEmpty(card.cardCode) && card.currentHp > 0)
            {
                if (card.isRedTeam) { realRedAlive++; playerWinPoints += card.baseValue; }
                else { realBlueAlive++; botWinPoints += card.baseValue; }
            }
        }

        if (realRedAlive == 0 && realBlueAlive > 0)
        {
            currentState = BattleState.GameOver;
            int losePenalty = Mathf.RoundToInt(botWinPoints * 0.75f);
            SaveSystem.SaveRating(SaveSystem.currentSaveSlot, Mathf.Max(0, SaveSystem.LoadRating(SaveSystem.currentSaveSlot) - losePenalty));
            
            InGameUIManager ui = FindAnyObjectByType<InGameUIManager>();
            if (ui != null) ui.ShowMatchResult(false); 
        }
        else if (realBlueAlive == 0 && realRedAlive > 0)
        {
            currentState = BattleState.GameOver;
            SaveSystem.SaveRating(SaveSystem.currentSaveSlot, SaveSystem.LoadRating(SaveSystem.currentSaveSlot) + playerWinPoints);
            
            InGameUIManager ui = FindAnyObjectByType<InGameUIManager>();
            if (ui != null) ui.ShowMatchResult(true); 
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            float currentSFXVolume = (SoundManager.Instance != null) ? SoundManager.Instance.GetSFXVolume() : 0.5f;
            
            audioSource.PlayOneShot(clip, currentSFXVolume);
        }
    }
}
