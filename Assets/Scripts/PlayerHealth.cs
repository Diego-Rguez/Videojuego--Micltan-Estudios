using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int vidaMaxima = 3;
    [SerializeField] private VidaUI vidaUI;
    [SerializeField] private Image flashDanio;

    private int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
        vidaUI.ConfigurarMaximo(vidaMaxima);

        if (flashDanio != null)
            flashDanio.color = new Color(1f, 0f, 0f, 0f);
    }

    public void RecibirDanio(int danio)
    {
        vidaActual -= danio;
        vidaActual = Mathf.Max(vidaActual, 0);
        vidaUI.ActualizarBarra(vidaActual);

        if (flashDanio != null)
            StartCoroutine(EfectoFlashDanio());

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private IEnumerator EfectoFlashDanio()
    {
        flashDanio.color = new Color(1f, 0f, 0f, 0.45f);
        float duracion = 0.35f;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.45f, 0f, tiempo / duracion);
            flashDanio.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
        flashDanio.color = new Color(1f, 0f, 0f, 0f);
    }

    private void Morir()
    {
        Debug.Log("El jugador murio");
        gameObject.SetActive(false);
        GameManager.Instancia.TriggerGameOver();
    }
}