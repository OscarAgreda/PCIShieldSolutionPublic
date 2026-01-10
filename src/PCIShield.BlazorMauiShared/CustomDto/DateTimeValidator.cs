using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
namespace PCIShield.Domain.ModelsDto;
public class DateTimeHandler
{
    private readonly ILogger _logger;
    private readonly IStringLocalizer _localizer;
    public DateTimeHandler(ILogger logger, IStringLocalizer localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }
    public void FormatDatesForUI<T>(T entityModel) where T : class
    {
        if (entityModel == null) return;
        try
        {
            var dateProperties = GetDateTimeProperties(entityModel);
            foreach (var prop in dateProperties)
            {
                var value = prop.GetValue(entityModel) as DateTime?;
                if (value.HasValue)
                {
                    var utcDateTime = EnsureUtc(value.Value);
                    var localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);
                    prop.SetValue(entityModel, localDate);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting dates for UI");
        }
    }
    public void FormatDatesForServer<T>(T entityModel) where T : class
    {
        if (entityModel == null) return;
        try
        {
            var dateProperties = GetDateTimeProperties(entityModel);
            foreach (var prop in dateProperties)
            {
                var value = prop.GetValue(entityModel) as DateTime?;
                if (value.HasValue)
                {
                    var localDateTime = EnsureLocal(value.Value);
                    var utcDate = TimeZoneInfo.ConvertTimeToUtc(localDateTime);
                    prop.SetValue(entityModel, utcDate);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting dates for server");
        }
    }
    private static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        else if (dateTime.Kind == DateTimeKind.Local)
        {
            return dateTime.ToUniversalTime();
        }
        return dateTime;
    }
    private static DateTime EnsureLocal(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }
        else if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime.ToLocalTime();
        }
        return dateTime;
    }
    private IEnumerable<PropertyInfo> GetDateTimeProperties<T>(T entity) where T : class
    {
        return entity.GetType()
            .GetProperties()
            .Where(p => p.PropertyType == typeof(DateTime?) || p.PropertyType == typeof(DateTime));
    }
    public async Task<string> ValidateAndUpdateDate<T>(
        T entityModel,
        string propertyName,
        DateTime? newDate,
        bool isRequired = false,
        int? minDaysFromToday = null,
        int? maxDaysFromToday = null) where T : class
    {
        if (entityModel == null) return _localizer["Invalid model"];
        if (newDate.HasValue)
        {
            newDate = EnsureUtc(newDate.Value);
        }
        var property = entityModel.GetType().GetProperty(propertyName);
        if (property != null)
        {
            property.SetValue(entityModel, newDate);
        }
        DateTime? customMinDate = minDaysFromToday.HasValue
            ? DateTime.UtcNow.Date.AddDays(minDaysFromToday.Value)
            : null;
        DateTime? customMaxDate = maxDaysFromToday.HasValue
            ? DateTime.UtcNow.Date.AddDays(maxDaysFromToday.Value)
            : null;
        var validationResult = DateTimeValidator.Validate(
            entityModel,
            propertyName,
            allowNull: !isRequired,
            customMinDate: customMinDate,
            customMaxDate: customMaxDate
        );
        return !validationResult.IsValid
            ? _localizer[validationResult.ValidationMessages.First()]
            : null;
    }
}
public class DateValidationResult
{
    public bool IsValid { get; set; }
    public List<string> ValidationMessages { get; set; } = new();
    public DateValidationResult(bool isValid = true)
    {
        IsValid = isValid;
    }
    public void AddError(string message)
    {
        IsValid = false;
        ValidationMessages.Add(message);
    }
}
public static class DateTimeValidator
{
    private static readonly DateTime MinimumAllowedDate = DateTime.UtcNow.AddYears(-10);
    public static DateValidationResult Validate<T>(T model, string propertyName,
        bool allowNull = true, DateTime? customMinDate = null, DateTime? customMaxDate = null)
    {
        var result = new DateValidationResult();
        if (model == null)
        {
            result.AddError($"Model cannot be null when validating {propertyName}");
            return result;
        }
        var property = model.GetType().GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property == null)
        {
            result.AddError($"Property {propertyName} not found on type {model.GetType().Name}");
            return result;
        }
        var value = property.GetValue(model);
        var isNullableDateTime = Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime);
        if (value == null)
        {
            if (!allowNull)
            {
                result.AddError($"{propertyName} cannot be null");
            }
            return result;
        }
        DateTime dateValue;
        if (isNullableDateTime)
        {
            dateValue = ((DateTime?)value).Value;
        }
        else if (value is DateTime dt)
        {
            dateValue = dt;
        }
        else
        {
            result.AddError($"{propertyName} is not a valid DateTime type");
            return result;
        }
        var minDate = customMinDate ?? MinimumAllowedDate;
        if (dateValue < minDate)
        {
            result.AddError($"{propertyName} cannot be earlier than {minDate:d}");
        }
        var maxDate = customMaxDate ?? DateTime.UtcNow;
        if (dateValue > maxDate)
        {
            result.AddError($"{propertyName} cannot be later than {maxDate:d}");
        }
        return result;
    }
    public static DateValidationResult ValidateMultiple<T>(T model,
        params (string PropertyName, bool AllowNull, DateTime? MinDate, DateTime? MaxDate)[] propertiesToValidate)
    {
        var result = new DateValidationResult();
        foreach (var prop in propertiesToValidate)
        {
            var propResult = Validate(model, prop.PropertyName, prop.AllowNull, prop.MinDate, prop.MaxDate);
            if (!propResult.IsValid)
            {
                result.IsValid = false;
                result.ValidationMessages.AddRange(propResult.ValidationMessages);
            }
        }
        return result;
    }
    public static bool IsDateValid(DateTime date)
    {
        return date.IsWithinRange(
            minDate: DateTime.UtcNow.AddMonths(-6),
            maxDate: DateTime.UtcNow.AddMonths(6)
        );
    }
    public static bool IsValidDateRange(DateTime? startDate, DateTime? endDate, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (startDate == null && endDate == null)
            return true;
        if (startDate == null || endDate == null)
        {
            errorMessage = "Both start and end dates must be provided";
            return false;
        }
        if (endDate < startDate)
        {
            errorMessage = "End date must be after start date";
            return false;
        }
        return true;
    }
    public static bool IsWithinRange(this DateTime date, DateTime? minDate = null, DateTime? maxDate = null)
    {
        var min = minDate ?? MinimumAllowedDate;
        var max = maxDate ?? DateTime.UtcNow;
        return date >= min && date <= max;
    }
}