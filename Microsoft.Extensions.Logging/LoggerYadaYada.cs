using Amazon.Lambda.Core;


namespace Microsoft.Extensions.Logging
{
    public class LoggerYadaYada : ILogger
    {
        private readonly LambdaLoggerOptions _options;
        public string CategoryName { get; }

        public LoggerYadaYada(string categoryName, LambdaLoggerOptions options)
        {
            _options = options;
            CategoryName = categoryName;
        }

        internal IExternalScopeProvider ScopeProvider { get;} = new LoggerExternalScopeProvider();
        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? new NoOpDisposable();

        public bool IsEnabled(LogLevel logLevel)
        {
            var isEnabled = (_options.Filter == null || _options.Filter(this.CategoryName, logLevel) || (Environment.GetEnvironmentVariable("LAMBDA_TRACE_ALL")?.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? false));
            return isEnabled;
        }

        public virtual void Write(string logEntry)
        {
            LambdaLogger.Log(logEntry);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                ScopeProvider.Push(state);

                if (formatter == null)
                {
                    throw new ArgumentNullException(nameof(formatter));
                }

                if (!IsEnabled(logLevel))
                {
                    return;
                }

                // Format of the logged text, optional components are in {}
                //  {[LogLevel] }{ => Scopes : }{Category: }{EventId: }MessageText {Exception}{\n}

                var components = new List<string>(4);


                if (_options.IncludeLogLevel)
                {
                    components.Add(logLevel.ToString().ToUpperInvariant());
                }

                if (_options.IncludeCategory)
                {
                    components.Add(this.CategoryName);
                }

                var text = formatter.Invoke(state, exception);
                components.Add(text);

                GetScopeInformation(components);

                if (_options.IncludeEventId)
                {
                    components.Add($"[{eventId}]:");
                }

                if (_options.IncludeException)
                {
                    components.Add($"{exception}");
                }

                if (_options.IncludeNewline)
                {
                    components.Add(Environment.NewLine);
                }

                var finalText = string.Join(Environment.NewLine, components).Replace("\n", " ");

                this.Write(finalText);

            }
            catch (Exception e)
            {
                this.Write(e.ToString());
            }
        }

        private void GetScopeInformation(List<string> logMessageComponents)
        {
            try
            {
                if (!_options.IncludeScopes) return;

                var keyedScopes = new Dictionary<string, string>();

                AddToKeyedScopes(keyedScopes, nameof(CategoryName), this.CategoryName);


                ScopeProvider.ForEachScope((scope, list) =>
                {
                    try
                    {
                        switch (scope)
                        {
                            case Dictionary<string, string> dictionaryStringString:
                                    
                                list.Add(System.Text.Json.JsonSerializer.Serialize(dictionaryStringString));
                                break;
                            case Dictionary<string, object> dictionaryStringObject:
                                list.Add(System.Text.Json.JsonSerializer.Serialize(dictionaryStringObject));
                                break;
                            case KeyValuePair<string, object>(var key, var value):
                                AddToKeyedScopes(keyedScopes, key, value);
                                break;
                            case IReadOnlyList<KeyValuePair<string, object>> readOnlyList:
                                foreach (var (readOnlyListKey, value) in readOnlyList)
                                {
                                    if (readOnlyListKey.StartsWith('{') && readOnlyListKey.EndsWith('}')) continue;
                                    AddToKeyedScopes(keyedScopes, readOnlyListKey, value);
                                }

                                break;
                            case KeyValuePair<string, string>(var key, var value):
                                AddToKeyedScopes(keyedScopes, key, value);
                                break;
                            default:
                                AddToKeyedScopes(keyedScopes,"Scope",scope.ToString());
                                break;
                        }

                    }
                    catch (NullReferenceException)
                    {
                        keyedScopes.Add(scope.ToString(), "null");

                    }
                    catch (Exception e)
                    {
                        keyedScopes.Add($"Error Adding Item {scope} {Guid.NewGuid()}", e.ToString());
                    }
                }, (logMessageComponents));

                if (keyedScopes.Any())
                {
                    logMessageComponents.Add(System.Text.Json.JsonSerializer.Serialize(keyedScopes));
                }

            }
            catch (Exception e)
            {
                logMessageComponents.Add(e.ToString());
            }
        }

        private static void AddToKeyedScopes(Dictionary<string, string> keyedScopes, string key, object value)
        {
                
            if (!keyedScopes.ContainsKey(key))
            {
                keyedScopes.Add(key, value.ToString());
            }
            else
            {
                if (keyedScopes[key] == value.ToString()) return;
                keyedScopes[key] += ", " + value;
            }
        }

        private class NoOpDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

    }
}
