using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador de la interfaz individual para cada item en la tienda.
/// </summary>
public class CharacterItemUI : MonoBehaviour
{
    [Header("Componentes Visuales")]
    public Image icon;
    public Image backgroundImage; 
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button actionButton;
    public TMP_Text buttonText;

    private CharacterShopManager shopManager;
    private int index;

    /// <summary>
    /// Configura el elemento de la lista con la lógica correspondiente a su estado.
    /// </summary>
    public void Setup(CharacterShopManager manager, int i, CharacterItem data, bool isOwned, bool isEquipped, bool isUnlocked)
    {
        shopManager = manager;
        index = i;

        icon.sprite = data.iconoTienda;
        nameText.text = data.nombre;

        // Limpiamos eventos previos para evitar ejecuciones múltiples.
        actionButton.onClick.RemoveAllListeners();

        // Lógica para personajes que aún no se pueden comprar (bloqueados por progresión).
        if (!isUnlocked)
        {
            EstablecerEstadoBloqueado();
            return;
        }

        // Si llegamos aquí, el personaje es visible.
        icon.color = Color.white;
        if (backgroundImage) backgroundImage.color = Color.white;

        // Definimos el texto del precio.
        priceText.text = isOwned ? "Comprado" : "$" + data.precio.ToString();

        // Lógica de estados del botón principal.
        if (isEquipped)
        {
            ConfigurarBoton("Equipado", false, new Color32(50, 50, 50, 255));
        }
        else if (isOwned)
        {
            ConfigurarBoton("Equipar", true, new Color32(160, 160, 160, 255));
            actionButton.onClick.AddListener(() => shopManager.EquipCharacter(index));
        }
        else
        {
            // Estado de compra: El color depende de si al usuario le alcanza el dinero.
            bool puedeComprar = (int)shopManager.GetBalance() >= data.precio;
            Color32 colorCompra = puedeComprar ? new Color32(0, 200, 0, 255) : new Color32(200, 0, 0, 255);
            
            ConfigurarBoton("Comprar", true, colorCompra);
            actionButton.onClick.AddListener(() => shopManager.BuyCharacter(index));
        }
    }

    /// <summary>
    /// Aplica el efecto visual de "misterio" para items no desbloqueados.
    /// </summary>
    private void EstablecerEstadoBloqueado()
    {
        icon.color = Color.black; 
        if (backgroundImage) backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        nameText.text = "???";
        priceText.text = "???";
        ConfigurarBoton("Bloqueado", false, new Color32(80, 80, 80, 255));
    }

    /// <summary>
    /// Helper para cambiar propiedades del botón de acción rápidamente.
    /// </summary>
    private void ConfigurarBoton(string texto, bool interactuable, Color32 color)
    {
        buttonText.text = texto;
        actionButton.interactable = interactuable;
        actionButton.image.color = color;
    }
}