using UnityEngine;

public class BossCollision2 : MonoBehaviour
{
    [SerializeField] private int danio = 1;
    [SerializeField] private float tiempoEntreGolpes = 1.5f;

    private float timerGolpe;
    private BossMovement2 bossMovement;

    void Start()
    {
        bossMovement = GetComponent<BossMovement2>();
    }

    void Update()
    {
        if (timerGolpe > 0)
            timerGolpe -= Time.deltaTime;
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
                bossMovement.Regresar();
            }
        }
    }
}
