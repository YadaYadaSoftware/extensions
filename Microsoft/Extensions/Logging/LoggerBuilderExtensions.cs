using System;
using Microsoft.Extensions.Configuration;
using YadaYada.BubbleBoy.WebApi.Services;
using YadaYada.Lambda.Services;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// ILoggingBuilder extensions
    /// </summary>
    public static class LoggerBuilderExtensions
    {

        /// <summary>
        /// Adds a Lambda logger provider with options loaded from the specified subsection of the
        /// configuration section.
        /// </summary>
        /// <param name="builder">ILoggingBuilder to add Lambda logger to.</param>
        /// <param name="configuration">IConfiguration to use when construction logging options.</param>
        /// <param name="loggingSectionName">Name of the logging section with required settings.</param>
        /// <returns>Updated ILoggingBuilder.</returns>
        public static ILoggingBuilder AddLoggerYadaYada(this ILoggingBuilder builder, IConfiguration configuration, string loggingSectionName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (string.IsNullOrEmpty(loggingSectionName))
            {
                throw new ArgumentNullException(nameof(loggingSectionName));
            }

            var options = new LambdaLoggerOptions(configuration, loggingSectionName);
            var provider = new LoggerProviderYadaYada(options);
            builder.AddProvider(provider);
            
            return builder;
        }
    }
}