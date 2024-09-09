using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace DatabaseGenerator
{
    public class Context
    {
        public string name { get; set; } = "";
        public string table { get; set; } = "";
    }

    internal class Program
    {

        //private static string ContextEnumRegister = "";
        private static string ContextRegionRegister = "";
        private static string GetContextRegister = "";
        private static string LoadDatabaseContextRegister = "";

        private static string GetDataContextRegister = "";
        private static string GetDataFuncContextRegister = "";

        private static string GetDataListContextRegister = "";
        private static string GetDataListFuncContextRegister = "";

        private static string GetValidateContextRegister = "";

        private static void Main(string[] args)
        {
            var json = File.ReadAllText("../../../DatabaseContext.json");
            var contexts = JsonSerializer.Deserialize<List<Context>>(json);

            if (contexts == null)
            {
                return;
            }

            foreach (var context in contexts)
            {
                string uname    = FirstCharToUpper(context.name);
                string data     = $"DB_{uname}";

                //ContextEnumRegister += string.Format(Format.ContextEnum, context.table);

                ContextRegionRegister += string.Format(Format.ContextRegion, data, context.name);
                GetContextRegister += string.Format(Format.GetContext, data, uname, context.name);
                LoadDatabaseContextRegister += string.Format(Format.LoadDatabaseContext, context.name, data, context.table);

                GetDataContextRegister += string.Format(Format.GetDataContext, data, uname);
                GetDataFuncContextRegister += string.Format(Format.GetDataFuncContext, data, uname);

                GetDataListContextRegister += string.Format(Format.GetDataListContext, data, uname);
                GetDataListFuncContextRegister += string.Format(Format.GetDataListFuncContext, data, uname);

                GetValidateContextRegister += string.Format(Format.GetValidateContext, context.name);
            }

            var GameServer = string.Format(GameServerText.context, ContextRegionRegister, GetContextRegister, LoadDatabaseContextRegister, GetDataContextRegister, GetDataListContextRegister);
            File.WriteAllText("DatabaseContext.cs", GameServer);

            var WebServer = string.Format(WebServerText.context, ContextRegionRegister, GetContextRegister, LoadDatabaseContextRegister, GetValidateContextRegister, GetDataContextRegister, GetDataListContextRegister);
            File.WriteAllText("MasterDB_Load.cs", WebServer);

            var WebMaster = string.Format(WebMasterText.context, GetDataFuncContextRegister, GetDataListFuncContextRegister);
            File.WriteAllText("IMasterDataDB.cs", WebMaster);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            string first = input.Substring(0, 1).ToUpper();
            string rest = input.Substring(1);
            return first + rest;
        }

    }
}