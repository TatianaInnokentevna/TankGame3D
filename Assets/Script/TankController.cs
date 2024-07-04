using UnityEngine;

public class TankController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 50f;

    private Rigidbody rb;
    public Transform centerOfMass;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.centerOfMass = centerOfMass.localPosition;
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        Vector3 movement = transform.forward * move;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        rb.MovePosition(transform.position + transform.forward * move);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        TiltTank(move);
    }

    void TiltTank(float move)
    {
        float tiltAngle = move > 0 ? -10f : (move < 0 ? 10f : 0);
        rb.rotation = Quaternion.Euler(tiltAngle, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z);
    }
}











































/*using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1, 0); // Сместим центр масс вниз для большей устойчивости
                                                 // Найти и отключить все снаряды с тегом "Bullet"
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.fixedDeltaTime;

        Vector3 movement = transform.forward * move;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        rb.MovePosition(rb.position + movement);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}*/
