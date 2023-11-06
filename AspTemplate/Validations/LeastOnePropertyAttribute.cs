using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

[AttributeUsage(AttributeTargets.Class)]
public class LeastOnePropertyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var properties = validationContext.ObjectType.GetProperties();

        bool atLeastOneProvided = properties.Any(property =>
        {
            var propertyValue = property.GetValue(validationContext.ObjectInstance);
            return propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString());
        });

        if (!atLeastOneProvided)
        {
            return new ValidationResult("At least one property must be provided.");
        }

        return ValidationResult.Success;
    }
}
