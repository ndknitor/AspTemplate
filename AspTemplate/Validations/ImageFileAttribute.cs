using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public class ImageFileAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult("Invalid image file.");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                return new ValidationResult("Invalid image file.");
            }
        }

        return ValidationResult.Success;
    }
}
