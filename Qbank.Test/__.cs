using System;
using System.CodeDom.Compiler;
using System.IO;
using NUnit.Framework;
using Qbank.Core;

namespace Qbank.Test
{
    /// <summary>
    ///     The namespace setup that sets the <see cref="System.CodeDom.Compiler.IndentedTextWriter" /> to enable nice printing of tests.
    /// </summary>
    [SetUpFixture]
    public class __
    {
        internal static IndentedTextWriter Indented;
        static TextWriter _writer;

        [OneTimeSetUp]
        public void SetUp()
        {
            EventSerializer.Register(typeof(__).Assembly);
            _writer = Console.Out;
            Indented = new IndentedTextWriter(_writer);
            Console.SetOut(Indented);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Console.SetOut(_writer);
        }
    }
}