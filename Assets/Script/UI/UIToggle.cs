using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class UIToggle : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform handle;
    public Image backgroundImage;

    [Header("Colors")]
    public Color onColor = new Color(0f, 0.8f, 1f); // M¨¤u xanh Cyan (B?t)
    public Color offColor = new Color(0.3f, 0.3f, 0.3f); // M¨¤u x¨¢m (T?t)

    [Header("Animation Settings")]
    public float onPositionX = 20f;
    public float offPositionX = -20f;
    public float slideSpeed = 15f; // T?c ?? tr??t

    private Toggle toggle;
    private Coroutine animateCoroutine;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        // L?ng nghe s? ki?n m?i khi ng??i ch?i b?m n¨²t
        toggle.onValueChanged.AddListener(OnSwitchTriggered);

        // Kh?i t?o tr?ng th¨¢i ban ??u m¨¤ kh?ng c?n ch?y animation
        if (toggle.isOn)
        {
            handle.anchoredPosition = new Vector2(onPositionX, 0);
            if (backgroundImage != null) backgroundImage.color = onColor;
        }
        else
        {
            handle.anchoredPosition = new Vector2(offPositionX, 0);
            if (backgroundImage != null) backgroundImage.color = offColor;
        }
    }

    private void OnSwitchTriggered(bool isOn)
    {
        if (animateCoroutine != null) StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateSwitch(isOn));
        AudioManager.Instance.PlayClick();
    }

    private IEnumerator AnimateSwitch(bool isOn)
    {
        Vector2 targetPos = new Vector2(isOn ? onPositionX : offPositionX, 0);
        Color targetColor = isOn ? onColor : offColor;

        while (Vector2.Distance(handle.anchoredPosition, targetPos) > 0.1f)
        {
            handle.anchoredPosition = Vector2.Lerp(handle.anchoredPosition, targetPos, Time.unscaledDeltaTime * slideSpeed);

            if (backgroundImage != null)
            {
                backgroundImage.color = Color.Lerp(backgroundImage.color, targetColor, Time.unscaledDeltaTime * slideSpeed);
            }

            yield return null;
        }

        handle.anchoredPosition = targetPos;
        if (backgroundImage != null) backgroundImage.color = targetColor;
    }

    private void OnDestroy()
    {
        if (toggle != null) toggle.onValueChanged.RemoveListener(OnSwitchTriggered);
    }
}