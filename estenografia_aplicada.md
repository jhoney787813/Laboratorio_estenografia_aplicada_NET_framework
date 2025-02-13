
# Laboratorio: Ocultamiento de Información en Imágenes Usando Esteganografía

## Universidad Politécnico Grancolombiano
**Especialización en Seguridad de la Información**  
Materia: Criptografía simétrica  
Docente: José Alfonso Valencia Rodríguez  
Autor: Jhon Edison Hincapié  
Año: 2025  

---

## Objetivo

El objetivo de este laboratorio es demostrar cómo se puede ocultar información, en particular un comando de consola que imprime un saludo, dentro de una imagen utilizando **esteganografía**. El proceso implicará:

1. Crear un comando que imprima un saludo en la consola.
2. Ocultar este comando en una imagen mediante esteganografía.
3. Crear una aplicación para extraer el comando oculto y ejecutarlo en la consola.

---

## Justificación

La esteganografía es una técnica que permite ocultar información dentro de otros archivos de manera que el contenido oculto no sea visible para el observador promedio. En el caso de este ejercicio, ocultaremos un comando dentro de una imagen. Esto puede ser útil tanto en escenarios legítimos como maliciosos. En contextos de seguridad, se puede utilizar para ocultar mensajes entre sistemas o incluso comandos que serán ejecutados en otros dispositivos, evitando la detección en tránsito.

---

## Herramientas y Tecnologías Utilizadas

- **Lenguaje de programación**: C# (con .NET 9)
- **Bibliotecas**: `System.Drawing.Common` para manipulación de imágenes.
- **Objetivos del ejercicio**:
  - Ocultar un mensaje de texto en una imagen.
  - Extraer el mensaje oculto y ejecutarlo como un comando en la consola.

---

## Paso a Paso

### Paso 1: Crear el Proyecto en .NET

1. Abre la terminal o Visual Studio y crea un proyecto de consola en C#.

   ```bash
   dotnet new console -n Esteganografia
   cd Esteganografia
   ```

2.  Instala la biblioteca necesaria para manejar imágenes con el siguiente comando:
 ```bash
    dotnet add package System.Drawing.Common
 ```


### Paso 2: Escribir el Código para Ocultar el Comando
Para ocultar el comando en una imagen, primero necesitamos definir qué comando queremos ocultar. En este caso, el comando es:
 ```bash
    echo ¡Hola desde la imagen!
    pause
 ```

 Este comando imprimirá un saludo en la consola y luego hará una pausa hasta que el usuario presione una tecla.


 1. Código para Ocultar el Comando en la Imagen:
Crea un archivo Program.cs con el siguiente código para ocultar el comando dentro de una imagen utilizando esteganografía:
 ```csharp
        using System;
        using System.Drawing;
        using System.Text;

        namespace Esteganografia
        {
            class Program
            {
                static void Main(string[] args)
                {
                    string mensaje = "echo ¡Hola desde la imagen! pause";  // El comando a ocultar
                    string imagenOriginal = "imagen_original.bmp";  // Ruta de la imagen original
                    string imagenOculta = "imagen_oculta.bmp";  // Ruta para la imagen resultante

                    OcultarMensajeEnImagen(imagenOriginal, imagenOculta, mensaje);

                    Console.WriteLine("Mensaje oculto en la imagen.");
                }

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
            }
        }
```

####    Explicación del Código:

-   **Ocultar el mensaje:** El comando de consola (echo ¡Hola desde la imagen! pause) se convierte a bytes utilizando UTF-8. Luego, se recorre cada píxel de la imagen, modificando el valor de los bits menos significativos de la componente azul de cada píxel para almacenar los bits del mensaje.

-  **Guardar la imagen modificada:** La imagen con el mensaje oculto se guarda como una nueva imagen (imagen_oculta.bmp).


### Paso 3: Extraer y Ejecutar el Comando desde la Imagen
Una vez que el mensaje ha sido ocultado en la imagen, necesitamos crear otro programa para extraer el mensaje de la imagen y ejecutarlo como un comando.

1.  Código para Extraer y Ejecutar el Comando:
En el mismo proyecto, crea un nuevo archivo Program.cs para extraer el mensaje oculto y ejecutarlo:

 ```csharp
        using System;
        using System.Drawing;
        using System.Text;
        using System.Diagnostics;

        namespace Esteganografia
        {
            class Program
            {
                static void Main(string[] args)
                {
                    string imagenOculta = "imagen_oculta.bmp";  // Ruta de la imagen con el mensaje oculto
                    string mensajeExtraido = ExtraerMensajeDeImagen(imagenOculta);
                    Console.WriteLine("Mensaje extraído: " + mensajeExtraido);

                    EjecutarComando(mensajeExtraido);
                }

                static string ExtraerMensajeDeImagen(string imagenOculta)
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
                                mensajeBytes[indiceByte] = (byte)((mensajeBytes[indiceByte] << 1) | mensajeBit);

                                if (j % 8 == 7)
                                {
                                    indiceByte++;
                                }
                            }
                        }

                        return Encoding.UTF8.GetString(mensajeBytes, 0, indiceByte);
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
            }
        }
```


####    Explicación del Código para la Extracción y Ejecución:
-   **Extraer los bits:** Recorrimos nuevamente los píxeles de la imagen y extraímos los bits menos significativos de la componente azul de cada píxel. Estos bits se recomponen en el comando original.
-   **Ejecutar el comando:** Usamos Process.Start para ejecutar el comando extraído en la consola de Windows.


## Conclusión

Este laboratorio mostró cómo ocultar un comando dentro de una imagen utilizando esteganografía. A través de este ejercicio, se demostró el proceso de ocultación y extracción de información en archivos aparentemente inofensivos, como imágenes. Este tipo de técnicas puede ser útil en contextos de seguridad informática, aunque también puede ser utilizada con fines maliciosos. La esteganografía sigue siendo una técnica poderosa para ocultar datos y proteger la información sensible.


##  Referencias
**Microsoft Documentation. (2025)**. System.Drawing.Common. Recuperado de https://docs.microsoft.com/en-us/dotnet/api/system.drawing.common

**Wikipedia. (2025)**. Steganography. Recuperado de https://en.wikipedia.org/wiki/Steganography
.NET Documentation. (2025). ProcessStartInfo. Recuperado de https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo