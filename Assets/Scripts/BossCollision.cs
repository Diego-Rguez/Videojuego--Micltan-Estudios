using UnityEngine;
using System.Collections;

public class BossCollision : MonoBehaviour
{
    [SerializeField] private int danio = 1;
    [SerializeField] private float tiempoEntreGolpes = 1.5f;

    private float timerGolpe;
    private BossMovement bossMovement;

    void Start()
    {
        bossMovement = GetComponent<BossMovement>();
    }

    void Update()
    {
        if (timerGolpe > 0)
        {
            timerGolpe -= Time.deltaTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && timerGolpe <= 0)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.RecibirDanio(danio);
                timerGolpe = tiempoEntreGolpes;

                // Le dice al BossMovement que regrese
                bossMovement.Regresar();
            }
        }
    }
}