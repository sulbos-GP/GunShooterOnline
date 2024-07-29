using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CSVReader : MonoBehaviour
{
    public string csvFilePath;

    void Start()
    {
        ExcelReader.ReadExcel();
        Debug.Log("success Read");
        Debug.Log(Data_TestItem.GetData(0));
        //List<Data_TestItem> playerDataList = ReadCSVFile(csvFilePath);
        //foreach (Data_TestItem player in playerDataList)
        //{
        //    Debug.Log("Name: " + player.index + ", Age: " + player.ItemName + ", Score: " + player.Oper);
        //}
    }

    List<Data_TestItem> ReadCSVFile(string filePath)
    {   
        List<Data_TestItem> playerDataList = new List<Data_TestItem>();

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

                    Data_TestItem playerData = new Data_TestItem();
                    playerDataList.Add(playerData);
                }
            }
        }

        return playerDataList;
    }
}
