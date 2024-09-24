using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class ExcelReader
{
    private const string EXCEL_PATH = "/StreamingAssets/";

    private static Dictionary<eTABLE_TYPE, Action<string>> _readDicData = new Dictionary<eTABLE_TYPE, Action<string>>()
    {
        {eTABLE_TYPE.TestItem, TableExcel.ReadCSV<Data_TestItem>},
        {eTABLE_TYPE.master_item_base, TableExcel.ReadCSV<Data_master_item_base> },
        {eTABLE_TYPE.master_item_backpack, TableExcel.ReadCSV<Data_master_item_backpack>},
        {eTABLE_TYPE.master_item_use, TableExcel.ReadCSV<Data_master_item_use>},
        {eTABLE_TYPE.master_item_weapon, TableExcel.ReadCSV<Data_master_item_weapon>},

        {eTABLE_TYPE.master_reward_base, TableExcel.ReadCSV<Data_master_reward_base> },
        {eTABLE_TYPE.master_reward_level, TableExcel.ReadCSV<Data_master_reward_level> },
        {eTABLE_TYPE.master_reward_box, TableExcel.ReadCSV<Data_master_reward_box> },
        {eTABLE_TYPE.master_reward_box_item, TableExcel.ReadCSV<Data_master_reward_box_item> }
    };

    public static void CopyExcel(string[] files)
    {
        if (!Application.isEditor)
        {
            foreach (var file in files)
            {
                //TO-DO : 추후에 텍스트 파일로 엑셀 이름 관리.
                string filePath = Path.Combine(Application.streamingAssetsPath, file);
                string filePerPath = Path.Combine(Application.persistentDataPath, file);

                byte[] fileData = BetterStreamingAssets.ReadAllBytes(file);

                string destinationPath = Path.Combine(Application.persistentDataPath, file);

                string directoryPath = Path.GetDirectoryName(destinationPath);

                File.WriteAllBytes(destinationPath, fileData);
                Debug.Log($"Copied file to: {destinationPath} {fileData}");
            }
        }
        ReadExcel();
    }

    public static void ReadExcel()
    {
        //string filePath = Path.Combine(Application.streamingAssetsPath, "Item.xlsx");
        //string filePerPath = Path.Combine(Application.persistentDataPath, "Item.xlsx");
        //UnityWebRequest request = UnityWebRequest.Get(filePath);
        //File.WriteAllBytes(filePerPath, request.downloadHandler.data);
        //Debug.Log("File copied to: " + filePerPath);

        string[] fileList;
#if UNITY_EDITOR
        fileList = Directory.GetFiles(Application.dataPath + EXCEL_PATH, "*.xlsx", SearchOption.TopDirectoryOnly);
        Debug.Log("유니티 에디터");
#elif UNITY_ANDROID
        //string filePerPath = Path.Combine(Application.persistentDataPath, "Item.xlsx");
        fileList =  Directory.GetFiles(Application.persistentDataPath,"*.xlsx", SearchOption.TopDirectoryOnly);
        Debug.Log("유니티 안드로이드");
#endif

        List<ISheet> sheetList = GetSheet(fileList);
        for (int i = 0; i < sheetList.Count; i++)
        {
            string sheetName = sheetList[i].SheetName;

            try
            {
                eTABLE_TYPE tableType;
                if (!Enum.TryParse(sheetName, out tableType))
                    continue;

                if (!_readDicData.ContainsKey(tableType))
                    continue;

                string csvData = SheetToCSV(sheetList[i]);
                _readDicData[tableType](csvData);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Error][ReadExcel][{sheetName}.xlsx][{e.Message}]");
            }
        }
    }

    private static List<ISheet> GetSheet(string[] files)
    {
        List<ISheet> list = new List<ISheet>();
        IWorkbook workbook;
        foreach (string fileName in files)
        {
            //if (fileName.IndexOf('~') >= 0)
            //    continue;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fileName.EndsWith("xls"))
                {
                    workbook = new HSSFWorkbook(fileStream);
                }
                else if (fileName.EndsWith("xlsx"))
                {
                    //#if UNITY_EDITOR_OSX
                    //throw new Exception("xlsx is not supported on OSX.");
                    //#else
                    workbook = new XSSFWorkbook(fileStream);
                    //#endif
                }
                else
                {
                    Debug.Log("파일 안열림");
                    continue;
                }
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);

                    if (sheet.SheetName.StartsWith("_"))
                        continue;

                    list.Add(sheet);
                }
            }
        }

        return list;
    }

    private static string SheetToCSV(ISheet sheet)
    {
        NUtil.sb.Clear();
        int rowCount = sheet.LastRowNum + 1;
        IRow columnRow = sheet.GetRow(0);
        int colCount = columnRow.LastCellNum;
        for (int i = 0; i < rowCount; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null)
                continue;

            ICell firstCell = row.GetCell(0);
            string firstStr = firstCell == null ? string.Empty : firstCell.CellType == CellType.Formula ? firstCell.NumericCellValue.ToString() : firstCell.ToString();
            if (string.IsNullOrEmpty(firstStr))
                continue;

            for (int j = 0; j < colCount; j++)
            {
                ICell columnCell = columnRow.GetCell(j);
                string targetStr;

                if (columnCell != null && columnCell.ToString().StartsWith("_"))
                {
                    if (j + 1 == colCount)
                        NUtil.sb.Append("\n");
                    continue;
                }
                else
                {
                    ICell cell = row.GetCell(j);
                    targetStr = cell == null ? string.Empty : cell.CellType == CellType.Formula ? cell.NumericCellValue.ToString() : cell.ToString();
                }

                // 쉼표, 쌍따옴표, 개행이 존재하면 예외처리.
                bool hasCRLF = targetStr.IndexOf('\n') >= 0 || targetStr.IndexOf('\r') >= 0;
                bool hasDoubleQuotes = targetStr.IndexOf('"') >= 0;
                bool hasComma = targetStr.IndexOf(',') >= 0;

                if (hasDoubleQuotes)
                    targetStr = targetStr.Replace("\"", "\"\"");

                if (hasCRLF || hasDoubleQuotes || hasComma)
                    targetStr = "\"" + targetStr + "\"";

                if (j + 1 == colCount)
                    NUtil.sb.Append(targetStr + "\n");
                else
                    NUtil.sb.Append(targetStr + ",");
            }
        }

        return NUtil.sb.ToString();
    }
}

public class TableExcel
{
    public static void ReadCSV<T>(string strCSV)where T : BaseData<T>,new()
    {
        BaseData<T>.ReadList(CSVParser.LoadData<T>(strCSV));
    }

    public static void ReadCSVMulti<T>(string strCSV)where T : BaseDataMulti<T>, new()
    {
        BaseDataMulti<T>.ReadList(CSVParser.LoadData<T>(strCSV));
    }
}
