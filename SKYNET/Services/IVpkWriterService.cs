namespace SKYNET;

/// <summary>
/// Servicio responsable de generar un VPK a partir de una carpeta.
/// </summary>
public interface IVpkWriterService
{
    /// <summary>
    /// Genera el archivo .vpk a partir de la carpeta indicada.
    /// Devuelve la ruta completa del .vpk generado.
    /// </summary>
    string BuildFromFolder(string sourceFolder);
}