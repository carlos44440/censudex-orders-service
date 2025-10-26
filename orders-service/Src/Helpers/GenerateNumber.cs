namespace orders_service.Src.Helpers
{
    public static class GenerateNumber
    {
        public static string GenerateTrackingNumber()
        {
            var random = new Random();
            return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{random.Next(100000, 999999)}";
        }
    }
}