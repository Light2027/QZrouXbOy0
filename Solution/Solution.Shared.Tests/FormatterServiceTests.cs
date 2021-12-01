namespace Solution.Shared.Tests
{
    using NUnit.Framework;
    using Solution.Shared.Services;
    using Solution.Shared.Services.Interfaces;
    using Solution.Shared.Tests.Data;

    public class FormatterServiceTests
    {
        private readonly IFormatterService formatterService;

        public FormatterServiceTests()
        {
            this.formatterService = new FormatterService();
        }

        [Test, TestCaseSource(typeof(FormatterServiceTestsData), nameof(FormatterServiceTestsData.))]
        public void ExampleDataTest(string input, string expectedReturnValue)
        {
            Assert.AreEqual(expectedReturnValue, this.formatterService.FormatToTimeString(input));
        }
    }
}