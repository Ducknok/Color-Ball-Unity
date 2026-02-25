using UnityEngine;

public class FastLearnerSkill : MonoBehaviour
{
    // Singleton để UIUpgrade dễ gọi
    public static FastLearnerSkill Instance;

    private void Awake()
    {
        Instance = this;
    }
    public void ActivateSkill(float value)
    {
        GameManager.Instance.uiInfo.SetBonusXP(value);

    }
}