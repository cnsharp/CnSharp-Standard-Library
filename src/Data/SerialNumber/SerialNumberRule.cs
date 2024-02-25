using System;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberRule
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public int StartValue { get; set; }
        public int Step { get; set; } = 1;
        public string Pattern { get; set; }
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset DateUpdated { get; set; }
    }
}
