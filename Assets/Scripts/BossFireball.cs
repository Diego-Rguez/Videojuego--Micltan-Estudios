using UnityEngine;
using UnityEngine.Pool;

public class BossFireball : MonoBehaviour
{
    private float velocidad;
    private int danio;
    private Vector3 direccion = new Vector3(-1f, -0.2f, 0f);
    private IObjectPool<BossFireball> pool;
    private bool devuelto = false;

    public void SetPool(IObjectPool<BossFireball> p) => pool = p;

    void OnEnable() => devuelto = false;

    // Nivel 1: dirección fija
    public void Inicializar(float vel, int dmg)
    {
        velocidad = vel;
        danio = dmg;
        direccion = new Vector3(-1f, -0.2f, 0f);
    }

    // Nivel 2: dirección libre (lluvia, dash, etc.)
    public void InicializarConDireccion(float vel, int dmg, Vector2 dir)
    {
        velocidad = vel;
        danio = dmg;
        direccion = new Vector3(dir.x, dir.y, 0f);
    }

    void Update()
    {
        transform.position += direccion.normalized * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.RecibirDanio(danio);
            Devolver();
        }
        else if (!other.CompareTag("Boss"))
        {
            Devolver();
        }
    }

    void OnBecameInvisible()
    {
        Devolver();
    }

    private void Devolver()
    {
        if (devuelto) return;
        devuelto = true;
        if (pool != null)
            pool.Release(this);
        else
            Destroy(gameObject);
    }
}
