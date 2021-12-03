namespace Solution.Shared.Tests.Data
{
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FormatterServiceTestsData
    {
        public static IEnumerable BasicCorrectTestData
        {
            get
            {
                yield return new TestCaseData("1:23.45", "1:23.45");
                yield return new TestCaseData("12345", "1:23.45");
                yield return new TestCaseData("12345678", "12:34:56.78");
                yield return new TestCaseData("02", "0.2");
                yield return new TestCaseData("-02", "-0.2");
                yield return new TestCaseData("+0.2", "+0.2");
                yield return new TestCaseData("1:23.4*", "1:23.4*");
                yield return new TestCaseData("1234*", "1:23.4*");
            }
        }

        public static IEnumerable BasicIncorrectTestData
        {
            get
            {
                yield return new TestCaseData("1:23.4524");
            }
        }
    }
}
