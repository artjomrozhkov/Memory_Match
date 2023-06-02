using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryMatch
{
    public class Result
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int WrongAttempts { get; set; }
        public int Place { get; set; }
    }
}
