using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadingsAPI.Models
{
    public class MeterReading
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public DateTime DateChecked { get; set; }
        public int Value { get; set; }
    }
}
