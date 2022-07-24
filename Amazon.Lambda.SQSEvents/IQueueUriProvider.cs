using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public interface IQueueUriProvider<TFunction>
{
    Task<Uri> GetQueueUriAsync(Expression<Action<TFunction>> expression);
    Task<Uri> GetQueueUriAsync();
}