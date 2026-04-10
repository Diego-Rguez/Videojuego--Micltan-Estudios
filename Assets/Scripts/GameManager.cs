using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Pantallas")]
    [SerializeField] private GameObject pantallaGameOver;
    [SerializeField] private GameObject pantallaVictoria;

    [Header("Botones Game Over")]
    [SerializeField] private Button botonReintentarGameOver;
    [SerializeField] private Button botonMenuGameOver;

    [Header("Botones Victoria")]
    [SerializeField] private Button botonReintentarVictoria;
    [SerializeField] private Button botonMenuVictoria;

    [Header("Configuración")]
    [SerializeField] private float pausaAntesDePanel = 2f;
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    private bool juegoTerminado = false;

    void Awake()
    {
        // Patrón Singleton — asegura que solo exista un GameManager
        if (Instancia == null)
            Instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Asegura que ambas pantallas estén ocultas al inicio
        pantallaGameOver.SetActive(false);
        pantallaVictoria.SetActive(false);

        // Asigna los botones
        botonReintentarGameOver.onClick.AddListener(Reintentar);
        botonMenuGameOver.onClick.AddListener(IrAlMenu);
        botonReintentarVictoria.onClick.AddListener(Reintentar);
        botonMenuVictoria.onClick.AddListener(IrAlMenu);
    }

    // PlayerHealth llama este método cuando el jugador muere
    public void TriggerGameOver()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        StartCoroutine(MostrarPantallaConPausa(pantallaGameOver));
    }

    // BossHealth llama este método cuando el boss muere
    public void TriggerVictoria()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        StartCoroutine(VictoriaConSlowMotion());
    }

    private IEnumerator VictoriaConSlowMotion()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(0.5f);
        pantallaVictoria.SetActive(true);
    }

    private IEnumerator MostrarPantallaConPausa(GameObject pantalla)
    {
        // Congela el juego pero espera en tiempo real
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pausaAntesDePanel);
        pantalla.SetActive(true);
    }

    private void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}