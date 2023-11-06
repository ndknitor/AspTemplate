using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public class PastDateTimeAttribute : ValidationAttribute
{
    public bool CanEquals { get; set; } = false;
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTimeValue)
        {
            if (CanEquals)
            {
                return dateTimeValue <= DateTime.Now;
            }
            else
            {
                return dateTimeValue < DateTime.Now;
            }
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be before the current date and time.";
    }
}
