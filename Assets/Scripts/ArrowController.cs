using UnityEngine;
using UnityEngine.Pool;

public class ArrowController : MonoBehaviour
{
    private Vector2 velocidadActual;
    private int danio;
    private bool inicializada = false;
    private bool devuelto = false;
    private IObjectPool<ArrowController> pool;

    [SerializeField] private float gravedad = 6f;

    public void SetPool(IObjectPool<ArrowController> p) => pool = p;

    void OnEnable()
    {
        inicializada = false;
        devuelto = false;
        velocidadActual = Vector2.zero;
    }

    public void Inicializar(Vector2 dir, float vel, int dmg)
    {
        velocidadActual = dir.normalized * vel;
        danio = dmg;
        inicializada = true;
    }

    void Update()
    {
        if (!inicializada) return;

        velocidadActual.y -= gravedad * Time.deltaTime;
        transform.position += (Vector3)velocidadActual * Time.deltaTime;

        float angulo = Mathf.Atan2(velocidadActual.y, velocidadActual.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
                bossHealth.RecibirDanio(danio);
            Devolver();
            return;
        }

        if (!other.CompareTag("Player"))
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
