namespace SKYNET;

/// <summary>
/// Servicio para manejar el estado visual de los BundleBox "Current*" en la UI.
/// Encapsula el acceso a los controles de frmMain.
/// </summary>
public interface IUIState
{
    // ════════════════════════════════════════
    // ASIGNAR ITEMS A LOS BUNDLE BOXES
    // ════════════════════════════════════════
    void SetTerrainItem(Items item);
    void SetWeatherEffectItem(Items item);
    void SetRadiantTowersItem(Items item);
    void SetDireTowersItem(Items item);
    void SetRadiantCreepsItem(Items item);
    void SetDireCreepsItem(Items item);
    void SetVersusScreenItem(Items item);
    void SetCursorPackItem(Items item);
    void SetMultikillBannerItem(Items item);
    void SetEmblemItem(Items item);
    void SetEmoticonsItem(Items item);
    void SetHUDSkinItem(Items item);
    void SetCourierItem(Items item);
    void SetWardItem(Items item);
    void SetLoadingScreenItem(Items item);
    void SetMusicPackItem(Items item);
    void SetAnnouncerItem(Items item);
    void SetMegaKillItem(Items item);
    void SetPetsItem(Items item);
    void SetStreakEffectItem(Items item);

    // ════════════════════════════════════════
    // RESET TODOS LOS BUNDLE BOXES
    // ════════════════════════════════════════
    /// <summary>
    /// Resetea visualmente el BundleBox de una categoría específica.
    /// </summary>
    void ResetCategory(ItemCategory category);
    /// <summary>
    /// Carga el item "default" de una categoría en el BundleBox correspondiente.
    /// Útil después de un reset para volver al estado inicial visible.
    /// </summary>
    void LoadDefaultForCategory(ItemCategory category);
    void ResetAll();
}