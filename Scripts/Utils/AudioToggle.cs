using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona el estado del audio del juego, su persistencia y la actualización de la interfaz.
/// </summary>
public class AudioToggle : MonoBehaviour
{
    public AudioSource musicaSource;
    public TMP_Text textoBoton;
    public Button boton; // Asegurate de asignar el botón en el Inspector

    private bool musicaActiva = true;

    void Start()
    {
        // Cargamos el estado guardado. Por defecto es 1 (activo).
        musicaActiva = PlayerPrefs.GetInt("Musica_Activa", 1) == 1;

        // Sincronizamos el componente AudioSource con el estado cargado.
        musicaSource.mute = !musicaActiva;
        ActualizarVisual();
    }

    /// <summary>
    /// Alterna entre activar/desactivar música y guarda el cambio.
    /// </summary>
    public void ToggleMusica()
    {
        musicaActiva = !musicaActiva;
        musicaSource.mute = !musicaActiva;

        // Guardamos el estado para que persista entre sesiones.
        PlayerPrefs.SetInt("Musica_Activa", musicaActiva ? 1 : 0);
        PlayerPrefs.Save();

        ActualizarVisual();
    }
    /// <summary>
    /// Cambia el ícono y el color del botón según el estado actual.
    /// </summary>
    void ActualizarVisual()
    {
        textoBoton.text = musicaActiva ? "🔊" : "🔇";

        // Cambiar color del botón
        Color verde = new Color32(0, 200, 0, 255);
        Color rojo = new Color32(200, 0, 0, 255);

        boton.image.color = musicaActiva ? verde : rojo;
    }
}
