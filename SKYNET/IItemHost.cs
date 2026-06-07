namespace SKYNET;

/// <summary>
/// Interfaz para formularios que pueden hospedar ItemControl.
/// Permite que ItemControl funcione tanto con frmItems como con frmTools.
/// </summary>
public interface IItemHost
{
    /// <summary>
    /// Devuelve el botón OK que se "clickea" cuando el usuario selecciona un item.
    /// </summary>
    Button GetOkButton();

    /// <summary>
    /// Carga los estilos de un item con styles disponibles.
    /// </summary>
    void LoadStyles(Items item);
}