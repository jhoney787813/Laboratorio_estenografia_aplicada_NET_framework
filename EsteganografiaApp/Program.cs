using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Esteganografia
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando lenctura");
            string mensaje = "echo ¡Bienvenidos al maravilloso mundo de la programación,YA ERES HACKER !";  // El comando a ocultar
            string imagenOriginal = @"C:\\Users\\jhon_hincapie\\Downloads\\LABOTARIORIOS\\perrito.png";  // Ruta de la imagen original
            string imagenOculta = @"C:\\Users\\jhon_hincapie\\Downloads\\LABOTARIORIOS\\perritoCodificado12022025.png";  // Ruta para la imagen resultante

            OcultarMensajeEnImagen(imagenOriginal, imagenOculta, mensaje);
            Console.WriteLine("Mensaje oculto en la imagen.");
            //------------------
            string mensajeExtraido = ExtraerMensajeDeImagen(imagenOculta, mensaje);
            Console.WriteLine("Mensaje extraído: " + mensajeExtraido);
            EjecutarComando(mensajeExtraido);

            Console.WriteLine("-----Finalizo lenctura de comando-----");

        }

        #region Metodos privados
        static void OcultarMensajeEnImagen(string imagenOriginal, string imagenOculta, string mensaje)
        {
            byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);
            int mensajeLength = mensajeBytes.Length;

            using (Bitmap imagen = new Bitmap(imagenOriginal))
            {
                int indiceByte = 0;
                for (int i = 0; i < imagen.Height; i++)
                {
                    for (int j = 0; j < imagen.Width; j++)
                    {
                        Color pixel = imagen.GetPixel(j, i);
                        int pixelAzul = pixel.B;

                        if (indiceByte < mensajeLength)
                        {
                            byte mensajeBit = (byte)((mensajeBytes[indiceByte] >> (7 - (j % 8))) & 1);
                            pixelAzul = (pixelAzul & 0xFE) | mensajeBit;
                            imagen.SetPixel(j, i, Color.FromArgb(pixel.R, pixel.G, pixelAzul));

                            if (j % 8 == 7)
                            {
                                indiceByte++;
                            }
                        }
                    }
                }

                imagen.Save(imagenOculta);
            }
        }

        static string ExtraerMensajeDeImagen(string imagenOculta, string mensajeOriginal)
        {
            try
            {


                StringBuilder mensaje = new StringBuilder();
                using (Bitmap imagen = new Bitmap(imagenOculta))
                {
                    int indiceByte = 0;
                    byte[] mensajeBytes = new byte[1024];

                    for (int i = 0; i < imagen.Height; i++)
                    {
                        for (int j = 0; j < imagen.Width; j++)
                        {
                            Color pixel = imagen.GetPixel(j, i);
                            byte pixelAzul = pixel.B;

                            byte mensajeBit = (byte)(pixelAzul & 1);


                            if (indiceByte >= mensajeBytes.Length)
                            {
                                Array.Resize(ref mensajeBytes, mensajeBytes.Length * 2);  // Doblamos el tamaño del arreglo
                            }

                            mensajeBytes[indiceByte] = (byte)((mensajeBytes[indiceByte] << 1) | mensajeBit);

                            if (j % 8 == 7)
                            {
                                indiceByte++;
                            }
                        }
                    }

                    return Encoding.UTF8.GetString(mensajeBytes, 0, indiceByte).Substring(0, mensajeOriginal.Length);
                }
            }
            catch (Exception ex)
            {
                return "error al buscar mensaje";
            }
        }

        static void EjecutarComando(string comando)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + comando,
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        #endregion
    }
}