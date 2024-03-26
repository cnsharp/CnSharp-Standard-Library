using System;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberRule<out TId>
    {
        TId Id { get; }
        string Code { get; set; }
        int StartValue { get; set; }
        int Step { get; set; }

        /// <summary>
        /// Pattern of sequence name,like '%wid%PO'
        /// would be formatted and replaced %wid' by real value in context
        /// </summary>
        string SequencePattern { get; set; }

        /// <summary>
        /// Pattern of serial number which will be generated,like '%wid%PO%yyyyMMdd%%seq5%',
        /// and generating result like 'BJ01PO2024022500001'
        /// </summary>
        string NumberPattern { get; set; }

        /// <summary>
        /// Get the seperated sequence name you customized per tenant or organization unit.
        /// Or as default as <see cref="Code"/>.
        /// </summary>
        /// <returns></returns>
        string SeperatedSequenceName { get; }
    }

    public interface IGuidSerialNumberRule : ISerialNumberRule<Guid>
    {
    }
}