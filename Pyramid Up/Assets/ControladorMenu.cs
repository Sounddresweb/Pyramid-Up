using UnityEngine;
using UnityEngine.SceneManagement; // Requisito para cambiar de escenas

public class ControladorMenu : MonoBehaviour
{
    // Esta función cargará tu escena de juego
    public void IniciarJuego()
    {
        // "Level_1" debe ser el nombre exacto de tu escena de juego
        SceneManager.LoadScene("Level_1");
    }

    // Esta función cerrará el juego
    public void SalirDelJuego()
    {
        Debug.Log("El jugador ha salido del juego"); // Para probar en la consola
        Application.Quit(); // Solo funciona en el juego exportado (.exe o .app)
    }
}