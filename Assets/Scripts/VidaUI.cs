using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [SerializeField] private Slider barraVida;

    public void ActualizarBarra(int vidaActual)
    {
        barraVida.value = vidaActual;
    }

    public void ConfigurarMaximo(int vidaMaxima)
    {
        barraVida.maxValue = vidaMaxima;
        barraVida.value = vidaMaxima;
    }
}