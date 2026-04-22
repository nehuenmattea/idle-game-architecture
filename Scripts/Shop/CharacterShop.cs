using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla la tienda de personajes en formato carrusel.
/// Permite visualizar, comprar y equipar personajes.
/// También actualiza la UI (imágenes, textos, botones) según el estado.
/// </summary>
public class CharacterShop : MonoBehaviour
{
    // Lista de todos los personajes disponibles en la tienda
    public CharacterItem[] personajes;

    // Referencias visuales en la UI
    public Image personajeVisual;   // Imagen del personaje
    public Image fondoVisual;       // Fondo del personaje
    public AudioSource musicaSource; // Música asociada al personaje

    // Referencia al GameManager para acceder al dinero y lógica global
    public GameManager gameManager;

    // Elementos de texto en la UI
    public TMP_Text nombreText;
    public TMP_Text precioText;

    // Botones de interacción
    public Button comprarButton;
    public Button usarButton;

    // Índice del personaje actualmente mostrado en el carrusel
    private int personajeActualIndex = 0;

    /// <summary>
    /// Se ejecuta al iniciar la escena.
    /// Carga los personajes comprados y muestra el primero.
    /// </summary>
    void Start()
    {
        CargarCompras();
        MostrarPersonaje(personajeActualIndex);
    }

    /// <summary>
    /// Avanza al siguiente personaje en el carrusel.
    /// Usa módulo (%) para volver al inicio al llegar al final.
    /// </summary>
    public void SiguientePersonaje()
    {
        personajeActualIndex = (personajeActualIndex + 1) % personajes.Length;
        MostrarPersonaje(personajeActualIndex);
    }

    /// <summary>
    /// Retrocede al personaje anterior en el carrusel.
    /// El "+ personajes.Length" evita índices negativos.
    /// </summary>
    public void AnteriorPersonaje()
    {
        personajeActualIndex = (personajeActualIndex - 1 + personajes.Length) % personajes.Length;
        MostrarPersonaje(personajeActualIndex);
    }

    /// <summary>
    /// Actualiza toda la UI según el personaje seleccionado:
    /// - Imágenes
    /// - Música
    /// - Textos
    /// - Estado de botones
    /// </summary>
    void MostrarPersonaje(int index)
    {
        var personaje = personajes[index];

        // === VISUALES ===
        personajeVisual.sprite = personaje.personajeSprite;
        fondoVisual.sprite = personaje.backgroundSprite;

        // === MÚSICA ===
        musicaSource.clip = personaje.musica;
        musicaSource.Play();

        // === TEXTOS ===
        nombreText.text = personaje.nombre;

        // Si está comprado, no muestra precio
        precioText.text = personaje.comprado 
            ? "Comprado" 
            : personaje.precio + " murkins";

        // === BOTONES ===

        // Comprar: solo si NO está comprado y alcanza el dinero
        comprarButton.interactable = !personaje.comprado && gameManager.count >= personaje.precio;

        // Usar: solo si ya está comprado
        usarButton.interactable = personaje.comprado;
    }

    /// <summary>
    /// Intenta comprar el personaje actual.
    /// - Verifica si no está comprado
    /// - Verifica si alcanza el dinero (PurchaseAction)
    /// - Guarda el progreso en PlayerPrefs
    /// </summary>
    public void Comprar()
    {
        var personaje = personajes[personajeActualIndex];

        if (!personaje.comprado && gameManager.PurchaseAction(personaje.precio))
        {
            // Marcar como comprado
            personaje.comprado = true;

            // Guardar en PlayerPrefs (1 = comprado)
            PlayerPrefs.SetInt("Personaje_" + personaje.nombre, 1);
            PlayerPrefs.Save();

            // Refrescar UI
            MostrarPersonaje(personajeActualIndex);
        }
    }

    /// <summary>
    /// Equipa el personaje actual si está comprado.
    /// También guarda la selección en PlayerPrefs.
    /// </summary>
    public void Usar()
    {
        var personaje = personajes[personajeActualIndex];

        if (personaje.comprado)
        {
            // Cambia el personaje activo en el juego
            gameManager.CambiarPersonaje(personaje);

            // Guarda cuál está equipado
            PlayerPrefs.SetString("Personaje_Equipado", personaje.nombre);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Carga desde PlayerPrefs qué personajes ya fueron comprados.
    /// Si no existe la clave, se asume que NO está comprado.
    /// </summary>
    void CargarCompras()
    {
        foreach (var personaje in personajes)
        {
            personaje.comprado = PlayerPrefs.GetInt("Personaje_" + personaje.nombre, 0) == 1;
        }
    }
}