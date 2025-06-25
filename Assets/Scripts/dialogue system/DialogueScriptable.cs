using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Scriptable", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class DialogueScriptable : ScriptableObject
{
    public bool _enteredOnce = false;
    public bool _isOnDialogue = false;
    public bool _mousePressed = false;
    public bool _mousePressedOnce = false;
}
