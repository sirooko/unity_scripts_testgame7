using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image healthFillImage;  // Fill 이미지 참조

    public void SetHealth(float ratio)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Mathf.Clamp01(ratio);
        }
    }
}
