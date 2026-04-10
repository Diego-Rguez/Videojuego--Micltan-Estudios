using UnityEngine;

public class BossFireball : MonoBehaviour
{
    private float velocidad;
    private int danio;

    public void Inicializar(float vel, int dmg)
    {
        velocidad = vel;
        danio = dmg;
    }

    void Update()
    {
        transform.position += new Vector3(-1f, -0.2f, 0f).normalized * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.RecibirDanio(danio);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
