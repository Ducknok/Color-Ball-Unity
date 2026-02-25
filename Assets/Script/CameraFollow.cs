using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset;

    void Start()
    {
        this.offset = this.transform.position - PlayerController.Instance.gameObject.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerController.Instance.isDead)
        {
            //Debug.Log("CameraFollow: Player chết, dừng đuổi theo!");
            this.enabled = false;
            return;
        }
        this.FollowPlayer();

    }
    void FollowPlayer()
    {
        Vector3 targerPos = PlayerController.Instance.gameObject.transform.position + offset;
        targerPos.x = 0;
        this.transform.position = targerPos;
    }
}
