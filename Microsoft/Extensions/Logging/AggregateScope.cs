using System;
using System.Collections.Generic;

// this is a comment

namespace Microsoft.Extensions.Logging;

public class AggregateScope : List<IDisposable>, IDisposable
{
    public void Dispose()
    {
        this.ForEach(_=>_?.Dispose());
        this.Clear();
    }
}
