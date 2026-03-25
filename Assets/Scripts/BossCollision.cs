using UnityEngine;
using System.Collections;

public class BossCollision : MonoBehaviour
{
    [SerializeField] private int danio = 1;
    [SerializeField] private float tiempoEntreGolpes = 1.5f;
    [SerializeField] private float distanciaRetroceso = 100f;
    [SerializeField] private float velocidadRetroceso = 7f;

    private float timerGolpe;
    private bool retrocediendo = false;

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

        while (Vector3.Distance(transform.position, posicionDestino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionDestino, velocidadRetroceso * Time.deltaTime);
            yield return null;
        }

        retrocediendo = false;
    }
}