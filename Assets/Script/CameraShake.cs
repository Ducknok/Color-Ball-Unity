using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private void Awake()
    {
        // Singleton để gọi từ bất cứ đâu
        if (Instance == null) Instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        //Debug.Log("rung ne");
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Tạo một độ dời ngẫu nhiên
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            // Đợi khung hình tiếp theo
            yield return null;
        }

        // Trả Camera về vị trí gốc sau khi rung xong
        transform.localPosition = originalPos;
    }
}