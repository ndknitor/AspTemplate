using System.ComponentModel.DataAnnotations;

public class LessThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;
    public bool CanEquals { get; set; }

    public LessThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        if (value is IComparable comparable)
        {
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (IComparable)property.GetValue(validationContext.ObjectInstance);

            if (CanEquals)
            {
                if (comparable.CompareTo(comparisonValue) > 0)
                    return new ValidationResult($"{validationContext.DisplayName} must be equals or less than {_comparisonProperty}");
            }
            else
            {
                if (comparable.CompareTo(comparisonValue) >= 0)
                    return new ValidationResult($"{validationContext.DisplayName} must be less than {_comparisonProperty}");
            }

        }
        return ValidationResult.Success;
    }
}