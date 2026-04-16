using UnityEngine;
using System.Collections;

public class BossMovement2 : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float velocidadCaminar = 1.9f;
    [SerializeField] private float velocidadRegresar = 10f;
    [SerializeField] private float velocidadDash = 10f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSaltoX = -7f;
    [SerializeField] private float fuerzaSaltoY = 10f;
    [SerializeField] private float duracionSalto = 0.8f;

    [Header("Bola de Fuego")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float velocidadBolaDeFuego = 6f;
    [SerializeField] private int danioBolaDeFuego = 1;

    [Header("Lluvia de Bolas")]
    [SerializeField] private int cantidadLluvia = 3;
    [SerializeField] private float delayEntreBolas = 0.35f;
    [SerializeField] private float dispersacionAngulo = 20f;

    [Header("Dash")]
    [SerializeField] private float duracionDash = 0.35f;
    [SerializeField] private int danioDash = 2;

    [Header("Tiempos")]
    [SerializeField] private float tiempoEsperaMinimo = 0.8f;
    [SerializeField] private float tiempoEsperaMaximo = 1.8f;

    [Header("Probabilidades")]
    [SerializeField] private float probabilidadSalto = 0.33f;
    [SerializeField] private float probabilidadLluvia = 0.33f;

    [Header("Límite Cámara")]
    [SerializeField] private float margenCamara = 1f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Camera camara;
    private float posicionInicialX;

    public enum EstadoBoss { Idle, Caminando, Saltando, LluviaDeBolas, Dash, Regresando }
    public EstadoBoss estadoActual = EstadoBoss.Idle;

    private float tiempoEnEstado;
    private float duracionEstado;

    void Start()
    {
        camara = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        posicionInicialX = transform.position.x;
        CambiarEstado(EstadoBoss.Caminando);
    }

    void Update()
    {
        if (estadoActual == EstadoBoss.Regresando || estadoActual == EstadoBoss.Dash) return;

        ClampPosicionCamara();

        tiempoEnEstado += Time.deltaTime;
        if (tiempoEnEstado >= duracionEstado)
            ElegirSiguienteEstado();

        if (estadoActual == EstadoBoss.Caminando)
            transform.Translate(Vector2.left * velocidadCaminar * Time.deltaTime);
    }

    private void ClampPosicionCamara()
    {
        float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - margenCamara;
        if (transform.position.x > limiteDerecho)
        {
            transform.position = new Vector3(limiteDerecho, transform.position.y, transform.position.z);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void ElegirSiguienteEstado()
    {
        float random = Random.value;

        if (random < probabilidadSalto)
            CambiarEstado(EstadoBoss.Saltando);
        else if (random < probabilidadSalto + probabilidadLluvia)
            CambiarEstado(EstadoBoss.LluviaDeBolas);
        else
            CambiarEstado(EstadoBoss.Dash);
    }

    private void CambiarEstado(EstadoBoss nuevoEstado)
    {
        estadoActual = nuevoEstado;
        tiempoEnEstado = 0f;

        if (nuevoEstado == EstadoBoss.Caminando)
        {
            duracionEstado = Random.Range(1f, 2.5f);
            rb.linearVelocity = Vector2.zero;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }
        else if (nuevoEstado == EstadoBoss.Saltando)
        {
            duracionEstado = duracionSalto + 1.7f;
            StartCoroutine(TelegrafiarYSaltar());
        }
        else if (nuevoEstado == EstadoBoss.LluviaDeBolas)
        {
            duracionEstado = cantidadLluvia * delayEntreBolas + 2f;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(TelegrafiarYLluvia());
        }
        else if (nuevoEstado == EstadoBoss.Dash)
        {
            duracionEstado = duracionDash + 2f;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(TelegrafiarYDash());
        }
    }

    // ── Salto ──────────────────────────────────────────────────────────────────

    private IEnumerator TelegrafiarYSaltar()
    {
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = Color.yellow;
                yield return new WaitForSeconds(0.15f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
            spriteRenderer.color = Color.yellow;
        }

        rb.linearVelocity = new Vector2(fuerzaSaltoX, fuerzaSaltoY);
        StartCoroutine(TerminarSalto());
    }

    private IEnumerator TerminarSalto()
    {
        yield return new WaitForSeconds(duracionSalto);
        if (estadoActual == EstadoBoss.Saltando)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            CambiarEstado(EstadoBoss.Caminando);
        }
    }

    // ── Lluvia de bolas ────────────────────────────────────────────────────────

    private IEnumerator TelegrafiarYLluvia()
    {
        // Parpadeo morado
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.color = new Color(0.6f, 0f, 1f);
                yield return new WaitForSeconds(0.12f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.08f);
            }
            spriteRenderer.color = new Color(0.6f, 0f, 1f);
        }

        // Dispara varias bolas con ángulo variado
        for (int i = 0; i < cantidadLluvia; i++)
        {
            float angulo = Random.Range(-dispersacionAngulo, dispersacionAngulo);
            Vector2 dir = RotarVector(Vector2.left, angulo);
            DisapararBola(dir);
            yield return new WaitForSeconds(delayEntreBolas);
        }

        yield return new WaitForSeconds(0.4f);
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        CambiarEstado(EstadoBoss.Caminando);
    }

    private Vector2 RotarVector(Vector2 v, float grados)
    {
        float rad = grados * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        );
    }

    // ── Dash ───────────────────────────────────────────────────────────────────

    private IEnumerator TelegrafiarYDash()
    {
        // Parpadeo naranja
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 4; i++)
            {
                spriteRenderer.color = new Color(1f, 0.5f, 0f);
                yield return new WaitForSeconds(0.12f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.08f);
            }
            spriteRenderer.color = new Color(1f, 0.5f, 0f);
        }

        // Ejecuta el dash
        float timerDash = 0f;
        while (timerDash < duracionDash)
        {
            float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - margenCamara;
            float limiteIzquierdo = camara.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + margenCamara;

            if (transform.position.x <= limiteIzquierdo)
                break;

            transform.Translate(Vector2.left * velocidadDash * Time.deltaTime);
            timerDash += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        Regresar();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private void DisapararBola(Vector2 direccion)
    {
        if (ProjectilePool.Instancia == null) return;
        Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position;
        BossFireball fireball = ProjectilePool.Instancia.ObtenerFireball(origen);
        fireball.InicializarConDireccion(velocidadBolaDeFuego, danioBolaDeFuego, direccion.normalized);
    }

    public void Regresar()
    {
        if (estadoActual == EstadoBoss.Regresando) return;
        StartCoroutine(RegresarAPosicionInicial());
    }

    private IEnumerator RegresarAPosicionInicial()
    {
        estadoActual = EstadoBoss.Regresando;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - margenCamara;
        float destinoX = Mathf.Min(posicionInicialX, limiteDerecho);
        Vector3 destino = new Vector3(destinoX, transform.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, destino) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadRegresar * Time.deltaTime);
            yield return null;
        }

        transform.position = destino;
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0f;

        float espera = Random.Range(tiempoEsperaMinimo, tiempoEsperaMaximo);
        yield return new WaitForSeconds(espera);

        CambiarEstado(EstadoBoss.Caminando);
    }
}
