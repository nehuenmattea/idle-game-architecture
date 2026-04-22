using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Representa un upgrade/generador en la tienda.
/// Maneja:
/// - Compra
/// - Escalado de precio
/// - Generación de ingresos
/// - Persistencia por personaje
/// </summary>
public class StoreUpgrade : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text priceText;
    public TMP_Text incomeInfoText;
    public Button button;
    public Image characterImage;
    public TMP_Text upgradeNameText;

    [Header("Generator values")]
    public string upgradeName;
    public int startPrice = 15;
    public float upgradePriceMultiplier = 1.15f;
    public float murkinsPerUpgrade = 0.1f;

    [Header("Managers")]
    public GameManager gameManager;

    [SerializeField] public int level = 0;

    /// <summary>
    /// Acción al comprar el upgrade.
    /// Aumenta nivel si el jugador puede pagarlo.
    /// </summary>
    public void ClickAction()
    {
        int price = CalculatePrice();

        if (gameManager.PurchaseAction(price))
        {
            level++;
            SaveLevel();
            UpdateUI();
        }
    }

    /// <summary>
    /// Actualiza la UI del upgrade:
    /// - Precio
    /// - Producción
    /// - Estado visual (bloqueado/desbloqueado)
    /// </summary>
    public void UpdateUI()
    {
        priceText.text = CalculatePrice().ToString();
        incomeInfoText.text = $"{level} x {murkinsPerUpgrade}/s";

        bool canAfford = gameManager.count >= CalculatePrice();
        button.interactable = canAfford;

        // Visual: oculto hasta ser comprado
        bool isPurchased = level > 0;
        characterImage.color = isPurchased ? Color.white : Color.black;
        upgradeNameText.text = isPurchased ? upgradeName : "?????";
    }

    /// <summary>
    /// Calcula el precio actual usando crecimiento exponencial.
    /// </summary>
    int CalculatePrice()
    {
        return Mathf.RoundToInt(startPrice * Mathf.Pow(upgradePriceMultiplier, level));
    }

    /// <summary>
    /// Devuelve la generación de dinero por segundo de este upgrade.
    /// </summary>
    public float CalculateIncomePerSecond()
    {
        return murkinsPerUpgrade * level;
    }

    /// <summary>
    /// Genera la clave única para guardar el nivel por personaje.
    /// </summary>
    private string GetLevelKey(string personaje)
    {
        return $"UpgradeLevel_{upgradeName}_{personaje}";
    }

    /// <summary>
    /// Guarda el nivel actual en PlayerPrefs.
    /// </summary>
    private void SaveLevel()
    {
        string personaje = gameManager.GetPersonajeActualNombre();
        PlayerPrefs.SetInt(GetLevelKey(personaje), level);
    }

    /// <summary>
    /// Fuerza la carga del nivel desde PlayerPrefs
    /// al cambiar de personaje o iniciar el juego.
    /// </summary>
    public void ForceLoad(string personaje)
    {
        level = PlayerPrefs.GetInt(GetLevelKey(personaje), 0);
        UpdateUI();
    }
}