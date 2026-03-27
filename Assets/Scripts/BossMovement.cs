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

    [Header("Tiempos")]
    [SerializeField] private float tiempoEsperaMinimo = 0.3f;
    [SerializeField] private float tiempoEsperaMaximo = 1f;
    [SerializeField] private float probabilidadSalto = 0.6f;

    [Header("Límite Cámara")]
    [SerializeField] private float margenCamara = 1f;

    private Rigidbody2D rb;
    private Camera camara;
    private float posicionInicialX;

    // Estados del boss
    public enum EstadoBoss { Idle, Caminando, Saltando, Regresando }
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
        if (Random.value < probabilidadSalto)
        {
            CambiarEstado(EstadoBoss.Saltando);
        }
        else
        {
            CambiarEstado(EstadoBoss.Caminando);
        }
    }


    private void CambiarEstado(EstadoBoss nuevoEstado)
    {
        estadoActual = nuevoEstado;
        tiempoEnEstado = 0f;

        if (nuevoEstado == EstadoBoss.Caminando)
        {
            duracionEstado = Random.Range(1f, 3f);
            rb.linearVelocity = Vector2.zero;
        }
        else if (nuevoEstado == EstadoBoss.Saltando)
        {
            duracionEstado = duracionSalto + 1f; // Tiempo extra para que pueda golpear
            rb.linearVelocity = new Vector2(fuerzaSaltoX, fuerzaSaltoY);
            StartCoroutine(TerminarSalto());
        }
    }

    private IEnumerator TerminarSalto()
    {
        yield return new WaitForSeconds(duracionSalto);

        // Al terminar el salto solo cancela la velocidad horizontal
        // NO llama Regresar() — eso lo hace BossCollision al golpear
        if (estadoActual == EstadoBoss.Saltando)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            // Si no golpeó a nadie, camina normalmente
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