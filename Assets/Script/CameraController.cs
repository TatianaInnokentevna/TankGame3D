using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 0.2f; // Уменьшенная чувствительность
    [SerializeField]
    private Transform barrel; // Ссылка на объект Barrel_LOD1
    [SerializeField]
    private Transform tank; // Ссылка на объект танка
    [SerializeField]
    private float barrelRotationMultiplier = 2f; // Умножение угла вращения
    [SerializeField]
    private Vector3 offset = new Vector3(0, 10, -25); // Смещение камеры относительно танка
    private float barrelRotationReduction = 0.5f; // Коэффициент уменьшения амплитуды для пушки

    private Vector3 currentRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Установка начального вращения камеры
        transform.rotation = Quaternion.Euler(20, 0, 0);
    }

    void LateUpdate()
    {
        // Следование камеры за танком с учетом смещения
        transform.position = tank.position + offset;

        // Получение ввода от мыши
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Обновление текущего вращения камеры
        currentRotation.y += mouseX;
        currentRotation.x += mouseY;

        // Ограничение углов вращения камеры
        currentRotation.x = Mathf.Clamp(currentRotation.x, -40f, 40f);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -20f, 20f);

        // Применение ограниченного вращения к камере
        transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0f);

        if (barrel != null)
        {
            float barrelX = Mathf.Clamp(currentRotation.x * barrelRotationMultiplier * barrelRotationReduction, -5f, 5f);
            float barrelY = Mathf.Clamp(currentRotation.y * barrelRotationMultiplier * barrelRotationReduction, -5f, 5f);
            barrel.localEulerAngles = new Vector3(barrelX, barrelY, 0f);
        }
    }
}
