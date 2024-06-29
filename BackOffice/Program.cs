using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
namespace BackOffice
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            /*string filePath = @"C:\docs\latota.txt"; // Reemplaza con la ruta donde quieras guardar el archivo

            // Escribir datos en el archivo
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine();
                writer.WriteLine("Puedes escribir cualquier cosa que desees guardar.");
            }

            Console.WriteLine("Datos escritos en el archivo.");
            }*/
        }
    }
}
