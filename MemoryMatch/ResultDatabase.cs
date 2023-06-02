using System;
using System.Collections.Generic;
using SQLite;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MemoryMatch
{
    public class ResultDatabase
    {
        private SQLiteAsyncConnection _database;

        public ResultDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Result>().Wait();
        }

        public Task<List<Result>> GetResultsAsync()
        {
            return _database.Table<Result>().ToListAsync();
        }

        public Task<int> SaveResultAsync(Result result)
        {
            if (result.Id != 0)
            {
                return _database.UpdateAsync(result);
            }
            else
            {
                return _database.InsertAsync(result);
            }
        }

        public Task<int> DeleteResultAsync(Result result)
        {
            return _database.DeleteAsync(result);
        }
        public async Task DeleteLastResultAsync(string playerName)
        {
            var lastResult = await _database.Table<Result>()
                                           .Where(r => r.PlayerName == playerName)
                                           .OrderByDescending(r => r.Id)
                                           .FirstOrDefaultAsync();

            if (lastResult != null)
            {
                await _database.DeleteAsync(lastResult);
            }
        }
        public Task<Result> GetResultAsync(string playerName)
        {
            return _database.Table<Result>()
                .Where(result => result.PlayerName == playerName)
                .FirstOrDefaultAsync();
        }
        public async Task DeleteResultByNameAsync(string playerName)
        {
            var connection = await GetConnectionAsync();
            var result = await connection.Table<Result>().FirstOrDefaultAsync(r => r.PlayerName == playerName);
            if (result != null)
            {
                await connection.DeleteAsync(result);
            }
        }
        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");
            var connection = new SQLiteAsyncConnection(dbPath);
            await connection.CreateTableAsync<Result>();
            return connection;
        }
    }
}