using System.ComponentModel.DataAnnotations;
public class DependentRequiredAttribute : ValidationAttribute
{
    private readonly string dependency;
    public DependentRequiredAttribute(string dependency)
    {
        this.dependency = dependency;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            var property = validationContext.ObjectType.GetProperty(dependency);
            if (property != null)
            {
                return new ValidationResult($"{validationContext.DisplayName} is required");
            }
        }
        return ValidationResult.Success;
    }
}