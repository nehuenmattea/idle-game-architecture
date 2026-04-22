using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la lógica principal del juego Idle:
/// - Manejo del dinero (count)
/// - Generación pasiva (idle)
/// - Click del jugador
/// - Cambio de personaje
/// - Persistencia de datos (PlayerPrefs)
/// </summary>
public class GameManager : MonoBehaviour
{
    // === UI ===
    [SerializeField] TMP_Text countText;         // Dinero en pantalla principal
    [SerializeField] TMP_Text tiendaCountText;   // Dinero en la tienda 
    [SerializeField] TMP_Text incomeText;        // Ganancia por segundo

    // === Upgrades ===
    [SerializeField] StoreUpgrade[] storeUpgrades;

    // === Configuración de actualización idle ===
    [SerializeField] int updatesPerSecond = 5;

    // === Visuales del personaje actual ===
    [SerializeField] Image personajeActual;
    [SerializeField] Image fondoActual;
    [SerializeField] AudioSource musicaActual;

    // Dinero actual del jugador
    [HideInInspector] public float count = 0;

    // Control de tiempo para el cálculo idle
    float nextTimeCheck = 1;

    // Último ingreso calculado por segundo
    float lastIncomeValue = 0;

    // Multiplicadores del personaje
    private float bonusPorClic = 0.02f;
    private float bonusIdle = 1f;

    // Nombre del personaje actualmente equipado
    private string personajeActualNombre;

    /// <summary>
    /// Inicializa el estado del juego:
    /// - Carga personaje equipado
    /// - Carga dinero asociado a ese personaje
    /// - Carga upgrades
    /// - Actualiza UI
    /// </summary>
    private void Start()
    {
        // Obtener personaje guardado (default: Murkins)
        personajeActualNombre = PlayerPrefs.GetString("Personaje_Equipado", "Murkins");

        // Cargar dinero específico de ese personaje
        count = PlayerPrefs.GetFloat($"{personajeActualNombre}_Count", 0);

        // Buscar el personaje en el shop y aplicarlo visualmente
        var shopManager = FindAnyObjectByType<CharacterShopManager>();
        if (shopManager != null)
        {
            foreach (var item in shopManager.personajes)
            {
                if (item.nombre == personajeActualNombre)
                {
                    CambiarPersonaje(item);
                    break;
                }
            }
        }

        // Cargar niveles de upgrades para ese personaje
        foreach (var upgrade in storeUpgrades)
        {
            upgrade.ForceLoad(personajeActualNombre);
        }

        UpdateUI();
    }

    /// <summary>
    /// Loop principal.
    /// Ejecuta el cálculo idle en intervalos definidos.
    /// </summary>
    void Update()
    {
        if (nextTimeCheck < Time.timeSinceLevelLoad)
        {
            IdleCalculate();
            nextTimeCheck = Time.timeSinceLevelLoad + (1f / updatesPerSecond);
        }
    }

    /// <summary>
    /// Calcula el ingreso pasivo total por segundo
    /// sumando todos los upgrades.
    /// </summary>
    void IdleCalculate()
    {
        float sum = 0;

        foreach (var storeUpgrade in storeUpgrades)
        {
            sum += storeUpgrade.CalculateIncomePerSecond();
            storeUpgrade.UpdateUI();
        }

        // Aplicar bonus del personaje
        lastIncomeValue = sum * bonusIdle;

        // Convertir ingreso por segundo a ingreso por frame lógico
        count += lastIncomeValue / updatesPerSecond;

        UpdateUI();
    }

    /// <summary>
    /// Acción de click del jugador:
    /// - Suma 1 base
    /// - Suma un bonus proporcional al ingreso idle
    /// </summary>
    public void ClickAction()
    {
        count++;
        count += lastIncomeValue * bonusPorClic;

        UpdateUI();
    }

    /// <summary>
    /// Intenta realizar una compra.
    /// Devuelve true si se pudo pagar.
    /// </summary>
    public bool PurchaseAction(int cost)
    {
        if (count >= cost)
        {
            count -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Actualiza toda la UI y guarda el dinero actual.
    /// </summary>
    void UpdateUI()
    {
        int roundedCount = Mathf.RoundToInt(count);

        countText.text = roundedCount.ToString();

        if (tiendaCountText != null)
            tiendaCountText.text = roundedCount.ToString();

        incomeText.text = lastIncomeValue.ToString("F1") + "/s";

        // Guardado automático del progreso
        PlayerPrefs.SetFloat($"{personajeActualNombre}_Count", count);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Cambia el personaje activo:
    /// - Actualiza visuales y música
    /// - Aplica bonuses
    /// - Carga progreso independiente
    /// </summary>
    public void CambiarPersonaje(CharacterItem personaje)
    {
        // Visuales
        personajeActual.sprite = personaje.personajeSprite;
        fondoActual.sprite = personaje.backgroundSprite;

        // Música
        musicaActual.clip = personaje.musica;
        musicaActual.Play();

        // Datos del personaje
        personajeActualNombre = personaje.nombre;
        bonusPorClic = personaje.bonusPorClic;
        bonusIdle = personaje.bonusIdle;

        // Cargar dinero propio de ese personaje
        count = PlayerPrefs.GetFloat($"{personajeActualNombre}_Count", 0);

        // Cargar upgrades de ese personaje
        foreach (var upgrade in storeUpgrades)
        {
            upgrade.ForceLoad(personajeActualNombre);
        }

        UpdateUI();
    }

    /// <summary>
    /// Devuelve el dinero actual (para otros sistemas).
    /// </summary>
    public float GetBalance() => count;

    /// <summary>
    /// Devuelve el nombre del personaje actual.
    /// </summary>
    public string GetPersonajeActualNombre() => personajeActualNombre;

    /// <summary>
    /// Guardado final al cerrar el juego.
    /// Asegura persistencia de dinero y upgrades.
    /// </summary>
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat($"{personajeActualNombre}_Count", count);

        foreach (var upgrade in storeUpgrades)
        {
            PlayerPrefs.SetInt(
                $"UpgradeLevel_{upgrade.upgradeName}_{personajeActualNombre}",
                upgrade.level
            );
        }

        PlayerPrefs.Save();
    }
}