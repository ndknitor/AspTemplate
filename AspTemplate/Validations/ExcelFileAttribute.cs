using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MiniExcelLibs;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ExcelFileValidationAttribute : ValidationAttribute
{
    private readonly Type _modelType;

    public ExcelFileValidationAttribute(Type modelType)
    {
        _modelType = modelType;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile formFile)
        {
            try
            {
                using (var stream = formFile.OpenReadStream())
                {
                    var excelData = MiniExcel.Query(stream);
                    var rowIndex = 0;
                    string validationErrorMessage = "";
                    foreach (var row in excelData)
                    {
                        rowIndex++;

                        if (!IsValidExcelData(row, _modelType, out validationErrorMessage))
                        {
                            return new ValidationResult($"Error at row {rowIndex}: {validationErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file processing.
                return new ValidationResult($"File processing error: {ex.Message}");
            }
        }

        return ValidationResult.Success;
    }
    private bool IsValidExcelData(object row, Type modelType, out string validationErrorMessage)
    {
        // Create an instance of the modelType
        var modelInstance = Activator.CreateInstance(modelType);

        // Set the row data to the modelInstance properties using reflection
        foreach (var property in modelType.GetProperties())
        {
            var rowValue = row.GetType().GetProperty(property.Name)?.GetValue(row, null);
            property.SetValue(modelInstance, rowValue);
        }

        var validationContext = new ValidationContext(modelInstance, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(modelInstance, validationContext, validationResults, true);

        if (!isValid)
        {
            validationErrorMessage = string.Join(" ", validationResults.Select(result => result.ErrorMessage));
        }
        else
        {
            validationErrorMessage = null;
        }

        return isValid;
    }

}
