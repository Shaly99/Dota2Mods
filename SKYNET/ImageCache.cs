namespace SKYNET;

/// <summary>
/// Cache de imágenes para evitar leer las mismas imágenes del disco múltiples veces.
/// Las imágenes se cargan UNA VEZ y se reutilizan.
/// </summary>
public static class ImageCache
{
    private static readonly Dictionary<string, Image> _cache = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Obtiene una imagen del cache, o la carga si no existe.
    /// IMPORTANTE: NO dispongas la imagen retornada, el cache la mantiene viva.
    /// </summary>
    public static Image GetOrLoad(string filePath, Image fallback = null)
    {
        if (string.IsNullOrEmpty(filePath))
            return fallback;

        // Normalizamos la ruta para evitar duplicados
        string key = Path.GetFullPath(filePath).ToLowerInvariant();

        lock (_lock)
        {
            // Si ya está en cache, devolverla
            if (_cache.TryGetValue(key, out Image cached))
            {
                return cached;
            }

            // Si el archivo no existe, devolver fallback
            if (!File.Exists(filePath))
            {
                return fallback;
            }

            try
            {
                // Cargamos la imagen SIN bloquear el archivo
                // (leemos los bytes y creamos la imagen desde memoria)
                byte[] bytes = File.ReadAllBytes(filePath);
                var stream = new MemoryStream(bytes);
                var image = Image.FromStream(stream);

                _cache[key] = image;
                return image;
            }
            catch
            {
                return fallback;
            }
        }
    }

    /// <summary>
    /// Agrega una imagen al cache manualmente (ej: imágenes generadas dinámicamente).
    /// </summary>
    public static void Add(string key, Image image)
    {
        if (string.IsNullOrEmpty(key) || image == null)
            return;

        string normalizedKey = key.ToLowerInvariant();

        lock (_lock)
        {
            // Si ya existía algo, lo disponemos antes de reemplazar
            if (_cache.TryGetValue(normalizedKey, out Image existing))
            {
                existing.Dispose();
            }
            _cache[normalizedKey] = image;
        }
    }

    /// <summary>
    /// Limpia todo el cache y libera memoria.
    /// Usar al cerrar la app o cambiar de héroe si querés liberar RAM.
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            foreach (var image in _cache.Values)
            {
                try { image?.Dispose(); } catch { }
            }
            _cache.Clear();
        }
    }

    /// <summary>
    /// Cantidad de imágenes en cache (útil para debug).
    /// </summary>
    public static int Count
    {
        get { lock (_lock) { return _cache.Count; } }
    }
}