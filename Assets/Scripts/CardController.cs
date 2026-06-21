using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    [Header("Stats")]
    public string cardCode;
    public string suit;
    public int baseValue;
    public int currentHp;
    public int currentDamage;
    public int shieldsCount = 0;
    public bool isRedTeam;

    [Header("UI Elements")]
    public TextMeshProUGUI statsText; 

    private void Awake()
    {
        UnityEngine.UI.Button btn = GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickCard);
        }
    }

    public void SetupCard(CardData data, bool isRed)
    {
        cardCode = data.cardCode;
        suit = data.suit;
        baseValue = data.value;
        isRedTeam = isRed;

        if (suit == "joker")
        {
            currentHp = 20;
            baseValue = 1;
        }
        else if (baseValue == 14 && suit != "h") 
        {
            currentHp = 1;
        }
        else 
        {
            currentHp = baseValue;
        }

        if (baseValue == 14 && suit == "d")
        {
            shieldsCount = 2;
        }

        CalculateCurrentDamage();
        UpdateVisualStats();
    }

    public void CalculateCurrentDamage()
    {
        if (suit == "joker")
        {
            currentDamage = 1;
            return;
        }

        switch (suit)
        {
            case "s":
            case "c":
                currentDamage = baseValue;
                break;
            case "h":
                currentDamage = currentHp;
                break;
            case "d":
                currentDamage = Mathf.CeilToInt(baseValue / 2f);
                break;
        }
    }

    public void TakeDamage(int amount)
    {
        if (shieldsCount > 0)
        {
            shieldsCount--;
            Debug.Log($"{cardCode} protects. shields: {shieldsCount}");
            UpdateVisualStats(); 
            return;
        }

        int finalDamage = amount;

        if (suit == "d")
        {
            finalDamage = Mathf.CeilToInt(amount / 2f);
        }

        currentHp -= finalDamage;
        if (currentHp < 0) currentHp = 0;

        Debug.Log($"{cardCode} {finalDamage} damage. hp {currentHp}");

        CalculateCurrentDamage();
        UpdateVisualStats(); 

        if (currentHp <= 0)
        {
            Debug.Log($"{cardCode} died");
            
            UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
            if (img != null) img.enabled = false;

            UnityEngine.UI.Button btn = GetComponent<UnityEngine.UI.Button>();
            if (btn != null) btn.interactable = false;

            if (statsText != null)
            {
                statsText.text = "";
                statsText.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateVisualStats()
    {
        if (statsText == null) return;

        if (shieldsCount > 0)
        {
            statsText.text = $"<voffset=10><sprite=1></voffset>{currentDamage}    <voffset=10><sprite=0></voffset>{currentHp}   (SHIELD:{shieldsCount})";
        }
        else
        {
            statsText.text = $"<voffset=10><sprite=1></voffset>{currentDamage}    <voffset=10><sprite=0></voffset>{currentHp}";
        }
    }

    public void OnClickCard()
    {
        CombatManager cm = FindAnyObjectByType<CombatManager>();
        if (cm != null)
        {
            cm.SelectCard(this); 
        }
    }
}
