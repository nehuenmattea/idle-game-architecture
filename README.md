# 🚀 Unity Idle Clicker - Core Logic Scripts

Este repositorio contiene el núcleo lógico de un juego tipo **Idle Clicker** desarrollado en C# para Unity. El sistema está diseñado para ser modular, permitiendo gestionar la economía, una tienda de mejoras incrementales y un sistema de skins/personajes con bonificadores únicos.

## 📂 Estructura de los Scripts

Los archivos están organizados según su responsabilidad dentro del sistema de juego:

### 🎮 Núcleo y Economía
* **GameManager.cs**: El cerebro del juego. Gestiona el balance de recursos, calcula los ingresos pasivos por segundo y coordina el cambio de personajes y bonificadores.
* **StoreUpgrade.cs**: Lógica para las mejoras comprables. Incluye la fórmula de costo incremental:  
  `Precio = PrecioBase * Multiplicador ^ Nivel`

### 🛒 Sistema de Tienda y Personajes
* **CharacterItem.cs**: Clase contenedora (Data Model) que define los atributos de cada personaje: precio, sprites, música y sus respectivos bonos de clic e idle.
* **CharacterShopManager.cs**: Administrador de la tienda en formato de **Grilla**. Ideal para mostrar todos los personajes disponibles simultáneamente mediante prefabs.
* **CharacterShop.cs**: Administrador de la tienda en formato de **Galería/Carrusel**. Permite la navegación lateral para inspeccionar personajes uno a uno.
* **CharacterItemUI.cs**: Controlador visual de cada ítem de la tienda. Maneja los estados de "Bloqueado", "Comprado" y "Equipado".

### 🛠️ Utilidades y Persistencia
* **AudioToggle.cs**: Gestión del estado del audio (On/Off) con feedback visual y persistencia.
* **Persistencia (PlayerPrefs)**: El sistema utiliza `PlayerPrefs` para guardar automáticamente el progreso, los personajes comprados y el nivel de cada mejora de forma independiente por personaje.

## ⚙️ Características Técnicas
* **Optimización de Cómputo**: El cálculo de ingresos pasivos no se ejecuta en cada frame, sino en intervalos configurables (`updatesPerSecond`) para mejorar el rendimiento.
* **Desacoplamiento UI/Lógica**: La lógica de negocio está separada de la representación visual, facilitando cambios en la interfaz sin romper el núcleo del juego.
* **Progresión Dinámica**: Sistema de desbloqueo en cadena donde los personajes se vuelven disponibles a medida que se adquiere el anterior.

## 🚀 Cómo usar estos scripts
1. Importa la carpeta `Scripts` en tu proyecto de Unity.
2. Asegúrate de tener instalado **TextMeshPro** desde el Package Manager.
3. Configura un `GameManager` en tu escena y vincula los componentes de UI correspondientes.
4. Los personajes se crean como objetos de la clase `CharacterItem` dentro de los arrays de la tienda en el Inspector.

---
*Nota: Este repositorio solo incluye los archivos de código fuente (.cs). Los assets visuales, auditivos y archivos de escena no están incluidos.*