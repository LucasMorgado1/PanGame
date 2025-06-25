using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueAnimations : MonoBehaviour
{
    [Header("Dialogue Box Image")]
    [SerializeField] private Image _dialogueBox;
    [SerializeField] private Image _arrowImage;
    private Vector2 _initialArrowPosition;
    private Vector2 _initialDialogueIndicatorPosition;

    private float rate = 0.05f;
    private float amplitude = 3f;

    public Image GetDialogueBoxImage { get => _dialogueBox; }
    public Image GetDialogueArrowImage { get => _arrowImage; }
    public Vector2 GetArrowInitialPosition { get => _initialArrowPosition; }
    public Vector2 GetDialogueIndicatorInitialPosition { get => _initialDialogueIndicatorPosition; }

    private void Awake()
    {
        FadeOutImage(_dialogueBox);
        FadeOutImage(_arrowImage);
        _initialArrowPosition = new Vector2(382.6f, 147.5f);
        _initialDialogueIndicatorPosition = new Vector2(-2f, 186.7f);
    }


    #region UI Animation
    public void FadeInImage(Image image, float duration = 0f)
    {
        image.DOFade(1, duration);
    }

    public void FadeOutImage(Image image, float duration = 0f)
    {
        image.DOFade(0, duration);
    }

    public void Wiggle(Image image, Vector2 position)
    {
        image.transform.localPosition = new Vector2(position.x, position.y + (Mathf.Sin(Time.frameCount * rate) * amplitude));
    }

    #endregion
}
