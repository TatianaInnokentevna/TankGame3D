using UnityEngine;

public class TankGroundCheck : MonoBehaviour
{
    [SerializeField]
    private Transform[] leftWheels; // ����� ������� �����
    [SerializeField]
    private Transform[] rightWheels; // ������ ������� �����
    [SerializeField]
    private LayerMask groundLayer; // ���� �����
    [SerializeField]
    private float checkDistance = 0.5f; // ���������� ��� �������� ������� �����

    private Rigidbody tankRigidbody;

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // �������� ������� �����
        bool leftWheelsOnGround = AreWheelsOnGround(leftWheels);
        bool rightWheelsOnGround = AreWheelsOnGround(rightWheels);

        // ���� ��� ������� �� �������� �����, ��������������� ���������
        if (!leftWheelsOnGround && !rightWheelsOnGround)
        {
            CorrectTankPosition();
        }

        // ������������ �������� �� ���� X � Z
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
        // ������������ ��������� �����
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + 0.5f; // �������� ����� ������� ���� �����������
            tankRigidbody.MovePosition(newPosition);
        }
    }
}
