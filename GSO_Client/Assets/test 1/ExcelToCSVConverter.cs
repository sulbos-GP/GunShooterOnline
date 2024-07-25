using UnityEngine;
using System.Data;
using System.IO;
using ExcelDataReader;

public class ExcelToCSVConverter : MonoBehaviour
{
    public string excelFilePath;
    public string csvOutputPath;

    void Start()
    {
        ConvertExcelToCSV(excelFilePath, csvOutputPath);
    }

    void ConvertExcelToCSV(string excelFilePath, string csvOutputPath)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                foreach (DataTable table in result.Tables)
                {
                    string csvData = DataTableToCSV(table);
                    string csvFilePath = Path.Combine(csvOutputPath, table.TableName + ".csv");
                    File.WriteAllText(csvFilePath, csvData);
                }
            }
        }
    }

    string DataTableToCSV(DataTable table)
    {
        StringWriter writer = new StringWriter();

        for (int i = 0; i < table.Columns.Count; i++)
        {
            writer.Write(table.Columns[i]);
            if (i < table.Columns.Count - 1)
            {
                writer.Write(",");
            }
        }
        writer.WriteLine();

        foreach (DataRow row in table.Rows)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                writer.Write(row[i].ToString());
                if (i < table.Columns.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine();
        }

        return writer.ToString();
    }
}
