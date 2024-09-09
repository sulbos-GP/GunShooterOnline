using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class MySQLToExcel : MonoBehaviour
{
    private string server = "localhost";
    private string database = "master_database";
    private string userId = "root";
    private string password = "!Q2w3e4r";
    private string feachDatabase = "data_version";


    private MySqlConnection connection;

    private string mysqlPath = "/StreamingAssets/";

    void Start()
    {
        try
        {
            string connString = $"Server={server};Database={database};User ID={userId};Password={password};Pooling=false;";
            connection = new MySqlConnection(connString);
            connection.Open();
            Debug.Log("MySQL 데이터베이스에 성공적으로 연결되었습니다.");

            List<string> tables = GetTableName();
            // MySQL에서 데이터를 가져와 엑셀에 저장
            foreach(var table in tables)
                ExportDataToExcel(table);
        }
        catch (Exception ex)
        {
            Debug.LogError("MySQL 데이터베이스 연결에 실패했습니다: " + ex.Message);
        }
    }

    private List<string> GetTableName()
    {
        List<string> tableNames = new List<string>();

        using (MySqlCommand cmd = new MySqlCommand("SHOW TABLES;", connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(0).Contains("version"))
                        continue;
                    // 테이블 이름을 리스트에 추가
                    tableNames.Add(reader.GetString(0));
                }
            }
        }

        return tableNames;
    }

    void ExportDataToExcel(string table)
    {
        // MySQL 쿼리 실행
        string query = "SELECT * FROM "+ table; // your_table_name을 실제 테이블 이름으로 변경
        MySqlCommand command = new MySqlCommand(query, connection);
        MySqlDataReader reader = command.ExecuteReader();

        // DataTable 생성 및 데이터 로드
        DataTable dataTable = new DataTable();
        dataTable.Load(reader);
        reader.Close();

        // 엑셀 파일 생성 또는 업데이트
        string filePath = Path.Combine(Application.dataPath+ mysqlPath, table+".xlsx");

        // 엑셀 워크북 및 시트 생성
        IWorkbook workbook;
        ISheet sheet;

        if (File.Exists(filePath))
        {
            // 기존 파일이 있으면 파일 열기
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
                sheet = workbook.GetSheet(table) ?? workbook.CreateSheet(table);
            }
        }
        else
        {
            // 새 파일 생성
            workbook = new XSSFWorkbook();
            sheet = workbook.CreateSheet(table);
        }

        // DataTable 데이터를 엑셀 시트에 쓰기
        IRow headerRow = sheet.CreateRow(0);
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            headerRow.CreateCell(i).SetCellValue(dataTable.Columns[i].ColumnName);
        }

        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            IRow row = sheet.CreateRow(i + 1);
            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                row.CreateCell(j).SetCellValue(dataTable.Rows[i][j].ToString());
            }
        }

        // 엑셀 파일 저장
        using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(file);
        }

        Debug.Log("엑셀 파일이 생성되었습니다: " + filePath);
    }

    void OnApplicationQuit()
    {
        if (connection != null)
        {
            connection.Close();
            Debug.Log("MySQL 데이터베이스 연결이 닫혔습니다.");
        }
    }
}
