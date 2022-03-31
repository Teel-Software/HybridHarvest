using System.Collections.Generic;
using System;
using System.Text;
using Tools;
using System.IO;
using UnityEngine;

public static class CSVStatsMerger
{
    /// <summary>
    /// Checs if this seedType exists (if not, creates it) and returns statistics
    /// </summary>
    public static SeedStatistics GetQuantumStatistics(string parent1, string parent2)
    {
        var seedName = parent1 + "-" + parent2;
        var stats = CSVReader.ParseSeedStats(seedName);
        if (stats == null)
        {
            var newRows = MergeExistingTables(parent1, parent2);
            CreateNewCSV(seedName, newRows);
        }

        stats = CSVReader.ParseSeedStats(seedName);
        return stats;
    }

    /// <summary>
    /// Merges values in existing tables according to plan
    /// </summary>
    /// <param name="parent1">name of first table`s seed</param>
    /// <param name="parent2">name of second table`s seed</param>
    /// <returns>List of rows for new table</returns>
    private static List<string> MergeExistingTables(string parent1, string parent2)
    {
        List<string> newRows = new List<string>();
        newRows.Add("�������,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20");
        var table1 = CSVReader.GetRawData(parent1);
        var table2 = CSVReader.GetRawData(parent2);
        foreach (var key in table1.Keys)
        {
            var newRow = new StringBuilder();
            switch (key)
            {
                case var str when str.Contains("�������"):
                    newRow.Append(key);
                    for (int i = 0; i < 20; i++)
                    {
                        newRow.Append(",");
                        newRow.Append((table1[key][i] + table2[key][i]).ToString());
                    }
                    newRows.Add(newRow.ToString());
                    break;
                case var str when str.Contains("����"):
                    newRow.Append(key);
                    for (int i = 0; i < 20; i++)
                    {
                        newRow.Append(",");
                        newRow.Append((table1[key][i] + table2[key][i]).ToString());
                    }
                    newRows.Add(newRow.ToString());
                    break;
                case var str when (str.Contains("���") && !str.Contains("min")):
                    newRow.Append(key);
                    for (int i = 0; i < 20; i++)
                    {
                        newRow.Append(",");
                        newRow.Append(((table1[key + "min"][i] + table2[key + "min"][i]) / 2).ToString());
                        newRow.Append("-");
                        newRow.Append(((table1[key][i] + table2[key][i]) / 2).ToString());
                    }
                    newRows.Add(newRow.ToString());
                    break;
                case var str when str.Contains("�����"):
                    newRow.Append(key);
                    for (int i = 0; i < 20; i++)
                    {
                        newRow.Append(",");
                        int chanse = table1[key][i] + table2[key][i];
                        switch (chanse)
                        {
                            case 0:
                                newRow.Append("������");
                                break;
                            case 1:
                                newRow.Append("�������");
                                break;
                            case 2:
                                newRow.Append("�������");
                                break;
                            default:
                                newRow.Append("����� �������");
                                break;
                        }
                    }
                    newRows.Add(newRow.ToString());
                    break;
                case "�����":
                    newRow.Append(key);
                    for (int i = 0; i < 20; i++)
                    {
                        newRow.Append(",");
                        newRow.Append(TimeFormatter.FormatTotableView(table1[key][i] + table2[key][i]));
                    }
                    newRows.Add(newRow.ToString());
                    break;
            }
        }
        return newRows;
    }

    /// <summary>
    /// Creates new table in app datapath
    /// </summary>
    /// <param name="seedName">table`s seed name</param>
    /// <param name="newRows">string rows for the table</param>
    private static void CreateNewCSV(string seedName, List<string> newRows)
    {
        var folder = Application.persistentDataPath;
        var filePath = Path.Combine(folder, seedName + ".csv");
        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(String.Join("\n", newRows));
        }
    }
}