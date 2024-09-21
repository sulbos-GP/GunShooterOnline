using UnityEngine;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.Formula.Functions;
using static UnityEditor.Progress;

public class CSVReader : MonoBehaviour
{
    public string csvFilePath;

    void Awake()
    {

#if UNITY_ANDROID
        BetterStreamingAssets.Initialize();
        string[] files = BetterStreamingAssets.GetFiles("/", "*.xlsx", SearchOption.AllDirectories);
        ExcelReader.CopyExcel(files);
#endif
        Debug.Log("success Read");
        Debug.Log(Data_master_item_base.GetData(101).name);
        Debug.Log(Data_master_item_base.AllData());
        //List<Data_TestItem> playerDataList = ReadCSVFile(csvFilePath);
        //foreach (Data_TestItem player in playerDataList)
        //{
        //    Debug.Log("Name: " + player.index + ", Age: " + player.ItemName + ", Score: " + player.Oper);
        //}
    }

    List<T> ReadCSVFile(string filePath)
    {   
        List<T> playerDataList = new List<T>();

        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV file not found: " + filePath);
            return playerDataList;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            bool isHeader = true;
            bool isVariable = true;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (isHeader)
                {
                    isHeader = false; // Skip the header line
                    continue;
                }
                if(isVariable)
                {
                    isVariable = false;
                    continue;
                }
                string[] values = line.Split(',');
                if (values.Length == 3)
                {
                    int var1 = int.Parse(values[0]);
                    string var2 = values[1];
                    string var3 = values[2];

                    T playerData = new T();
                    playerDataList.Add(playerData);
                }
            }
        }

        return playerDataList;
    }
}
