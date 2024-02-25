using System;
using System.ComponentModel.DataAnnotations;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberRolling
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Date { get; set; }
        public long CurrentValue { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
        public DateTimeOffset DateCreated { get; set;  } = DateTimeOffset.Now;
        public DateTimeOffset DateUpdated { get; set; }
    }
}
