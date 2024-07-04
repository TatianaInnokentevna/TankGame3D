using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject Bullet; // ������ �������
    [SerializeField]
    private Transform FirePoint; // ����� ��������
    [SerializeField]
    private Transform FirePlace; // ����� ��� ������� ��������
    [SerializeField]
    private GameObject WFXMR_FlameThrower_Looped; // ������ ������� ��������
    [SerializeField]
    private GameObject ExplosionPrefab; // ������ ������
    [SerializeField]
    private float projectileSpeed = 50f; // �������� �������
    [SerializeField]
    private float recoilForce = 1000f; // ���� ������
    [SerializeField]
    private float bulletLifetime = 7f; // ����� ����� �������
    [SerializeField]
    private float explosionDelay = 1f; // �������� ����� �������

    private Rigidbody tankRigidbody;
    private bool canShoot = true; // ����, ����������� ��������

    [SerializeField]
    private float explosionForce = 1000f; // ���� ������
    [SerializeField]
    private float explosionRadius = 100f; // ������ ������
   

    void Start()
    {
        tankRigidbody = GetComponentInParent<Rigidbody>(); // �������� Rigidbody �����

        // ��������� ��� ������� � ����� "Bullet" ��� ������
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.SetActive(false);
        }

        // ��������� ������ ��������
        if (WFXMR_FlameThrower_Looped != null)
        {
            WFXMR_FlameThrower_Looped.SetActive(false);
        }


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot) // ��������� ������� ������ ����
        {
            canShoot = false; // ��������� �������� �� ���������� ����
            Fire();
        }
    }

    void Fire()
    {
        Debug.Log("Fire method called.");

        // ������� ������ �� ������� FirePoint
        GameObject projectile = Instantiate(Bullet, FirePoint.position, FirePoint.rotation);
        projectile.SetActive(true); // ���������� ������ �� ������, ���� �� ��������� ����������
        Debug.Log("Projectile created at: " + FirePoint.position);

        // �������� Rigidbody ������� ��� ��������� ��� ��������
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            // ������ �������� �������� ������� �� ��� FirePoint.forward
            projectileRb.velocity = FirePoint.forward * projectileSpeed;
            Debug.Log("Projectile velocity set: " + projectileRb.velocity);
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on projectile.");
        }

        // ����������� �� ������ ������� � FixedUpdate
        projectileRb.gameObject.AddComponent<LimitYPosition>();

        // �������� � ����� ��������� ������ ��������
        if (WFXMR_FlameThrower_Looped != null)
        {
            WFXMR_FlameThrower_Looped.SetActive(true);
            Invoke(nameof(DeactivateFlameEffect), 1f);
            Debug.Log("Flame effect activated at: " + FirePlace.position);
        }

       

        // ������� ������ ������ �� ������� ������� ����� �������� �����
        Invoke(nameof(CreateExplosionAtProjectilePosition), explosionDelay);

        // ���������� ������ ����� ������������ �����
        Destroy(projectile, bulletLifetime);
        Debug.Log("Projectile scheduled for destruction in: " + bulletLifetime + " seconds.");

        // ��������������� ���������� �� �������� ����� �������� �����
        Invoke(nameof(ResetShootPermission), bulletLifetime);


        // ��������� ������ ����� 
        if (tankRigidbody != null)
        {
            tankRigidbody.AddForce(-FirePlace.forward * recoilForce, ForceMode.Impulse);
            Debug.Log("Recoil force applied: " + (-FirePlace.forward * recoilForce));
        }
    }


    void DeactivateFlameEffect()
    {
        if (WFXMR_FlameThrower_Looped != null)
        {
            WFXMR_FlameThrower_Looped.SetActive(false);
            Debug.Log("Flame effect deactivated.");
        }
    }



    void CreateExplosionAtProjectilePosition()
    {
        // ������� ��� �������� ������ - ������� ������� �������
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Bullet");
        if (projectiles.Length > 0)
        {
            GameObject lastProjectile = projectiles[projectiles.Length - 1];
            Vector3 position = lastProjectile.transform.position;
            Debug.Log("Attempting to create explosion at: " + position);

            // ������� ������ ������ �� ������� �������
            if (ExplosionPrefab != null)
            {
                GameObject explosion = Instantiate(ExplosionPrefab, position, Quaternion.identity);
                ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    var main = particleSystem.main;
                    main.simulationSpeed = 1f; // ��������������� �� ����������� ���������
                    Debug.Log("Explosion particle system found and configured.");
                }
                else
                {
                    Debug.LogWarning("No ParticleSystem found on explosion prefab.");
                }
                Destroy(explosion, 6f); // ������ ������ �������� ����� 6 ������
                Debug.Log("Explosion created at: " + position);
            }
            else
            {
                Debug.LogWarning("ExplosionPrefab is not assigned.");
            }

            // ��������� ���� ������ �� ���� �������� � �������
            Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, position, explosionRadius);
                }
            }
        }
        else
        {
            Debug.LogWarning("No projectiles found to create explosion.");
        }
    }

    void ResetShootPermission()
    {
        canShoot = true; // ��������� �������� �����
        Debug.Log("Shoot permission reset.");
    }

    void FixedUpdate()
    {
        // ������������ �������� �� ���� X � Z
        Vector3 angularVelocity = tankRigidbody.angularVelocity;
        angularVelocity.x = 0f;
        angularVelocity.z = 0f;
        tankRigidbody.angularVelocity = angularVelocity;
    }

}


