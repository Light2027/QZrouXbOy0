namespace Solution.Shared.Services
{
    using Solution.Shared.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class FormatterService : IFormatterService
    {
        public string FormatToTimeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"'{nameof(input)}' cannot be null or whitespace.", nameof(input));

            if (input.All(c => c == '0'))
                throw new ArgumentException($"{nameof(input)} cannot solely consist of zeros(0)-s.");

            // Rules:
            // Correctly formatted time is fine then sanitize it, meaning remove leading zeros ("00.45" -> "0.45"), then return the sanitized version
            // If partially correct e.g.: "1:234" => Error

            var inputValidator = new Regex(@"^(0*)(([1-9]|1[0-9]|2[0-3])?)(([0-9]|[1-5][0-9])?)(([0-9]|[1-5][0-9])?)([1-9]|([1-9]\*)|[1-9][0-9])$");
            if (this.IsValidTimeString(input))
            {
                // Only have to sanitize Input here => Remove leading (01.34 -> 1.34) or trainling zeros (1.340 - 1.34)
                var withoutLeadingZeros = this.RemoveLeadingZerosFromTimeString(input);
                return this.RemoveTrailingZerosFromTimeString(withoutLeadingZeros);
            }

            if (!inputValidator.IsMatch(input))
                throw new ArgumentException($"{nameof(input)} cannot be partially correct, it must be either fully correct e.g.: '1:23.45' or formattable e.g.: '1234' or '1234*'.");

            var hasLeadingZero = input.First() == '0';
            var withoutLeadingZerosRaw = this.RemoveLeadingZeros(input);
            var formattingReadyInput = this.RemoveTrailingZeros(withoutLeadingZerosRaw);


            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes trailing zeros e.g.: 1230000 -> 123
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string RemoveTrailingZeros(string input) => input.TrimEnd('0');

        /// <summary>
        /// Removes leading zeros e.g.: 00001234 -> 1234
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string RemoveLeadingZeros(string input) => input.TrimStart('0');

        /// <summary>
        /// Removes trailing zeros up to the decimal point e.g.: 23.000 -> 23
        /// </summary>
        /// <param name="withoutLeadingZeros"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string RemoveTrailingZerosFromTimeString(object withoutLeadingZeros)
        {
            var builder = new StringBuilder();


            throw new NotImplementedException();

            return builder.ToString();
        }

        /// <summary>
        /// Removes leading zeros up to the decimal point e.g.: 00:00:00.45 -> 0.45
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private object RemoveLeadingZerosFromTimeString(string input)
        {
            var builder = new StringBuilder();

            throw new NotImplementedException();

            return builder.ToString();
        }

        private bool IsValidTimeString(string input)
        {
            // Rule:
            // [+-]? ([0-23]:)? ([0-59]:)? ([0-59].[1-999])



            throw new NotImplementedException();
        }
    }
}
