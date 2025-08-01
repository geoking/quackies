using UnityEngine;

public class CauldronBoard : MonoBehaviour
{
    public GameObject tokenSlotPrefab;
    public int numberOfSlots = 35;

    [Header("Spiral Settings")]
    public float a = 0.8f;            // Base radius
    public float b = 0.06f;           // Radius growth per radian
    public float arcLengthStep = 0.4f; // Desired distance between tokens

    void Start()
    {
        CreateAccurateSpiral();
    }

    void CreateAccurateSpiral()
    {
        float theta = 0f;

        for (int i = 0; i < numberOfSlots; i++)
        {
            // Calculate radius for current angle
            float r = a + b * theta;

            // Convert polar to Cartesian
            float x = r * Mathf.Cos(theta);
            float y = r * Mathf.Sin(theta);
            Vector3 position = new Vector3(x, y, 0f);

            GameObject slot = Instantiate(tokenSlotPrefab, position, Quaternion.identity, transform);
            slot.name = $"TokenSlot_{i + 1}";

            // Estimate the next angle using arc length formula for Archimedean spiral
            // Δs ≈ sqrt(r^2 + (dr/dθ)^2) * Δθ  → we solve for Δθ using arc length step
            float drdTheta = b;
            float ds = arcLengthStep;
            float dTheta = ds / Mathf.Sqrt(r * r + drdTheta * drdTheta);

            theta += dTheta;
        }
    }
}