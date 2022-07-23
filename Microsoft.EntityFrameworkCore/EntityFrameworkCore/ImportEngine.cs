using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using DateTime = System.DateTime;
using Exception = System.Exception;

namespace YadaYada.Data.Library;

public interface IImportEngine<TContext, TEntity> where TContext : DbContext
{
    IEnumerable<TEntity> Import(TContext context, Dictionary<string, PropertyInfo> columnMappings, List<string> columns, string data, char delimiter = '\t');
    Dictionary<string, PropertyInfo> GetColumnMappings();


}
public class ImportEngine<TContext, TEntity> : IImportEngine<TContext, TEntity> where TContext : DbContext where TEntity : new()
{
    private readonly ILogger _logger;

    static ImportEngine()
    {
        TypeDescriptor.AddAttributes(typeof(DateTime), new ConvertsAttribute(typeof(StringToDateTimeConverter)));
        TypeDescriptor.AddAttributes(typeof(bool), new ConvertsAttribute(typeof(StringToBoolConverter)));
        TypeDescriptor.AddAttributes(typeof(decimal), new ConvertsAttribute(typeof(StringToDecimalConverter)));
        TypeDescriptor.AddAttributes(typeof(int), new ConvertsAttribute(typeof(IntConverter)));
    }

    public ImportEngine(ILoggerProvider loggerProvider)
    {
        _logger = loggerProvider.CreateLogger(GetType().FullName);
    }

    public IEnumerable<TEntity> Import(TContext context, Dictionary<string, PropertyInfo> columnMappings, List<string> columns, string data, char delimiter = '\t')
    {
        using (_logger.BeginScope(nameof(Import)))
        {
            TypeConverterBase.Context = context;

            var errorMessage = new StringBuilder($"{nameof(TEntity)}={typeof(TEntity)}{Environment.NewLine}{nameof(delimiter)}={delimiter}");

            try
            {
                var returnValue = new List<TEntity>();

                var values = data.Split(delimiter);
                TEntity newEntity = new TEntity();
                returnValue.Add(newEntity);

                for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var key = columns[columnIndex].ToUpperInvariant();

                    using (_logger.BeginScope(key))
                    {
                        if (!columnMappings.ContainsKey(key))
                        {
                            _logger.LogTrace($"No column for key '{key}'");
                            continue;
                        }

                        PropertyInfo info = columnMappings[key];

                        if (columnIndex >= values.Length)
                        {
                            throw new InvalidOperationException($"Cannot find value for {nameof(columnIndex)}={columnIndex} in '{data}'");
                        }

                        object value = values[columnIndex];

                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            TypeConverter typeConverter = null;
                            try
                            {
                                if (info.PropertyType != typeof(string))
                                {
                                    try
                                    {
                                        typeConverter = TypeDescriptor.GetConverter(info.PropertyType);

                                        value = typeConverter.ConvertFromString(value.ToString());

                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogWarning(e, $"Cannot convert '{key}' with a value of '{values[columnIndex]}'");
                                        continue;
                                    }
                                }

                                info.GetSetMethod().Invoke(newEntity, new object[] { value });

                                _logger.LogTrace($"Set value for {key} to '{value}'");

                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e,
                                    $"{nameof(info)}.{nameof(info.PropertyType)}={info.PropertyType}, {nameof(key)}={key}, {nameof(value)}={value}, {nameof(typeConverter)}={typeConverter} {e.Message}");
                                throw;
                            }

                        }

                    }
                }


                return returnValue;

            }
            catch (Exception e)
            {
                throw new Exception(errorMessage.ToString(), e);
            }

        }        }

    public Dictionary<string, PropertyInfo> GetColumnMappings()
    {
        var properties = typeof(TEntity).GetProperties();
        var returnValue = new Dictionary<string, PropertyInfo>();

        foreach (PropertyInfo propertyInfo in properties)
        {
            var tabDelimitedAttributes = propertyInfo.GetCustomAttributes<TabDelimitedAttribute>();
            foreach (TabDelimitedAttribute tabDelimitedAttribute in tabDelimitedAttributes)
            {
                returnValue.Add(tabDelimitedAttribute.ColumnName.ToUpperInvariant(), propertyInfo);
            }
        }

        var classLevelAttributes = typeof(TEntity).GetCustomAttributes<TabDelimitedAttribute>();
        foreach (var item in classLevelAttributes)
        {
            var propertyInfo = properties.SingleOrDefault(p => p.Name.ToUpperInvariant() == item.PropertyName.ToUpperInvariant());
            if (propertyInfo == null) throw new MissingMemberException(item.PropertyName);
            returnValue.Add(item.ColumnName.ToUpperInvariant(), propertyInfo);
        }

        return returnValue;

    }
}

[Obsolete($"Use ImportEngine<,> instead.")]
public class ImportEngine
{

    public static IList<T> Import<T>(string s, char delimiter = '\t') where T:new()
    {
        var errorMessage = new StringBuilder($"{nameof(T)}={typeof(T)}{Environment.NewLine}{nameof(delimiter)}={delimiter}");
        try
        {
            var returnValue = new List<T>();
            var lines = s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var dictionary = GetColumnMappings<T>();
            var columns = lines.First().Trim().ToUpperInvariant().Split(delimiter);
            for (var i = 0; i < columns.Length; i++)
            {
                columns[i] = columns[i].Trim();
                errorMessage.AppendLine($"{i}={columns[i]}");

            }
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                var values = line.Split(delimiter);
                T newEntity = new T();
                returnValue.Add(newEntity);
                for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
                {
                    var key = columns[columnIndex];

                    if (dictionary.ContainsKey(key))
                    {
                        PropertyInfo info = dictionary[key];
                        if (columnIndex >= values.Length)
                        {
                            throw new InvalidOperationException($"Cannot find value for {nameof(columnIndex)}={columnIndex} in '{line}'");
                        }
                        object value = values[columnIndex]; 
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            try
                            {
                                if (info.PropertyType != typeof(string))
                                {
                                    var typeConverter = TypeDescriptor.GetConverter(info.PropertyType);
                                    value = typeConverter.ConvertFromString(value.ToString());
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Cannot convert value of '{value}' to {info.PropertyType.Name}:{e}");
                                throw;
                            }                                
                            info.GetSetMethod().Invoke(newEntity, new object[] { value });
                        }
                    }
                }
            }

            return returnValue;

        }
        catch (Exception e)
        {
            throw new Exception(errorMessage.ToString(),e);
        }
    }

    public static Dictionary<string, PropertyInfo> GetColumnMappings<T>() where T:new()
    {
        var properties = typeof(T).GetProperties();
        var returnValue = new Dictionary<string, PropertyInfo>();

        foreach (PropertyInfo propertyInfo in properties)
        {
            var tabDelimitedAttributes = propertyInfo.GetCustomAttributes<TabDelimitedAttribute>();
            foreach (TabDelimitedAttribute tabDelimitedAttribute in tabDelimitedAttributes)
            {
                returnValue.Add(tabDelimitedAttribute.ColumnName.ToUpperInvariant(), propertyInfo);
            }
        }

        var classLevelAttributes = typeof(T).GetCustomAttributes<TabDelimitedAttribute>();
        foreach (var item in classLevelAttributes)
        {
            var propertyInfo = properties.SingleOrDefault(p => p.Name.ToUpperInvariant() == item.PropertyName.ToUpperInvariant());
            if(propertyInfo==null) throw new MissingMemberException(item.PropertyName);
            returnValue.Add(item.ColumnName.ToUpperInvariant(), propertyInfo);
        }

        return returnValue;

    }

    static ImportEngine()
    {
        TypeDescriptor.AddAttributes(typeof(DateTime), new ConvertsAttribute(typeof(StringToDateTimeConverter)));
        TypeDescriptor.AddAttributes(typeof(bool), new ConvertsAttribute(typeof(StringToBoolConverter)));
        TypeDescriptor.AddAttributes(typeof(decimal), new ConvertsAttribute(typeof(StringToDecimalConverter)));
        TypeDescriptor.AddAttributes(typeof(int), new ConvertsAttribute(typeof(IntConverter)));
    }
}