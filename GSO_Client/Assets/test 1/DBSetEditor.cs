using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using MySqlConnector;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLiteConnection;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Text;
using System.Linq;

public class DBSetEditor : EditorWindow
{
    private string server = "localhost";
    private string database = "master_database";
    private string userId = "root";
    private string password = "!Q2w3e4r";

    private MySqlConnection connection;

    private string mysqlPath = "/StreamingAssets/";
    private string dataScriptPath = "Assets/Scripts/Data/Data_Master.cs";
    private string dataTablePath = "Assets/test 1/ExcelReader.cs";

    // 체크박스 상태를 저장할 bool 변수
    private bool IsMulti = false;

    private string dbScriptPath;

    [MenuItem("Tools/DbSet")]
    public static void ShowWindow()
    {
        GetWindow<DBSetEditor>("DB Set Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("DB기반 엑셀 파일 갱신", EditorStyles.boldLabel);

        // 버튼을 생성하고 클릭 이벤트 처리
        if (GUILayout.Button("LoadDB"))
        {
            LoadDB();
        }

        GUILayout.Label("Data 구조 생성", EditorStyles.boldLabel);

        if(GUILayout.Button("Create Data"))
        {
            CreateData();
        }
    }

    private void LoadDB()
    {
        try
        {
            string connString = $"Server={server};Database={database};User ID={userId};Password={password};Pooling=false;";
            connection = new MySqlConnection(connString);
            connection.Open();
            Debug.Log("MySQL 데이터베이스에 성공적으로 연결되었습니다.");

            loadData();

            DisConnect();
        }
        catch (Exception ex)
        {
            Debug.LogError("MySQL 데이터베이스 연결에 실패했습니다: " + ex.Message);
        }
    }

    public void loadData()
    {
        List<string> tables = GetTableName();
        // MySQL에서 데이터를 가져와 엑셀에 저장
        foreach (var table in tables)
            ExportDataToExcel(table);
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
        string query = "SELECT * FROM " + table; // your_table_name을 실제 테이블 이름으로 변경
        MySqlCommand command = new MySqlCommand(query, connection);
        MySqlDataReader reader = command.ExecuteReader();

        // DataTable 생성 및 데이터 로드
        DataTable dataTable = new DataTable();
        dataTable.Load(reader);
        reader.Close();

        // 엑셀 파일 생성 또는 업데이트
        string filePath = System.IO.Path.Combine(Application.dataPath + mysqlPath, table + ".xlsx");

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

    public void Connect()
    {
        string connString = $"Server={server};Database={database};User ID={userId};Password={password};Pooling=false;";
        connection = new MySqlConnection(connString);
        connection.Open();
        Debug.Log("MySQL 데이터베이스에 성공적으로 연결되었습니다.");
    }

    public void DisConnect()
    {
        if (connection != null)
        {
            connection.Close();
            Debug.Log("MySQL 데이터베이스 연결이 닫혔습니다.");
        }
    }

    public void CreateData()
    {
        Connect();
        List<string> tables = GetTableName();
        List<string> dataName = GetClassNamesFromSingleFile();
        for(int i=0;i<tables.Count;i++)
        {
            if (!dataName.Contains(tables[i]))
            {
                var columns = GetTableColumns(connection, tables[i]);
                SaveClassToFile(tables[i], GenerateClassCode(tables[i], columns));
                InsertTableScript(tables[i]);
            }
        }
        DisConnect();
    }

    public void InsertTableScript(string tableName)
    {
        var lines = File.ReadAllLines(dataTablePath).ToList();

        // "_readDicData" 딕셔너리 부분의 끝을 찾음
        int index = lines.FindIndex(line => line.Contains("};"));

        if (index != -1)
        {
            // "};" 전에 새로운 eTABLE_TYPE 요소를 추가
            lines[index-1] = lines[index-1].TrimEnd()+ $",\n\t\t{{eTABLE_TYPE.{tableName}, TableExcel.ReadCSV<Data_{tableName}> }}";

            // 파일에 다시 저장
            File.WriteAllLines(dataTablePath, lines);
            Console.WriteLine("New element added successfully.");
        }
        else
        {
            Console.WriteLine("Dictionary not found.");
        }
    }

    public List<string> GetClassNamesFromSingleFile()
    {
        List<string> classNames = new List<string>();

        // 파일 내용 읽기
        if (File.Exists(dataScriptPath))
        {
            string content = File.ReadAllText(dataScriptPath);

            // 정규식으로 모든 클래스 선언 찾기
            MatchCollection matches = Regex.Matches(content, @"class\s+(\w+)");

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    // 여러 클래스 이름을 리스트에 추가
                    classNames.Add(match.Groups[1].Value.Replace("Data_", ""));
                }
            }
        }
        else
        {
            Debug.Log($"File not found: {dataScriptPath}");
        }

        return classNames;
    }

    private List<ColumInfo> GetTableColumns(MySqlConnection connection, string tableName)
    {
        var columns = new List<ColumInfo>();

        // MySQL의 INFORMATION_SCHEMA에서 테이블 메타데이터 가져오기
        string query = $@"
        SELECT COLUMN_NAME, DATA_TYPE 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = '{tableName}' 
        AND TABLE_SCHEMA = '{database}'
        ORDER BY ORDINAL_POSITION;"; 

        using (var cmd = new MySqlCommand(query, connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                bool firstCulmn = true;
                while (reader.Read())
                {
                    if(firstCulmn)
                    {
                        firstCulmn = false;
                        continue;
                    }
                    var column = new ColumInfo
                    {
                        Name = reader.GetString(0), // 컬럼명
                        DataType = reader.GetString(1) // 컬럼의 데이터 타입
                    };
                    columns.Add(column);
                }
            }
        }
        return columns;
    }

    // 데이터베이스 타입을 C# 타입으로 변환하는 함수
    private string GetCSharpType(string dbType)
    {
        switch (dbType.ToLower())
        {
            case "int":
            case "integer":
                return "int";  // 'int'형 데이터를 C#의 'int'로 변환
            case "bigint":
                return "long";  // 'bigint'는 'long'으로 변환
            case "float":
                return "float";  // 'float'형 데이터를 C#의 'float'로 변환
            case "double":
            case "real":
                return "double";
            case "char":  // 'char'형 데이터를 C#의 'char'로 변환
                return "char";
            case "varchar":
            case "text":
                return "string";
            case "blob":
                return "byte[]";
            case "boolean":
            case "bool":
                return "bool";
            case "datetime":
            case "timestamp":
                return "DateTime";
            default:
                return "string"; // 기본적으로 문자열로 처리
        }
    }

    private string GenerateClassCode(string tableName, List<ColumInfo> columns)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"public class Data_{tableName} : BaseData<Data_{tableName}>");
        sb.AppendLine("{");

        foreach (var column in columns)
        {
            var csharpType = GetCSharpType(column.DataType);
            sb.AppendLine($"    public {csharpType} {column.Name} {{ get; set; }}"); // 동적 컬럼에 맞춘 속성 생성
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private void SaveClassToFile(string tableName, string classCode)
    {

        var filePath = dataScriptPath; // 모든 클래스를 하나의 파일에 추가
        File.AppendAllText(filePath, classCode);
        // 파일이 존재하면 클래스 코드를 추가하고, 없으면 새로 생성

        Console.WriteLine($"Class for {tableName} added to {filePath}");
    }


    private void SetDB()
    {
        //dbScriptPath = "Assets/ExistingScript.cs";
        //if (File.Exists(dbScriptPath))
        //{
        //    string scriptContent = File.ReadAllText(dbScriptPath);
        //}
    }
}

public class ColumInfo
{
    public string Name { get; set; }
    public string DataType { get; set; }
}
