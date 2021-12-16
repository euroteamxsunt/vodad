using System.Data.Objects;
using System.Text.RegularExpressions;

namespace VodadModel.Utilities
{
    public static class Extensions
    {
        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            string sql = context.CreateObjectSet<T>().ToTraceString();
            Regex regex = new Regex(@"FROM (?<table>.*) AS");
            Match match = regex.Match(sql);
            string table = string.Empty;
            var split = match.Groups["table"].Value.Split('.');
            if (split.Length > 1)
                table = split[1].Trim('[').Trim(']');
            else
                table = string.Join("", split);
            return table;
        }
    }
}
