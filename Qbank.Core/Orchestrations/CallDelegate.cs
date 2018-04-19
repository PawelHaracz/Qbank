using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qbank.Core.Orchestrations
{
    /// <summary>
    /// Represent an asynchronous call, that results in <see cref="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns></returns>
    public delegate Task<TResult> CallDelegate<TResult>(Guid callId);
}
