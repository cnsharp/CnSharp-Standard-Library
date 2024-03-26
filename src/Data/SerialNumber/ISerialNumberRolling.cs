using System;
using System.ComponentModel.DataAnnotations;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberRolling<out TId>
    {
        TId Id { get; }
        string Code { get; set; }
        string Date { get; set; }
        long CurrentValue { get; set; }
    }

    public interface IGuidSerialNumberRolling : ISerialNumberRolling<Guid>
    {
        
    }
}