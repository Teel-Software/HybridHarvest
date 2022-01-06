using System;
using System.Collections.Generic;
using System.Text;
public class SeedStatistics
{
    public Dictionary<int, int> Gabitus { get; set; }
    public Dictionary<int, int> Taste { get; set; }
    public Dictionary<int, int> MinAmount { get; set; }
    public Dictionary<int, int> MaxAmount { get; set; }
    public Dictionary<MutationChance, int> MutationChance { get; set; }
    public Dictionary<int, int> GrowTime { get; set; }

    public SeedStatistics()
    {
        Gabitus = new Dictionary<int, int>();
        Taste = new Dictionary<int, int>();
        MinAmount = new Dictionary<int, int>();
        MaxAmount = new Dictionary<int, int>();
        MutationChance = new Dictionary<MutationChance, int>();
        GrowTime = new Dictionary<int, int>();
    }
    public override string ToString()
    {
        StringBuilder encodedData = new StringBuilder();
        encodedData.Append(string.Join(" ", Gabitus))
            .Append("/")
            .Append(string.Join(" ", Taste))
            .Append("/")
            .Append(string.Join(" ", MinAmount))
            .Append("/")
            .Append(string.Join(" ", MaxAmount))
            .Append("/")
            .Append(string.Join(" ", MutationChance))
            .Append("/")
            .Append(string.Join(" ", GrowTime));
        return encodedData.ToString();
    }
}
