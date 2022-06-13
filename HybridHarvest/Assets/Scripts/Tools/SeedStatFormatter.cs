using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public static class SeedStatFormatter
    {
        public static Dictionary<MutationChance, string> MutationTranslations =
            new Dictionary<MutationChance, string> { 
                { MutationChance.Low, "Низкий" },
                { MutationChance.Normal, "Обычный" },
                { MutationChance.High, "Высокий" },
                { MutationChance.Ultra, "Огромный" },
            };

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

        public static string FormatToHarvestMenue(Seed seed, Seed parent)
        {
            var formatedString = new StringBuilder();
            formatedString.Append($"{seed.NameInRussian}\n");

            formatedString.Append($"Вкус: {seed.Taste}");
            if (seed.Taste > parent.Taste)
                formatedString.Append(" <sprite=0>");
            formatedString.Append("\n");

            formatedString.Append($"Габитус: {seed.Gabitus}");
            if (seed.Gabitus > parent.Gabitus)
                formatedString.Append(" <sprite=0>");
            formatedString.Append("\n");

            formatedString.Append($"Время роста: {TimeFormatter.Format(seed.GrowTime)}");
            if (seed.GrowTime < parent.GrowTime)
                formatedString.Append(" <sprite=0>");
            formatedString.Append("\n");

            formatedString.Append($"Кол-во плодов: {seed.MinAmount} - {seed.MaxAmount}");
            if (seed.MinAmount > parent.MinAmount || seed.MaxAmount > parent.MaxAmount)
                formatedString.Append(" <sprite=0>");
            formatedString.Append("\n");

            formatedString.Append($"Шанс мутации: {MutationTranslations[seed.MutationChance]}");
            if (seed.MutationChance > parent.MutationChance)
                formatedString.Append(" <sprite=0>");
            formatedString.Append("\n");

            return formatedString.ToString();
        }

        public static string FormatToHarvestMenue2(Seed seed, Seed parent)
        {
            var formatedString = new StringBuilder();
            formatedString.Append($"{seed.NameInRussian}\n");

            formatedString.Append($"Вкус: ");
            if (seed.Taste > parent.Taste)
                formatedString.Append($"<color=green>{seed.Taste}</color>\n");
            else
                formatedString.Append($"{seed.Taste}\n");

            formatedString.Append($"Габитус: ");
            if (seed.Gabitus > parent.Gabitus)
                formatedString.Append($"<color=green>{seed.Gabitus}</color>\n");
            else
                formatedString.Append($"{seed.Gabitus}\n");

            formatedString.Append($"Время роста: ");
            if (seed.GrowTime < parent.GrowTime)
                formatedString.Append($"<color=green>{TimeFormatter.Format(seed.GrowTime)}</color>\n");
            else
                formatedString.Append($"{TimeFormatter.Format(seed.GrowTime)}\n");

            formatedString.Append($"Кол-во плодов: ");
            if (seed.MinAmount > parent.MinAmount || seed.MaxAmount > parent.MaxAmount)
                formatedString.Append($"<color=green>{seed.MinAmount} - {seed.MaxAmount}</color>\n");
            else
                formatedString.Append($"{seed.MinAmount} - {seed.MaxAmount}\n");

            formatedString.Append($"Шанс мутации: ");
            if (seed.MutationChance > parent.MutationChance)
                formatedString.Append($"<color=green>{MutationTranslations[seed.MutationChance]}</color>\n");
            else
                formatedString.Append($"{MutationTranslations[seed.MutationChance]}\n");

            return formatedString.ToString();
        }
    }
}