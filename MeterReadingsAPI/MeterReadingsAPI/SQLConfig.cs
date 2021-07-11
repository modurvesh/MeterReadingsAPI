using CsvHelper;
using MeterReadingsAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadingsAPI
{
    public class SQLConfig
    {
        private static string _connectionString = null;
        // DB recreated on startup, set to true to keep current db
        public static bool TempDBCreated = false;

        public static string connectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public static void PopulateUserAccounts() {
            // Add each test account to the db table upon startup
            using (var reader = new StreamReader("Test_Accounts.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var accounts = csv.GetRecords<UserAccount>();

                foreach (UserAccount account in accounts) {
                    using (MeterReadingsContext ctx = new MeterReadingsContext())
                    {
                        ctx.UserAccounts.Add(account);
                        ctx.SaveChanges();
                    }
                }
            }
        }
    }
}
