namespace orders_service.Src.Helpers
{
    /// <summary>
    /// Clase estática utilizada para generar valores numéricos y códigos únicos.
    /// </summary>
    public static class GenerateNumber
    {
        /// <summary>
        /// Genera un número de seguimiento único para un pedido.
        /// </summary>
        /// <remarks>
        /// El número de seguimiento generado contiene:
        /// <list type="bullet">
        /// <item>Un prefijo fijo <c>TRK</c>.</item>
        /// <item>La fecha actual en formato <c>yyyyMMdd</c>.</item>
        /// <item>Un número aleatorio de seis dígitos.</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Retorna una cadena que representa el número de seguimiento generado.
        /// Ejemplo: <c>TRK-20241222-583920</c>.
        /// </returns>
        public static string GenerateTrackingNumber()
        {
            var random = new Random();
            return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{random.Next(100000, 999999)}";
        }
    }
}
