using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

[AttributeUsage(AttributeTargets.Property)]
public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly long _maxFileSizeInBytes;

    public MaxFileSizeAttribute(int maxFileSizeInMegabytes)
    {
        if (maxFileSizeInMegabytes <= 0)
        {
            throw new ArgumentException("Maximum file size must be greater than zero.", nameof(maxFileSizeInMegabytes));
        }

        _maxFileSizeInBytes = maxFileSizeInMegabytes * 1024 * 1024; // Convert MB to bytes
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            if (file.Length > _maxFileSizeInBytes)
            {
                return new ValidationResult($"The file size cannot exceed {_maxFileSizeInBytes / (1024 * 1024)} MB.");
            }
        }

        return ValidationResult.Success;
    }
}
