using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class SwipeMenu : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect scrollRect;

    [Header("UI References")]
    public RectTransform contentPanel;

    [Header("Snapping Settings")]
    public float snapSpeed = 15f;
    public float scaleActive = 1f;
    public float scaleInactive = 0.7f;

    [Header("Fix Lòi Bóng (Tàng hình ở rìa)")]
    [Tooltip("Khoảng cách tối đa từ tâm Viewport ra 2 bên để bóng hiển thị (Tính bằng Pixel). VD: 400 hoặc 500")]
    public float hideDistance = 450f;

    private float[] itemPositions;
    private bool isDragging;
    private int currentItemIndex;
    private RectTransform[] items;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (contentPanel == null) contentPanel = scrollRect.content;

        StartCoroutine(WaitAndSetup());
    }

    private IEnumerator WaitAndSetup()
    {
        yield return new WaitForEndOfFrame();
        SetupItems();
    }

    public void SetupItems()
    {
        int itemCount = contentPanel.childCount;
        items = new RectTransform[itemCount];
        itemPositions = new float[itemCount];

        if (itemCount > 1)
        {
            float distance = 1f / (itemCount - 1f);
            for (int i = 0; i < itemCount; i++)
            {
                items[i] = contentPanel.GetChild(i).GetComponent<RectTransform>();
                itemPositions[i] = distance * i;
            }
        }
        else if (itemCount == 1)
        {
            items[0] = contentPanel.GetChild(0).GetComponent<RectTransform>();
            itemPositions[0] = 0.5f;
        }
    }

    private void Update()
    {
        if (items == null || items.Length == 0) return;

        if (isDragging)
        {
            float minDistance = float.MaxValue;
            for (int i = 0; i < itemPositions.Length; i++)
            {
                float dist = Mathf.Abs(scrollRect.horizontalNormalizedPosition - itemPositions[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    currentItemIndex = i;
                }
            }
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                itemPositions[currentItemIndex],
                snapSpeed * Time.unscaledDeltaTime
            );
        }

        for (int i = 0; i < items.Length; i++)
        {
            // Phóng to thu nhỏ thẻ
            float targetScale = (i == currentItemIndex) ? scaleActive : scaleInactive;
            items[i].localScale = Vector3.Lerp(
                items[i].localScale,
                new Vector3(targetScale, targetScale, 1f),
                snapSpeed * Time.unscaledDeltaTime
            );

            // --- CÔNG THỨC MỚI NHẤT: BẤT CHẤP LỖI LỆCH PIVOT ---
            UISkinCard card = items[i].GetComponent<UISkinCard>();
            if (card != null)
            {
                // 1. Lấy tâm vật lý thực sự của thẻ rồi chuyển ra tọa độ thế giới (World Space)
                Vector3 cardCenterWorld = items[i].TransformPoint(items[i].rect.center);

                // 2. Đưa cái tâm đó về góc nhìn của cái Khung Vuốt (Viewport)
                Vector3 cardCenterInViewport = scrollRect.viewport.InverseTransformPoint(cardCenterWorld);

                // 3. Đo khoảng cách tuyệt đối từ tâm Thẻ đến tâm Viewport
                float distFromCenter = Mathf.Abs(cardCenterInViewport.x - scrollRect.viewport.rect.center.x);

                // Nếu nằm trong khoảng cách an toàn -> Bật. Bị trượt ra ngoài -> Tắt.
                card.Toggle3DBall(distFromCenter < hideDistance);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        scrollRect.velocity = Vector2.zero;
    }
}