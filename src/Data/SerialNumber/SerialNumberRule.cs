using System;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberRule
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int StartValue { get; set; }
        public int Step { get; set; } = 1;
       /// <summary>
       /// Pattern of sequence name,like '%wid%PO'
       /// would be formatted and replaced %wid' by real value by context 
       /// </summary>
        public string SequencePattern { get; set; }
        /// <summary>
        /// Pattern of serial number which will be generated,like '%wid%PO%yyyyMMdd%%seq5%',
        /// and generating result like 'BJ01PO2024022500001'
        /// </summary>
        public string NumberPattern { get; set; }
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset DateUpdated { get; set; }
    }
}
