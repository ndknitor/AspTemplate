using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class)]
public class OnlyOnePropertyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var properties = validationContext.ObjectType.GetProperties();

        int count = 0;

        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(validationContext.ObjectInstance);

            if (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
            {
                count++;
            }
        }

        if (count > 1)
        {
            return new ValidationResult("Only one of the properties should be provided.");
        }

        return ValidationResult.Success;
    }
}
