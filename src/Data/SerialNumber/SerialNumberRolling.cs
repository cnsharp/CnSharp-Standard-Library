﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberRolling
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public int Y { get; set; }
        public int M { get; set; }
        public int D { get; set; }
        public long CurrentValue { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
        public DateTimeOffset DateCreated { get; set;  } = DateTimeOffset.Now;
        public DateTimeOffset DateUpdated { get; set; }
    }
}
