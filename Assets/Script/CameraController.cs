using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 0.2f; // ����������� ����������������
    [SerializeField]
    private Transform barrel; // ������ �� ������ Barrel_LOD1
    [SerializeField]
    private Transform tank; // ������ �� ������ �����
    [SerializeField]
    private float barrelRotationMultiplier = 2f; // ��������� ���� ��������
    [SerializeField]
    private Vector3 offset = new Vector3(0, 10, -25); // �������� ������ ������������ �����
    private float barrelRotationReduction = 0.5f; // ����������� ���������� ��������� ��� �����

    private Vector3 currentRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ��������� ���������� �������� ������
        transform.rotation = Quaternion.Euler(20, 0, 0);
    }

    void LateUpdate()
    {
        // ���������� ������ �� ������ � ������ ��������
        transform.position = tank.position + offset;

        // ��������� ����� �� ����
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // ���������� �������� �������� ������
        currentRotation.y += mouseX;
        currentRotation.x += mouseY;

        // ����������� ����� �������� ������
        currentRotation.x = Mathf.Clamp(currentRotation.x, -40f, 40f);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -20f, 20f);

        // ���������� ������������� �������� � ������
        transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0f);

        if (barrel != null)
        {
            float barrelX = Mathf.Clamp(currentRotation.x * barrelRotationMultiplier * barrelRotationReduction, -5f, 5f);
            float barrelY = Mathf.Clamp(currentRotation.y * barrelRotationMultiplier * barrelRotationReduction, -5f, 5f);
            barrel.localEulerAngles = new Vector3(barrelX, barrelY, 0f);
        }
    }
}
