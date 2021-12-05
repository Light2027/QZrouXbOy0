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

                // Zero accuracy cases
                yield return new TestCaseData("1:23", "1:23");
                yield return new TestCaseData("11:23", "11:23");
                yield return new TestCaseData("1:11:23", "1:11:23");
                yield return new TestCaseData("11:11:23", "11:11:23");
                yield return new TestCaseData("+1:23", "+1:23");
                yield return new TestCaseData("+11:23", "+11:23");
                yield return new TestCaseData("+1:11:23", "+1:11:23");
                yield return new TestCaseData("+11:11:23", "+11:11:23");
                yield return new TestCaseData("-1:23", "-1:23");
                yield return new TestCaseData("-11:23", "-11:23");
                yield return new TestCaseData("-1:11:23", "-1:11:23");
                yield return new TestCaseData("-11:11:23", "-11:11:23");
            }
        }

        public static IEnumerable BasicIncorrectTestData
        {
            get
            {
                yield return new TestCaseData("1:23.4524", "Millisecond accuracy must be at most 3 decimal places.");
            }
        }
    }
}
