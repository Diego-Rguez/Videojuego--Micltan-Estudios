using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [SerializeField] private Slider barraVida;
    [SerializeField] private Transform objetivo;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.2f, 0);

    void Start()
    {

    }
    
    void Update()
    {
        transform.position = objetivo.position + offset;
    }

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