using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NewRecord : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private Vector3 originalScale;

    [Header("Hiệu ứng nhịp đập (Pulse)")]
    public float pulseSpeed = 8f;      // Tốc độ đập
    public float pulseAmount = 0.15f;  // Độ to/nhỏ khi đập

    [Header("Hiệu ứng màu (Rainbow)")]
    public bool enableRainbow = true;
    public float colorSpeed = 1.5f;    // Tốc độ đổi màu

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        // Lưu lại kích thước gốc bạn đã chỉnh trong Inspector
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Khi chữ vừa được bật lên (phá kỷ lục), thu nhỏ nó về 0 
        // để tạo hiệu ứng Pop-up nảy ra màn hình
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        // 1. HIỆU ỨNG POP-UP & NHỊP TIM
        // Dùng unscaledTime để hoạt động được cả khi Time.timeScale = 0
        float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
        Vector3 targetScale = originalScale + new Vector3(pulse, pulse, 0);

        // Lerp giúp chữ nảy từ 0 lên kích thước thật một cách mượt mà, sau đó đập nhịp tim
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * 10f);

        // 2. HIỆU ỨNG ĐỔI MÀU (RAINBOW)
        if (enableRainbow && textMesh != null)
        {
            // PingPong tạo ra giá trị chạy từ 0 -> 1 -> 0 liên tục
            float h = Mathf.PingPong(Time.unscaledTime * colorSpeed, 1f);

            // Chuyển hệ màu HSV sang RGB để ra màu cầu vồng (Sáng và Rực rỡ)
            textMesh.color = Color.HSVToRGB(h, 0.7f, 1f);
        }
    }
}