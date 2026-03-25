using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Vector2 velocidadActual;
    private int danio;
    private bool inicializada = false;

    [SerializeField] private float gravedad = 6f;

    public void Inicializar(Vector2 dir, float vel, int dmg)
    {
        // La velocidad inicial es la dirección * velocidad
        velocidadActual = dir.normalized * vel;
        danio = dmg;
        inicializada = true;
    }

    void Update()
    {
        if (!inicializada) return;

        // Aplica gravedad manual reduciendo la velocidad vertical cada frame
        velocidadActual.y -= gravedad * Time.deltaTime;

        // Mueve la flecha según su velocidad actual
        transform.position += (Vector3)velocidadActual * Time.deltaTime;

        // Rota la flecha para que siempre apunte hacia donde va
        float angulo = Mathf.Atan2(velocidadActual.y, velocidadActual.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.RecibirDanio(danio);
            }
            Destroy(gameObject);
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}