using CI.QuickSave;
using System.Collections.Generic;

public static class Statistics
{
    public static void UpdatePurchasedSeeds(string seedName)
    {
        UpdateSeedInformation("PurchasedSeeds", seedName);
    }

    public static void UpdateSoldSeeds(string seedName)
    {
        UpdateSeedInformation("SelledSeeds", seedName);
    }

    public static void UpdateGrowedSeeds(string seedName)
    {
        UpdateSeedInformation("GrowedSeeds", seedName);
    }

    public static void UpdateCrossedSeeds(string seedName)
    {
        UpdateSeedInformation("CrossedSeeds", seedName);
    }

    /// <summary>
    /// Увеличивает счётчик указанного семечка на 1
    /// </summary>
    /// <param name="key">Название категории сохранения</param>
    /// <param name="seedName">Имя семечка</param>
    private static void UpdateSeedInformation(string key, string seedName)
    {
        var seedsInfo = new Dictionary<string, int>();
        var reader = QSReader.Create("Statistics");
        var writer = QuickSaveWriter.Create("Statistics");

        if (reader.Exists(key))
            seedsInfo = reader.Read<Dictionary<string, int>>(key);

        if (!seedsInfo.ContainsKey(seedName))
            seedsInfo[seedName] = 0;
        seedsInfo[seedName]++;

        writer.Write(key, seedsInfo);
        writer.Commit();
    }
}
