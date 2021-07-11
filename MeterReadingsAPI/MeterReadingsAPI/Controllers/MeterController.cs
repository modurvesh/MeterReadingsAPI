using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MeterReadingsAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Text.RegularExpressions;

namespace TestAppApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeterController : ControllerBase
    {
        [HttpPost("/meter-reading-uploads")]
        public ActionResult<string> UploadReadings(List<IFormFile> files)
        {
            // initialise counter
            int failedReadings = 0;
            int successfulReadings = 0;

            foreach (var file in files)
            {
                // check the file is not empty & the file type is csv
                if (file.Length > 0 && (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/csv" || file.ContentType == "text/csv"))
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            // get the csv records
                            var meterReadings = csv.GetRecords<CsvReading>();

                            // for loop to validate each record and upon success - import into the database
                            foreach (CsvReading reading in meterReadings)
                            {
                                // initialise local variables
                                DateTime dateValue;
                                int numberValue;

                                // validation checks - check valid formats for accountId, date and value
                                if (string.IsNullOrEmpty(reading.AccountId) ||
                                    !DateTime.TryParse(reading.MeterReadingDateTime, out dateValue) ||
                                    !int.TryParse(reading.MeterReadValue, out numberValue) ||
                                    !Regex.IsMatch(reading.MeterReadValue, @"^[1-9]\d{4}$"))
                                {
                                    // upon failure, update the counter and break
                                    failedReadings++;
                                    continue;
                                }

                                // store the csv record in the db table format
                                MeterReading meterReading = new MeterReading()
                                {
                                    AccountId = reading.AccountId,
                                    DateChecked = DateTime.Parse(reading.MeterReadingDateTime),
                                    Value = int.Parse(reading.MeterReadValue)
                                };

                                using (MeterReadingsContext ctx = new MeterReadingsContext())
                                {
                                    // check for if the accountId is associated with an account
                                    bool accountExists = ctx.UserAccounts.Where(a => a.AccountId == meterReading.AccountId).Count() > 0;
                                    // check for if the reading is a duplicate entry
                                    bool readingExists = ctx.MeterReadings.Where(r => r.AccountId == meterReading.AccountId && r.Value == meterReading.Value).Count() > 0;

                                    if (!accountExists || readingExists)
                                    {
                                        failedReadings++;
                                        continue;
                                    }

                                    // if validation checks pass, add the reading to the db table
                                    ctx.MeterReadings.Add(meterReading);
                                    ctx.SaveChanges();
                                    successfulReadings++;
                                }
                            }
                        }
                    }
                }
                else
                    return "Incorrect file format. Please try again.";
            }

            return "Successful readings: " + successfulReadings + ", Failed readings: " + failedReadings;
        }

        [HttpGet("/meter-readings")]
        public ActionResult<List<MeterReading>> GetAllReadings()
        {
            using (MeterReadingsContext ctx = new MeterReadingsContext())
            {
                return ctx.MeterReadings.ToList();
            }
        }

        [HttpGet("/meter-reading")]
        public ActionResult<List<MeterReading>> GetUserReadings(string accountId)
        {
            using (MeterReadingsContext ctx = new MeterReadingsContext())
            {
                return ctx.MeterReadings.Where(r => r.AccountId == accountId).ToList();
            }
        }

        [HttpGet("/users")]
        public ActionResult<List<UserAccount>> GetAllUsers()
        {
            using (MeterReadingsContext ctx = new MeterReadingsContext())
            {
                return ctx.UserAccounts.ToList();
            }
        }

        [HttpGet("/user")]
        public ActionResult<UserAccount> GetUser(string firstName, string lastName)
        {
            using (MeterReadingsContext ctx = new MeterReadingsContext())
            {
                return ctx.UserAccounts.Where(a => a.FirstName == firstName && a.LastName == lastName).FirstOrDefault();
            }
        }
    }
}
