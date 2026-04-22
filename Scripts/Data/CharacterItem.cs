using UnityEngine;

/// <summary>
/// Modelo de datos que representa a un personaje/skin dentro del juego.
/// </summary>
[System.Serializable]
public class CharacterItem
{
    public string nombre;
    public Sprite iconoTienda;
    public Sprite personajeSprite;
    public Sprite backgroundSprite;
    public AudioClip musica;
    public int precio;
    public bool comprado;

    public float bonusPorClic = 0.02f; // % extra de ingreso por clic.
    public float bonusIdle = 1f; // Multiplicador base de ingresos.
}
