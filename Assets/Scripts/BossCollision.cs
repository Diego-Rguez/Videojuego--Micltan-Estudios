using UnityEngine;
using System.Collections;

public class BossCollision : MonoBehaviour
{
    [SerializeField] private int danio = 1;
    [SerializeField] private float tiempoEntreGolpes = 1.5f;
    [SerializeField] private float distanciaRetroceso = 25f;
    [SerializeField] private float velocidadRetroceso = 7f;

    private float timerGolpe;
    private bool retrocediendo = false;
    private Camera camara;

    void Start()
    {
        camara = Camera.main;
    }

    void Update()
    {
        if (timerGolpe > 0)
        {
            timerGolpe -= Time.deltaTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && timerGolpe <= 0)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !retrocediendo)
            {
                playerHealth.RecibirDanio(danio);
                timerGolpe = tiempoEntreGolpes;
                StartCoroutine(RetrocederSuavemente());
            }
        }
    }

    private IEnumerator RetrocederSuavemente()
    {
        retrocediendo = true;

        Vector3 posicionDestino = new Vector3(
            transform.position.x + distanciaRetroceso,
            transform.position.y,
            transform.position.z
        );

        // Calcula el límite derecho de la cámara con margen del ancho del boss
        float anchoBoss = GetComponent<SpriteRenderer>() != null
            ? GetComponent<SpriteRenderer>().bounds.extents.x
            : 0.5f;
        float limiteDerecho = camara.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - anchoBoss;

        // Recorta el destino si se pasa del límite de cámara
        if (posicionDestino.x > limiteDerecho)
        {
            posicionDestino.x = limiteDerecho;
        }

        while (Vector3.Distance(transform.position, posicionDestino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                posicionDestino,
                velocidadRetroceso * Time.deltaTime
            );
            yield return null;
        }

        retrocediendo = false;
    }
}