namespace Qbank.Tests
{
    /// <summary>
    ///     The namespace setup that sets the <see cref="IndentedTextWriter" /> to enable nice printing of tests.
    /// </summary>
    [SetUpFixture]
    public class __
    {
        internal static IndentedTextWriter Indented;
        static TextWriter _writer;

        [OneTimeSetUp]
        public void SetUp()
        {
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