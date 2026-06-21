using UnityEngine;

[System.Serializable]
public class CardData
{
    public string cardCode;
    public string suit;
    public int value;
    public Sprite cardSprite;

    public CardData(string code, string suit, int value, Sprite sprite)
    {
        this.cardCode = code;
        this.suit = suit;
        this.value = value;
        this.cardSprite = sprite;
    }
}
