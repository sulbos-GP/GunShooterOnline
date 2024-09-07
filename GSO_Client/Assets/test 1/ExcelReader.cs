using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExcelReader
{
    private const string EXCEL_PATH = "Data/";

    private static Dictionary<eTABLE_TYPE, Action<string>> _readDicData = new Dictionary<eTABLE_TYPE, Action<string>>()
    {
        {eTABLE_TYPE.TestItem, TableExcel.ReadCSV<Data_TestItem>},
        {eTABLE_TYPE.Item, TableExcel.ReadCSV<Data_Item> }
    };

    public static void ReadExcel()
    {
        // ±‚∫ªµ•¿Ã≈Õ ø¢ºø ∑ŒµÂ.
        string[] fileList = Directory.GetFiles(EXCEL_PATH, "*.xlsx", SearchOption.TopDirectoryOnly);
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
                return;
            }
        }
    }

    private static List<ISheet> GetSheet(string[] files)
    {
        List<ISheet> list = new List<ISheet>();
        IWorkbook workbook;
        foreach (string fileName in files)
        {
            if (fileName.IndexOf('~') >= 0)
                continue;

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

                // Ω∞«•, Ω÷µ˚ø»«•, ∞≥«‡¿Ã ¡∏¿Á«œ∏È øπø‹√≥∏Æ.
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
