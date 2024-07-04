using UnityEngine;

public class LimitYPosition : MonoBehaviour
{
    public float maxY = 1f; // Максимальное значение Y

    void FixedUpdate()
    {
        if (transform.position.y > maxY)
        {
            Vector3 position = transform.position;
            position.y = maxY;
            transform.position = position;
        }
    }
}
