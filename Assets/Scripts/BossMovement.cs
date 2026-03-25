using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 1.5f;
    [SerializeField] private float velocidadRapida = 2.5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSaltoX = -5f;
    [SerializeField] private float fuerzaSaltoY = 10f;

    [Header("Límites")]
    [SerializeField] private Transform jugador;
    [SerializeField] private float distanciaMinima = 1f;

    private Rigidbody2D rb;
    private bool estaSaltando;
    private float tiempoEnEstado;
    private float duracionEstado;
    private bool estaCaminando;
    private Camera camara;

    void Start()
    {
        camara = Camera.main;
        estaCaminando = true;
        duracionEstado = Random.Range(1f, 4f);
        tiempoEnEstado = 0f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        tiempoEnEstado += Time.deltaTime;
        if (tiempoEnEstado >= duracionEstado)
        {
            CambiarEstado();
        }

        if (estaSaltando) return;

        if (EstaSaliendoDeCamara())
        {
            MoverseAIzquierda(velocidadRapida);
        }
        else if (estaCaminando)
        {
            MoverseAIzquierda(velocidadNormal);
        }
    }

    void LateUpdate()
    {
        if (jugador != null)
        {
            // Límite izquierdo — no puede pasar al lado del jugador
            float limiteIzquierdo = jugador.position.x + distanciaMinima;
            if (transform.position.x < limiteIzquierdo)
            {
                transform.position = new Vector3(
                    limiteIzquierdo,
                    transform.position.y,
                    transform.position.z
                );
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                estaSaltando = false;
            }

            // Límite derecho — no puede salirse de la cámara
            // Después
            float anchoBoss = GetComponent<SpriteRenderer>() != null
                ? GetComponent<SpriteRenderer>().bounds.extents.x
                : 0.5f;
            float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - anchoBoss;
            if (transform.position.x > limiteDerecho)
            {
                transform.position = new Vector3(
                    limiteDerecho,
                    transform.position.y,
                    transform.position.z
                );
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    private void MoverseAIzquierda(float velocidad)
    {
        transform.Translate(Vector2.left * velocidad * Time.deltaTime);
    }

    private bool EstaSaliendoDeCamara()
    {
        float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        return transform.position.x > limiteDerecho - 1f;
    }

    private void CambiarEstado()
    {
        tiempoEnEstado = 0f;
        int estado = Random.Range(0, 3);

        if (estado == 0)
        {
            estaCaminando = false;
            estaSaltando = false;
            duracionEstado = Random.Range(1f, 3f);
        }
        else if (estado == 1)
        {
            estaCaminando = true;
            estaSaltando = false;
            duracionEstado = Random.Range(1f, 4f);
        }
        else if (estado == 2)
        {
            estaSaltando = true;
            estaCaminando = false;
            Saltar();
            duracionEstado = 2f;
        }
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(fuerzaSaltoX, fuerzaSaltoY);
        Invoke(nameof(TerminarSalto), 0.8f);
    }

    private void TerminarSalto()
    {
        estaSaltando = false;
    }
}