using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Pantallas")]
    [SerializeField] private GameObject pantallaInicio;
    [SerializeField] private GameObject pantallaComoJugar;
    [SerializeField] private GameObject pantallaNiveles;

    [Header("Botones - Menú principal")]
    [SerializeField] private Button botonNiveles;
    [SerializeField] private Button botonComoJugar;

    [Header("Botones - Cómo jugar")]
    [SerializeField] private Button botonCerrarComoJugar;

    [Header("Botones - Niveles")]
    [SerializeField] private Button botonNivel1;
    [SerializeField] private Button botonNivel2;
    [SerializeField] private Button botonCerrarNiveles;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaNivel1 = "SampleScene";
    [SerializeField] private string nombreEscenaNivel2 = "Nivel2";

    void Start()
    {
        MostrarPantalla(pantallaInicio);

        botonNiveles.onClick.AddListener(AbrirNiveles);
        botonComoJugar.onClick.AddListener(AbrirComoJugar);
        botonCerrarComoJugar.onClick.AddListener(CerrarComoJugar);
        botonNivel1.onClick.AddListener(CargarNivel1);
        botonNivel2.onClick.AddListener(CargarNivel2);
        botonCerrarNiveles.onClick.AddListener(CerrarNiveles);
    }

    private void MostrarPantalla(GameObject pantalla)
    {
        pantallaInicio.SetActive(pantalla == pantallaInicio);
        pantallaComoJugar.SetActive(pantalla == pantallaComoJugar);
        pantallaNiveles.SetActive(pantalla == pantallaNiveles);
    }

    private void AbrirNiveles()
    {
        MostrarPantalla(pantallaNiveles);
    }

    private void CerrarNiveles()
    {
        MostrarPantalla(pantallaInicio);
    }

    private void AbrirComoJugar()
    {
        MostrarPantalla(pantallaComoJugar);
    }

    private void CerrarComoJugar()
    {
        MostrarPantalla(pantallaInicio);
    }

    private void CargarNivel1()
    {
        SceneManager.LoadScene(nombreEscenaNivel1);
    }

    private void CargarNivel2()
    {
        SceneManager.LoadScene(nombreEscenaNivel2);
    }
}
