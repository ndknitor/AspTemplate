using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public class FutureDateAttribute : ValidationAttribute
{
    public bool CanEquals { get; set; } = false;
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTimeValue)
        {
            if (CanEquals)
            {
                return dateTimeValue.Date >= DateTime.Now.Date;
            }
            else
            {
                return dateTimeValue.Date > DateTime.Now.Date;
            }

        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be after the current date and time.";
    }
}
