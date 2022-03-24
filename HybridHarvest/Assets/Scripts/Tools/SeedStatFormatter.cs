namespace Tools
{
    public static class SeedStatFormatter
    {
        public static string FormatLarge(Seed seed)
        {
            return $"Вкус: {seed.Taste}\n\n" +
                   $"Габитус: {seed.Gabitus}\n\n" +
                   $"Время роста: {TimeFormatter.Format(seed.GrowTime)}\n\n" +
                   $"Цена: {seed.Price}<sprite name=\"Money\">\n";
        }

        public static string FormatSmall(Seed seed)
        {
            return $"Вкус: {seed.Taste}\n" +
                   $"Габитус: {seed.Gabitus}\n" +
                   $"Время роста: {TimeFormatter.Format(seed.GrowTime)}\n" +
                   $"Цена: {seed.Price}<sprite name=\"Money\">";
        }
    }
}