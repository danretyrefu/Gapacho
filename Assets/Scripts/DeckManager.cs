using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [Header("База данных ассетов")]
    public List<Sprite> allCardSprites; 

    [Header("Элементы UI Драфта")]
    public GameObject draftPanel;
    public Image leftCardImage;
    public Image rightCardImage;

    [Header("Слоты Игрока (Рука)")]
    public Image[] playerHandSlots; 

    [Header("Слоты Бота (Синие)")]
    public Image[] enemyHandSlots; 

    [HideInInspector] private List<CardData> playerMainDeck = new List<CardData>();
    [HideInInspector] private List<CardData> enemyMainDeck = new List<CardData>();
    [HideInInspector] private List<CardData> discardPile = new List<CardData>();
    [HideInInspector] private List<CardData> currentlyDrawnOptions = new List<CardData>();
    [HideInInspector] private List<CardData> playerHand = new List<CardData>();
    [HideInInspector] private List<CardData> enemyHand = new List<CardData>();

    private bool isDraftingActive = false;

    void Start()
    {
        InitializeDeck(playerMainDeck);
        InitializeDeck(enemyMainDeck);
        
        ShuffleDeck(playerMainDeck);
        ShuffleDeck(enemyMainDeck);
    }

    void InitializeDeck(List<CardData> deckToFill)
    {
        deckToFill.Clear();
        string[] suits = { "c", "d", "h", "s" };
        string[] courtCards = { "J", "Q", "K", "A" };

        for (int v = 2; v <= 10; v++)
        {
            foreach (string s in suits)
            {
                string code = v.ToString() + s;
                FindAndAddCard(code, s, v, deckToFill);
            }
        }

        for (int i = 0; i < courtCards.Length; i++)
        {
            int v = 11 + i;
            foreach (string s in suits)
            {
                string code = courtCards[i] + s;
                FindAndAddCard(code, s, v, deckToFill);
            }
        }

        FindAndAddCard("blackJ", "joker", 0, deckToFill);
        FindAndAddCard("redJ", "joker", 0, deckToFill);
    }

    void FindAndAddCard(string code, string suit, int value, List<CardData> deckToFill)
    {
        string unitySpriteName = code + "_0"; 
        
        Sprite foundSprite = allCardSprites.Find(s => s.name == unitySpriteName);
        if (foundSprite != null)
        {
            deckToFill.Add(new CardData(code, suit, value, foundSprite));
        }
        else
        {
            Debug.LogError($"Error, card with name '{unitySpriteName}' not found.");
        }
    }

    public void ShuffleDeck(List<CardData> deckToShuffle)
    {
        for (int i = deckToShuffle.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            CardData temp = deckToShuffle[i];
            deckToShuffle[i] = deckToShuffle[rnd];
            deckToShuffle[rnd] = temp;
        }
    }

    public void OnClickDeck()
    {
        if (isDraftingActive)
        {
            Debug.LogWarning("player choose one of two cards on the screen");
            return;
        }

        if (playerHand.Count >= 4)
        {
            Debug.LogWarning("player has full hand");
            return;
        }

        currentlyDrawnOptions = DrawTwoCardsFromPlayerDeck();

        if (currentlyDrawnOptions != null && currentlyDrawnOptions.Count == 2)
        {
            isDraftingActive = true;
            draftPanel.SetActive(true);

            leftCardImage.sprite = currentlyDrawnOptions[0].cardSprite;
            rightCardImage.sprite = currentlyDrawnOptions[1].cardSprite;
        }
    }

    private List<CardData> DrawTwoCardsFromPlayerDeck()
    {
        if (playerMainDeck.Count < 2) return null;

        List<CardData> drawn = new List<CardData>();
        drawn.Add(playerMainDeck[0]); playerMainDeck.RemoveAt(0);
        drawn.Add(playerMainDeck[0]); playerMainDeck.RemoveAt(0);
        return drawn;
    }

    public void SelectLeftCard()
    {
        ChooseCard(0, 1);
    }

    public void SelectRightCard()
    {
        ChooseCard(1, 0);
    }

    private void ChooseCard(int chosenIndex, int discardedIndex)
    {
        if (currentlyDrawnOptions == null || currentlyDrawnOptions.Count < 2) return;

        CardData chosenCard = currentlyDrawnOptions[chosenIndex];
        playerHand.Add(chosenCard);

        int freeSlotIndex = playerHand.Count - 1;
        playerHandSlots[freeSlotIndex].sprite = chosenCard.cardSprite;
        
        CardController cc = playerHandSlots[freeSlotIndex].GetComponent<CardController>();
        if (cc != null) cc.SetupCard(chosenCard, true);
        
        playerHandSlots[freeSlotIndex].gameObject.SetActive(true); 

        discardPile.Add(currentlyDrawnOptions[discardedIndex]);

        currentlyDrawnOptions.Clear();
        draftPanel.SetActive(false);
        
        isDraftingActive = false;

        Debug.Log($"card {chosenCard.cardCode} selected: {playerHand.Count}/4");

        if (playerHand.Count == 4)
        {
            DealCardsToEnemy();
        }
    }

    public void DealCardsToEnemy()
    {
        enemyHand.Clear();

        for (int i = 0; i < 4; i++)
        {
            if (enemyMainDeck.Count > 0)
            {
                CardData randomCard = enemyMainDeck[0];
                enemyHand.Add(randomCard);
                enemyMainDeck.RemoveAt(0); 

                enemyHandSlots[i].sprite = randomCard.cardSprite;
                
                CardController cc = enemyHandSlots[i].GetComponent<CardController>();
                if (cc != null) cc.SetupCard(randomCard, false); 

                enemyHandSlots[i].gameObject.SetActive(true);
            }
        }
        Debug.Log("bot has full hand");

        CombatManager cm = FindAnyObjectByType<CombatManager>();
        if (cm != null)
        {
            cm.StartBattlePhase();
        }

    }
}
