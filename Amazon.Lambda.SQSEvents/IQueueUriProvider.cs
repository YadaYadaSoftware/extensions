using System.Linq.Expressions;

namespace Amazon.Lambda.SQSEvents;

public interface IQueueUriProvider<TFunction>
{
    Task<Uri> GetQueueUriAsync(Expression<Action<TFunction>> expression);
    Task<Uri> GetQueueUriAsync();
}