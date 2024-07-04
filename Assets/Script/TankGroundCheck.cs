using UnityEngine;

public class TankGroundCheck : MonoBehaviour
{
    [SerializeField]
    private Transform[] leftWheels; // Левая сторона колес
    [SerializeField]
    private Transform[] rightWheels; // Правая сторона колес
    [SerializeField]
    private LayerMask groundLayer; // Слой земли
    [SerializeField]
    private float checkDistance = 0.5f; // Расстояние для проверки касания земли

    private Rigidbody tankRigidbody;

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Проверка касания земли
        bool leftWheelsOnGround = AreWheelsOnGround(leftWheels);
        bool rightWheelsOnGround = AreWheelsOnGround(rightWheels);

        // Если обе стороны не касаются земли, скорректировать положение
        if (!leftWheelsOnGround && !rightWheelsOnGround)
        {
            CorrectTankPosition();
        }

        // Ограничиваем вращение по осям X и Z
        Vector3 angularVelocity = tankRigidbody.angularVelocity;
        angularVelocity.x = 0f;
        angularVelocity.z = 0f;
        tankRigidbody.angularVelocity = angularVelocity;
    }

    bool AreWheelsOnGround(Transform[] wheels)
    {
        foreach (Transform wheel in wheels)
        {
            if (Physics.Raycast(wheel.position, Vector3.down, checkDistance, groundLayer))
            {
                return true;
            }
        }
        return false;
    }

    void CorrectTankPosition()
    {
        // Корректируем положение танка
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + 0.5f; // Поднятие танка немного выше поверхности
            tankRigidbody.MovePosition(newPosition);
        }
    }
}
