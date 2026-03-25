using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private float velocidadNormal = 1.5f;
    [SerializeField] private float velocidadRapida = 2.5f;

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
    }

    void Update()
    {
        tiempoEnEstado += Time.deltaTime;

        if (tiempoEnEstado >= duracionEstado)
        {
            CambiarEstado();
        }

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
        estaCaminando = !estaCaminando;

        if (estaCaminando)
        {
            duracionEstado = Random.Range(1f, 4f);
        }
        else
        {
            duracionEstado = Random.Range(1f, 3f);
        }
    }
}