using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int vidaMaxima = 3;
    [SerializeField] private VidaUI vidaUI;

    private int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
        vidaUI.ConfigurarMaximo(vidaMaxima);
    }

    public void RecibirDanio(int danio)
    {
        vidaActual -= danio;
        vidaActual = Mathf.Max(vidaActual, 0);
        vidaUI.ActualizarBarra(vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("El jugador murio");
        gameObject.SetActive(false);
    }
}