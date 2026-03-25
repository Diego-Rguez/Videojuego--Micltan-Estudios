using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private float velocidadNormal = 1.5f;
    [SerializeField] private float velocidadRapida = 2.5f;
    [SerializeField] private float fuerzaSaltoX = -5f;
    [SerializeField] private float fuerzaSaltoY = 10f;

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

        int estado = Random.Range(0, 3); // 0 = idle, 1 = caminar, 2 = salto

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
            duracionEstado = 2f; // tiempo del salto
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