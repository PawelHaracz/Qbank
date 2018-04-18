using System;
using System.Collections.Generic;
using System.Text;

namespace Qbank.Tests
{
    /// <summary>
    /// Enables writing indented text to <see cref="Console.Out"/>
    /// </summary>
    public static class ConsoleExtensions
    {
        /// <summary>
        /// Increases the indent returing <see cref="IDisposable"/> that brings the previous value when disposed.
        /// </summary>
        /// <returns></returns>
        public static IDisposable Indent()
        {
            const int value = 1;

            __.Indented.Indent += value;

            return new Disposable(() => __.Indented.Indent -= value);
        }

        class Disposable : IDisposable
        {
            readonly Action a;

            public Disposable(Action a)
            {
                this.a = a;
            }

            public void Dispose()
            {
                a();
            }
        }
    }
}
