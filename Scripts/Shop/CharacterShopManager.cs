using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Administrador principal de la tienda basada en lista dinámica (prefabs).
/// </summary>
public class CharacterShopManager : MonoBehaviour
{
    public GameObject panelTienda;
    public Transform contentHolder;
    public GameObject characterCardPrefab;
    public CharacterItem[] personajes;

    public TextMeshProUGUI nombrePersonajeText;
    public Image personajeVisual;
    public Image fondoVisual;
    public AudioSource musicaSource;
    public GameManager gameManager;
    public GameObject personajeActualGO;
    public GameObject fondoActualGO;

    private float musicaTime = 0f;
    private int currentCharacterIndex = -1;
    private bool[] owned;

    void Start()
    {
        owned = new bool[personajes.Length];

        // Inicialización de persistencia y desbloqueo del primer personaje.
        for (int i = 0; i < personajes.Length; i++)
        {
            string key = "Personaje_" + personajes[i].nombre;
            owned[i] = PlayerPrefs.GetInt(key, (i == 0 ? 1 : 0)) == 1;

            if (i == 0 && PlayerPrefs.GetInt(key, -1) == -1)
            {
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
            }
        }
        
        if (!PlayerPrefs.HasKey("Personaje_Equipado"))
        {
            // Cargar el personaje que el usuario dejó equipado.
            PlayerPrefs.SetString("Personaje_Equipado", personajes[0].nombre);
            PlayerPrefs.Save();
        }

        string personajeEquipado = PlayerPrefs.GetString("Personaje_Equipado");
        for (int i = 0; i < personajes.Length; i++)
        {
            if (personajes[i].nombre == personajeEquipado)
            {
                EquipCharacter(i);
                break;
            }
        }

        panelTienda.SetActive(false);
    }

    /// <summary>
    /// Abre la UI de la tienda y pausa elementos visuales/sonoros del juego base.
    /// </summary>
    public void AbrirTienda()
    {
        panelTienda.SetActive(true);
        personajeActualGO.SetActive(false);
        fondoActualGO.SetActive(false);

        if (musicaSource.isPlaying)
        {
            musicaTime = musicaSource.time;
            musicaSource.Pause();
        }

        ActualizarTienda();
    }

    public void CerrarTienda()
    {
        panelTienda.SetActive(false);
        personajeActualGO.SetActive(true);
        fondoActualGO.SetActive(true);

        musicaSource.time = musicaTime;
        musicaSource.Play();
    }

    /// <summary>
    /// Limpia y regenera la lista de personajes en la UI.
    /// </summary>
    void ActualizarTienda()
    {
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);

        for (int i = 0; i < personajes.Length; i++)
        {
            var obj = Instantiate(characterCardPrefab, contentHolder);
            var ui = obj.GetComponent<CharacterItemUI>();

            bool isOwned = owned[i];
            bool isEquipped = i == currentCharacterIndex;

            // Lógica de progresión: un personaje se desbloquea si posees el anterior.
            bool isUnlocked = i == 0 || owned[i - 1]; // solo está desbloqueado si el anterior fue comprado

            ui.Setup(this, i, personajes[i], isOwned, isEquipped, isUnlocked);
        }
    }

    /// <summary>
    /// Lógica de compra. Verifica balance y actualiza el estado 'owned'.
    /// </summary>
    public void BuyCharacter(int index)
    {
        if (!owned[index - 1]) return; // Doble validación de seguridad.

        if (gameManager.PurchaseAction(personajes[index].precio))
        {
            owned[index] = true;
            PlayerPrefs.SetInt("Personaje_" + personajes[index].nombre, 1);
            PlayerPrefs.Save();
            EquipCharacter(index);
        }
    }

    /// <summary>
    /// Actualiza el estado visual del juego y el GameManager con el nuevo personaje.
    /// </summary>
    public void EquipCharacter(int index)
    {
        currentCharacterIndex = index;

        personajeVisual.sprite = personajes[index].personajeSprite;
        fondoVisual.sprite = personajes[index].backgroundSprite;
        musicaSource.clip = personajes[index].musica;
        nombrePersonajeText.text = personajes[index].nombre;
        musicaSource.Play();

        gameManager.CambiarPersonaje(personajes[index]);

        PlayerPrefs.SetString("Personaje_Equipado", personajes[index].nombre);
        PlayerPrefs.Save();

        ActualizarTienda();
    }

    public bool IsOwned(int index)
    {
        return owned[index];
    }

    public int GetBalance()
    {
        return Mathf.RoundToInt(gameManager.GetBalance());
    }
}
