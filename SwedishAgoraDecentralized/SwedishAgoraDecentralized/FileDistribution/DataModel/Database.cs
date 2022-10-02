using MySql.Data.MySqlClient;

namespace SwedishAgoraDecentralized.FileDistribution.DataModel
{
    public interface IHasInsertMetadata
    {
        IEnumerable<KeyValuePair<string, object>> FieldParameters { get; }
        string TableName { get; }
    }

    public sealed class Database
    {
        private readonly string _connectionString;
        private readonly string _backupFilePath;

        public Database(string connectionString, string backupFilePath)
        {
            _connectionString = connectionString;
            _backupFilePath = backupFilePath;
        }

        public async Task<bool> InsertObjectAsync<T>(T obj) where T : class, IHasInsertMetadata
        {
            string sqlQuery = $"INSERT INTO {obj.TableName} VALUES ({string.Join(",", obj.FieldParameters.Select(f => f.Key))});";

            using MySqlConnection connection = OpenNewConnection();
            using MySqlCommand cmd = new(sqlQuery) { Connection = connection };

            foreach (var val in obj.FieldParameters)
                if (val.Key != "null")
                    cmd.Parameters.Add(new MySqlParameter(val.Key, val.Value ?? DBNull.Value));

            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public async Task BackupAsync() => await Task.Run(() => PerformBackupOperation(mb => mb.ExportToFile(_backupFilePath)));
        public async Task RestoreAsync()=> await Task.Run(() => PerformBackupOperation(mb => mb.ImportFromFile(_backupFilePath)));

        private void PerformBackupOperation(Action<MySqlBackup> operation)
        {
            using MySqlConnection connection = OpenNewConnection();
            using MySqlCommand cmd = new() { Connection = connection };
            using MySqlBackup mb = new(cmd);

            operation(mb);
        }
        private MySqlConnection OpenNewConnection()
        {
            MySqlConnection connection = new(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
