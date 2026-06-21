using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 5)] public string stepText;
    public bool showArrow = true;
    public Vector2 arrowPosition;
    public float arrowRotationZ = 0f;
}

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements Tutorial")]
    public TextMeshProUGUI uiText;
    public Image uiImageArrow;

    [Header("Tutorial steps")]
    public TutorialStep[] steps; 
    
    private int currentStepIndex = 0;

    void Start()
    {
        currentStepIndex = 0;
        ShowStep(currentStepIndex);
    }

    void Update()
    {
        bool mouseClicked = UnityEngine.InputSystem.Mouse.current != null && 
                            UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
                            
        bool spacePressed = UnityEngine.InputSystem.Keyboard.current != null && 
                            UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame;

        if (mouseClicked || spacePressed)
        {
            NextStep();
        }
    }

    void ShowStep(int index)
    {
        if (index >= steps.Length)
        {
            ExitToMenu();
            return;
        }

        if (uiText != null)
        {
            uiText.text = steps[index].stepText;
        }

        if (uiImageArrow != null)
        {
            if (steps[index].showArrow)
            {
                uiImageArrow.gameObject.SetActive(true);
                uiImageArrow.rectTransform.anchoredPosition = steps[index].arrowPosition;
                uiImageArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, steps[index].arrowRotationZ);
            }
            else
            {
                uiImageArrow.gameObject.SetActive(false);
            }
        }
    }

    void NextStep()
    {
        currentStepIndex++;
        ShowStep(currentStepIndex);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0); 
    }
}
