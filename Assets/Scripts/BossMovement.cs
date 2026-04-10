using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float velocidadCaminar = 1.8f;
    [SerializeField] private float velocidadRegresar = 8f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSaltoX = -7f;
    [SerializeField] private float fuerzaSaltoY = 10f;
    [SerializeField] private float duracionSalto = 0.8f;

    [Header("Bola de Fuego")]
    [SerializeField] private GameObject prefabBolaDeFuego;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float velocidadBolaDeFuego = 6f;
    [SerializeField] private int danioBolaDeFuego = 1;

    [Header("Tiempos")]
    [SerializeField] private float tiempoEsperaMinimo = 0.3f;
    [SerializeField] private float tiempoEsperaMaximo = 1f;
    [SerializeField] private float probabilidadSalto = 0.5f;
    [SerializeField] private float probabilidadDisparo = 0.25f;

    [Header("Límite Cámara")]
    [SerializeField] private float margenCamara = 1f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Camera camara;
    private float posicionInicialX;

    // Estados del boss
    public enum EstadoBoss { Idle, Caminando, Saltando, Disparando, Regresando }
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
        if (estadoActual == EstadoBoss.Regresando) return;

        ClampPosicionCamara();

        tiempoEnEstado += Time.deltaTime;
        if (tiempoEnEstado >= duracionEstado)
        {
            ElegirSiguienteEstado();
        }

        if (estadoActual == EstadoBoss.Caminando)
        {
            transform.Translate(Vector2.left * velocidadCaminar * Time.deltaTime);
        }
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
        else if (random < probabilidadSalto + probabilidadDisparo)
            CambiarEstado(EstadoBoss.Disparando);
        else
            CambiarEstado(EstadoBoss.Caminando);
    }


    private void CambiarEstado(EstadoBoss nuevoEstado)
    {
        estadoActual = nuevoEstado;
        tiempoEnEstado = 0f;

        if (nuevoEstado == EstadoBoss.Caminando)
        {
            duracionEstado = Random.Range(1f, 3f);
            rb.linearVelocity = Vector2.zero;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }
        else if (nuevoEstado == EstadoBoss.Saltando)
        {
            duracionEstado = duracionSalto + 1.7f; // Tiempo extra incluyendo el telegrafeo
            StartCoroutine(TelegrafiarYSaltar());
        }
        else if (nuevoEstado == EstadoBoss.Disparando)
        {
            duracionEstado = 3f;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(TelegrafiarYDisparar());
        }
    }

    private IEnumerator TelegrafiarYSaltar()
    {
        // Parpadeo naranja para avisar al jugador
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = Color.yellow;
                yield return new WaitForSeconds(0.15f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
            spriteRenderer.color = Color.yellow; // amarillo durante el salto
        }

        rb.linearVelocity = new Vector2(fuerzaSaltoX, fuerzaSaltoY);
        StartCoroutine(TerminarSalto());
    }

    private IEnumerator TelegrafiarYDisparar()
    {
        // Parpadeo morado para diferenciarlo del salto (naranja)
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 4; i++)
            {
                spriteRenderer.color = new Color(0.6f, 0f, 1f);
                yield return new WaitForSeconds(0.15f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Dispara la bola de fuego
        if (prefabBolaDeFuego != null)
        {
            Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position;
            GameObject bola = Instantiate(prefabBolaDeFuego, origen, Quaternion.identity);
            BossFireball fireball = bola.GetComponent<BossFireball>();
            if (fireball != null)
                fireball.Inicializar(velocidadBolaDeFuego, danioBolaDeFuego);
        }

        yield return new WaitForSeconds(0.5f);

        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        CambiarEstado(EstadoBoss.Caminando);
    }

    private IEnumerator TerminarSalto()
    {
        yield return new WaitForSeconds(duracionSalto);

        // Al terminar el salto solo cancela la velocidad horizontal
        // NO llama Regresar() — eso lo hace BossCollision al golpear
        if (estadoActual == EstadoBoss.Saltando)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            CambiarEstado(EstadoBoss.Caminando);
        }
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

        // Calcula el límite de cámara y respeta el que sea menor
        float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - margenCamara;
        float destinoX = Mathf.Min(posicionInicialX, limiteDerecho);

        Vector3 destino = new Vector3(destinoX, transform.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, destino) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadRegresar * Time.deltaTime
            );
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