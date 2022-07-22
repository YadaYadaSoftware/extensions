using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>Options that can be used to configure Lambda logging.</summary>
public class LambdaLoggerOptions
{
    internal const string DEFAULT_SECTION_NAME = "Lambda.Logging";
    private const string INCLUDE_LOG_LEVEL_KEY = "IncludeLogLevel";
    private const string INCLUDE_CATEGORY_KEY = "IncludeCategory";
    private const string INCLUDE_NEWLINE_KEY = "IncludeNewline";
    private const string INCLUDE_EXCEPTION_KEY = "IncludeException";
    private const string INCLUDE_EVENT_ID_KEY = "IncludeEventId";
    private const string INCLUDE_SCOPES_KEY = "IncludeScopes";
    private const string LOG_LEVEL_KEY = "LogLevel";
    private const string DEFAULT_CATEGORY = "Default";

    /// <summary>
    /// Flag to indicate if LogLevel should be part of logged message.
    /// Default is true.
    /// </summary>
    public bool IncludeLogLevel { get; set; }

    /// <summary>
    /// Flag to indicate if Category should be part of logged message.
    /// Default is true.
    /// </summary>
    public bool IncludeCategory { get; set; }

    /// <summary>
    /// Flag to indicate if logged messages should have a newline appended
    /// to them, if one isn't already there.
    /// Default is true.
    /// </summary>
    public bool IncludeNewline { get; set; }

    /// <summary>
    /// Flag to indicate if Exception should be part of logged message.
    /// Default is false.
    /// </summary>
    public bool IncludeException { get; set; }

    /// <summary>
    /// Flag to indicate if EventId should be part of logged message.
    /// Default is false.
    /// </summary>
    public bool IncludeEventId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether scopes should be included in the message.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IncludeScopes { get; set; }

    /// <summary>
    /// Function used to filter events based on the log level.
    /// Default value is null and will instruct logger to log everything.
    /// </summary>
    [CLSCompliant(false)]
    public Func<string, LogLevel, bool> Filter { get; set; }

    /// <summary>
    /// Constructs instance of LambdaLoggerOptions with default values.
    /// </summary>
    public LambdaLoggerOptions()
    {
        this.IncludeCategory = true;
        this.IncludeLogLevel = true;
        this.IncludeNewline = true;
        this.IncludeException = false;
        this.IncludeEventId = false;
        this.IncludeScopes = false;
        this.Filter = (Func<string, LogLevel, bool>)null;
    }

    /// <summary>
    /// Constructs instance of LambdaLoggerOptions with values from "Lambda.Logging"
    /// subsection of the specified configuration.
    /// The following configuration keys are supported:
    ///  IncludeLogLevel - boolean flag indicates if LogLevel should be part of logged message.
    ///  IncludeCategory - boolean flag indicates if Category should be part of logged message.
    ///  LogLevels - category-to-LogLevel mapping which indicates minimum LogLevel for a category.
    /// </summary>
    /// <param name="configuration"></param>
    [CLSCompliant(false)]
    public LambdaLoggerOptions(IConfiguration configuration)
        : this(configuration, "Lambda.Logging")
    {
    }

    /// <summary>
    /// Constructs instance of LambdaLoggerOptions with values from specified
    /// subsection of the configuration.
    /// The following configuration keys are supported:
    ///  IncludeLogLevel - boolean flag indicates if LogLevel should be part of logged message.
    ///  IncludeCategory - boolean flag indicates if Category should be part of logged message.
    ///  LogLevels - category-to-LogLevel mapping which indicates minimum LogLevel for a category.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="loggingSectionName"></param>
    [CLSCompliant(false)]
    public LambdaLoggerOptions(IConfiguration configuration, string loggingSectionName)
        : this()
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
        IConfigurationSection configurationSection = !string.IsNullOrEmpty(loggingSectionName) ? configuration.GetSection(loggingSectionName) : throw new ArgumentNullException(nameof(loggingSectionName));
        if (configurationSection == null)
            throw new ArgumentOutOfRangeException(nameof(loggingSectionName), "Unable to find section '" + loggingSectionName + "' in current configuration.");
        string str1;
        if (LambdaLoggerOptions.TryGetString((IConfiguration)configurationSection, nameof(IncludeCategory), out str1))
            this.IncludeCategory = bool.Parse(str1);
        string str2;
        if (LambdaLoggerOptions.TryGetString((IConfiguration)configurationSection, nameof(IncludeLogLevel), out str2))
            this.IncludeLogLevel = bool.Parse(str2);
        string str3;
        if (LambdaLoggerOptions.TryGetString((IConfiguration)configurationSection, nameof(IncludeException), out str3))
            this.IncludeException = bool.Parse(str3);
        string str4;
        if (LambdaLoggerOptions.TryGetString((IConfiguration)configurationSection, nameof(IncludeEventId), out str4))
            this.IncludeEventId = bool.Parse(str4);
        string str5;
        if (LambdaLoggerOptions.TryGetString((IConfiguration)configurationSection, nameof(IncludeNewline), out str5))
            this.IncludeNewline = bool.Parse(str5);
        IConfiguration logLevelsSection;
        if (LambdaLoggerOptions.TryGetSection((IConfiguration)configurationSection, "LogLevel", out logLevelsSection))
            this.Filter = LambdaLoggerOptions.CreateFilter(logLevelsSection);
        string str6;
        if (!LambdaLoggerOptions.TryGetString((IConfiguration)configurationSection, nameof(IncludeScopes), out str6))
            return;
        this.IncludeScopes = bool.Parse(str6);
    }

    private static bool TryGetString(IConfiguration configuration, string key, out string value)
    {
        value = configuration[key];
        return value != null;
    }

    private static bool TryGetSection(
        IConfiguration configuration,
        string key,
        out IConfiguration value)
    {
        value = (IConfiguration)configuration.GetSection(key);
        return value != null;
    }

    private static Func<string, LogLevel, bool> CreateFilter(
        IConfiguration logLevelsSection)
    {
        List<IConfigurationSection> list = logLevelsSection.GetChildren().ToList<IConfigurationSection>();
        if (list.Count == 0)
            return (Func<string, LogLevel, bool>)null;
        Dictionary<string, LogLevel> logLevelsMapping = new Dictionary<string, LogLevel>((IEqualityComparer<string>)StringComparer.Ordinal);
        LogLevel defaultLogLevel = LogLevel.Information;
        foreach (IConfigurationSection configurationSection in list)
        {
            string key1 = configurationSection.Key;
            string str = configurationSection.Value;
            LogLevel result;
            if (!Enum.TryParse<LogLevel>(str, out result))
                throw new InvalidCastException("Unable to convert level '" + str + "' for category '" + key1 + "' to LogLevel.");
            if (key1.Contains("*"))
            {
                if (key1.Count<char>((Func<char, bool>)(x => x == '*')) > 1)
                    throw new ArgumentOutOfRangeException("Category '" + key1 + "' is invalid - only 1 wildcard is supported in a category.");
                string key2 = key1.IndexOf('*') == key1.Length - 1 ? key1.TrimEnd('*') : throw new ArgumentException("Category '" + key1 + "' is invalid - wilcards are only supported at the end of a category.");
                logLevelsMapping[key2] = result;
            }
            else if (key1.Equals("Default", StringComparison.OrdinalIgnoreCase))
                defaultLogLevel = result;
            else
                logLevelsMapping[key1] = result;
        }
        List<string> orderedCategories = logLevelsMapping.Keys.OrderByDescending<string, string>((Func<string, string>)(categoryKey => categoryKey)).ToList<string>();
        return (Func<string, LogLevel, bool>)((category, logLevel) =>
        {
            LogLevel logLevel1;
            if (logLevelsMapping.TryGetValue(category, out logLevel1))
                return logLevel >= logLevel1;
            string key = orderedCategories.FirstOrDefault<string>(new Func<string, bool>(category.StartsWith));
            if (key == null)
                return logLevel >= defaultLogLevel;
            LogLevel logLevel2 = logLevelsMapping[key];
            return logLevel >= logLevel2;
        });
    }
}