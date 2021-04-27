using UnityEngine;

public class ChargeBar : MonoBehaviour
{
    [SerializeField] float maxScaleX;
    [SerializeField] int maxCharge;

    public void SetCharge(int charge)
    {
        if (charge > maxCharge) charge = maxCharge;

        Vector2 scale = transform.localScale;
        scale.x = (maxScaleX / maxCharge) * charge;
        transform.localScale = scale;
    }

    public void ResetCharge()
    {
        Vector2 scale = transform.localScale;
        scale.x = 0.0f;
        transform.localScale = scale;
    }
}