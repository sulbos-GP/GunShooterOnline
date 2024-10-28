
using DatabaseGenerator;
using ItemGenerator;
using MySqlConnector;
using Server.Server;
using System.Reflection;
using WebCommonLibrary.Models.MasterDatabase;
using WebCommonLibrary.Reposiotry;
using WebCommonLibrary.Reposiotry.MasterDatabase;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program
{
    private static readonly string _connectionString = "Server=127.0.0.1;user=root;Password=!Q2w3e4r;Database=master_database;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;";

    public DbTable<FMasterItemBackpack> MasterItemBackpack { get; }
    public DbTable<FMasterItemBase> MasterItemBase { get; }
    public static DbTable<FMasterItemUse> MasterItemUse { get; private set; }


    private static string cHandlerRegister = "";
    private static string cDelclarationRegister = "";
    private static string mManagerRegister = "";

    private static void Main(string[] args)
    {
        Init();

        
    }

    private static void Init()
    {
        MasterItemUse = LoadMasterItemUse().Result;

        foreach (var item in MasterItemUse)
        {
            cHandlerRegister += "\t\t"+ string.Format(Format.ItemCoolRegister, item.Key) + Environment.NewLine;
            cDelclarationRegister += "\t" + string.Format(Format.ItemCoolDeclaration, item.Key) + Environment.NewLine;
            mManagerRegister += "\t\t" + string.Format(Format.ManagerHandlerRegister, item.Key) + Environment.NewLine;


        }

        var ItemCool = string.Format(ItemCoolDownText.context, cHandlerRegister, cDelclarationRegister);
        var ItemManager = string.Format(ItemManagerText.context, mManagerRegister);
        File.WriteAllText("../../../../Server/Game/ItemManager/ItemCoolDown.cs", ItemCool);
        File.WriteAllText("../../../../Server/Game/ItemManager/ItemManager.cs", ItemManager);
    }




    private static async Task<DbTable<FMasterItemUse>> LoadMasterItemUse()
    {
        string query = "SELECT item_id, energy, active_time, duration, effect, cool_time FROM master_item_use;";
        return await LoadDatabaseTable<FMasterItemUse>(_connectionString, query);
    }


    protected static async Task<DbTable<T>> LoadDatabaseTable<T>(string connectionString, string query) where T : class, new()
    {
        try
        {
            Dictionary<int, T> datas = new Dictionary<int, T>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            T data = new T();
                            foreach (var prop in properties)
                            {
                                if (reader.GetOrdinal(prop.Name) < 0)
                                {
                                    throw new Exception("테이블이 일치하지 않습니다, 데이터베이스를 업데이트 해주세요");
                                }

                                if (reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                {
                                    continue;
                                }

                                if (prop.PropertyType.BaseType!.Name == "Enum")
                                {
                                    prop.SetValue(data, System.Enum.Parse(prop.PropertyType, reader[prop.Name].ToString()!));
                                }
                                else
                                {
                                    prop.SetValue(data, reader[prop.Name]);
                                }
                            }
                            datas.Add(reader.GetInt32(0), data);
                        }
                    }
                }
                await conn.CloseAsync();
            }

            return new DbTable<T>(datas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbContext.LoadDatabaseTable] : {ex.Message}");
            return new DbTable<T>(null);
        }
    }
}


