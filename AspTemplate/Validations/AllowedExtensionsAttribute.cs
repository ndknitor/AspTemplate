using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Linq;

[AttributeUsage(AttributeTargets.Property)]
public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public AllowedExtensionsAttribute(params string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                string allowedExtensions = string.Join(", ", _allowedExtensions);
                return new ValidationResult($"Allowed file extensions are: {allowedExtensions}");
            }
        }

        return ValidationResult.Success;
    }
}
