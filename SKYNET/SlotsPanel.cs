namespace SKYNET;

/// <summary>
/// Panel que contiene los slots de items de un héroe.
/// Hace wrap automático cuando no caben en una sola fila
/// y agrega scroll vertical si es necesario.
/// </summary>
public class SlotsPanel : FlowLayoutPanel
{
    public SlotsPanel()
    {
        // Configuración del FlowLayout
        FlowDirection = FlowDirection.LeftToRight;
        WrapContents = true;

        // Scroll automático cuando no caben todos
        AutoScroll = true;

        // Para que se vea consistente con el resto de la UI
        BackColor = Color.FromArgb(31, 32, 35);

        // Padding interno para que no se peguen los bordes
        Padding = new Padding(3);
    }

    /// <summary>
    /// Agrega un slot al panel.
    /// El margin se aplica para separar visualmente cada slot.
    /// </summary>
    public void AddSlot(Control slot)
    {
        slot.Margin = new Padding(2);
        Controls.Add(slot);
    }

    /// <summary>
    /// Limpia todos los slots y libera recursos.
    /// </summary>
    public new void Clear()
    {
        // Disposeamos los controles para evitar leaks
        foreach (Control control in Controls)
        {
            control.Dispose();
        }
        Controls.Clear();
    }
}