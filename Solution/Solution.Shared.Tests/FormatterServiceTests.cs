namespace Solution.Shared.Tests
{
    using NUnit.Framework;
    using Solution.Shared.Services;
    using Solution.Shared.Services.Interfaces;
    using Solution.Shared.Tests.Data;
    using System;

    public class FormatterServiceTests
    {
        private readonly IFormatterService formatterService;

        public FormatterServiceTests()
        {
            this.formatterService = new FormatterService();
        }

        [Test, TestCaseSource(typeof(FormatterServiceTestsData), nameof(FormatterServiceTestsData.BasicCorrectTestData))]
        public void ExampleDataTest(string input, string expectedReturnValue)
        {
            Assert.AreEqual(expectedReturnValue, this.formatterService.FormatToTimeString(input));
        }

        [Test, TestCaseSource(typeof(FormatterServiceTestsData), nameof(FormatterServiceTestsData.BasicIncorrectTestData))]
        public void BadDataTest(string input, string expectedMessage)
        {
            TestDelegate test = () => this.formatterService.FormatToTimeString(input);
            Assert.Throws<ArgumentException>(test, expectedMessage);
        }
    }
}