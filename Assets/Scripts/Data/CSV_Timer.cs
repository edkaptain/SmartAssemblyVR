using UnityEngine;
using System.IO;

public class CSV_Timer : MonoBehaviour
{
    private void Start()
    {
        CreateCSV();
       
    }
    public void CreateCSV()
    {
        string path = Path.Combine(Application.persistentDataPath, "datos.csv");

        using (StreamWriter writer = new StreamWriter(path))
        {
            // Encabezados
            writer.WriteLine("Nombre,Puntuacion,Tiempo");

            // Filas de datos
            writer.WriteLine("Eduardo,100,25.5");
            writer.WriteLine("Jugador2,80,30.2");
        }

        Debug.Log("CSV creado en: " + path);
    }
}
