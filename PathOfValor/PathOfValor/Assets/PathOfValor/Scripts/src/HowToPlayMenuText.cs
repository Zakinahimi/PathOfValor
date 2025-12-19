using TMPro;
using UnityEngine;

[ExecuteAlways]
public class HowToPlayMenuText : MonoBehaviour
{
    private const string TutorialText =
@"<b>How to Play</b>
Movement: WASD/Arrow Keys or Left Stick
Combat: Left Mouse / Right Trigger to attack
Combat (Level 4): Right Mouse to cast fireball
Interact: Press E to talk/advance dialogue
Menu: Arrow Keys/WASD or Left Stick to navigate
Menu: Enter/Space or Gamepad Button 0 to select
Menu: Esc or Gamepad Button 1 to back";

    private void OnEnable()
    {
        ApplyText();
    }

    private void OnValidate()
    {
        ApplyText();
    }

    private void ApplyText()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        if (text == null)
        {
            return;
        }

        if (text.text != TutorialText)
        {
            text.SetText(TutorialText);
        }
    }
}
