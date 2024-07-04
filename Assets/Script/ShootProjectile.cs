using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject Bullet; // Префаб снаряда
    [SerializeField]
    private Transform FirePoint; // Точка выстрела
    [SerializeField]
    private Transform FirePlace; // Точка для эффекта выстрела
    [SerializeField]
    private GameObject WFXMR_FlameThrower_Looped; // Префаб эффекта огнемета
    [SerializeField]
    private GameObject ExplosionPrefab; // Префаб взрыва
    [SerializeField]
    private float projectileSpeed = 50f; // Скорость снаряда
    [SerializeField]
    private float recoilForce = 1000f; // Сила отдачи
    [SerializeField]
    private float bulletLifetime = 7f; // Время жизни снаряда
    [SerializeField]
    private float explosionDelay = 1f; // Задержка перед взрывом

    private Rigidbody tankRigidbody;
    private bool canShoot = true; // Флаг, позволяющий стрелять

    [SerializeField]
    private float explosionForce = 1000f; // Сила взрыва
    [SerializeField]
    private float explosionRadius = 100f; // Радиус взрыва
   

    void Start()
    {
        tankRigidbody = GetComponentInParent<Rigidbody>(); // Получаем Rigidbody танка

        // Отключаем все объекты с тегом "Bullet" при старте
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.SetActive(false);
        }

        // Отключаем эффект огнемета
        if (WFXMR_FlameThrower_Looped != null)
        {
            WFXMR_FlameThrower_Looped.SetActive(false);
        }


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot) // Проверяем нажатие кнопки мыши
        {
            canShoot = false; // Запрещаем стрелять до следующего раза
            Fire();
        }
    }

    void Fire()
    {
        Debug.Log("Fire method called.");

        // Создаем снаряд на позиции FirePoint
        GameObject projectile = Instantiate(Bullet, FirePoint.position, FirePoint.rotation);
        projectile.SetActive(true); // Активируем снаряд на случай, если он создается неактивным
        Debug.Log("Projectile created at: " + FirePoint.position);

        // Получаем Rigidbody снаряда для настройки его скорости
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            // Задаем скорость движения снаряда по оси FirePoint.forward
            projectileRb.velocity = FirePoint.forward * projectileSpeed;
            Debug.Log("Projectile velocity set: " + projectileRb.velocity);
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on projectile.");
        }

        // Ограничение по высоте снаряда в FixedUpdate
        projectileRb.gameObject.AddComponent<LimitYPosition>();

        // Включаем и затем отключаем эффект огнемета
        if (WFXMR_FlameThrower_Looped != null)
        {
            WFXMR_FlameThrower_Looped.SetActive(true);
            Invoke(nameof(DeactivateFlameEffect), 1f);
            Debug.Log("Flame effect activated at: " + FirePlace.position);
        }

       

        // Создаем эффект взрыва на позиции снаряда через заданное время
        Invoke(nameof(CreateExplosionAtProjectilePosition), explosionDelay);

        // Уничтожаем снаряд через определенное время
        Destroy(projectile, bulletLifetime);
        Debug.Log("Projectile scheduled for destruction in: " + bulletLifetime + " seconds.");

        // Восстанавливаем разрешение на стрельбу через заданное время
        Invoke(nameof(ResetShootPermission), bulletLifetime);


        // Применяем отдачу танка 
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
        // Позиция для создания взрыва - текущая позиция снаряда
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Bullet");
        if (projectiles.Length > 0)
        {
            GameObject lastProjectile = projectiles[projectiles.Length - 1];
            Vector3 position = lastProjectile.transform.position;
            Debug.Log("Attempting to create explosion at: " + position);

            // Создаем эффект взрыва на позиции снаряда
            if (ExplosionPrefab != null)
            {
                GameObject explosion = Instantiate(ExplosionPrefab, position, Quaternion.identity);
                ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    var main = particleSystem.main;
                    main.simulationSpeed = 1f; // Воспроизведение со стандартной скоростью
                    Debug.Log("Explosion particle system found and configured.");
                }
                else
                {
                    Debug.LogWarning("No ParticleSystem found on explosion prefab.");
                }
                Destroy(explosion, 6f); // Эффект взрыва исчезает через 6 секунд
                Debug.Log("Explosion created at: " + position);
            }
            else
            {
                Debug.LogWarning("ExplosionPrefab is not assigned.");
            }

            // Применяем силу взрыва ко всем объектам в радиусе
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
        canShoot = true; // Разрешаем стрелять снова
        Debug.Log("Shoot permission reset.");
    }

    void FixedUpdate()
    {
        // Ограничиваем вращение по осям X и Z
        Vector3 angularVelocity = tankRigidbody.angularVelocity;
        angularVelocity.x = 0f;
        angularVelocity.z = 0f;
        tankRigidbody.angularVelocity = angularVelocity;
    }

}


