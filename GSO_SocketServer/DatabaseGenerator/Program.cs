using System.Text.Json;

namespace DatabaseGenerator
{
    public class Context
    {
        public string name { get; set; } = "";
        public string table { get; set; } = "";
    }

    internal class Program
    {

        private static string ContextEnumRegister = "";
        private static string ContextRegionRegister = "";
        private static string GetContextRegister = "";
        private static string LoadDatabaseContextRegister = "";
        private static string GetDataContextRegister = "";

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

                ContextEnumRegister += string.Format(Format.ContextEnum, context.table, uname);
                ContextRegionRegister += string.Format(Format.ContextRegion, data, context.name);
                GetContextRegister += string.Format(Format.GetContext, data, uname, context.name);
                LoadDatabaseContextRegister += string.Format(Format.LoadDatabaseContext, uname, data, context.table);
                GetDataContextRegister += string.Format(Format.GetDataContext, data, uname);
            }

            var text = string.Format(Format.context, ContextEnumRegister, ContextRegionRegister, GetContextRegister, LoadDatabaseContextRegister, GetDataContextRegister);
            File.WriteAllText("DatabaseContext.cs", text);
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