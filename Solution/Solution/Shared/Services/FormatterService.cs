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

            if (input.Length < 3)
                throw new ArgumentException($"{nameof(input)} is too short to be valid.");

            // Rules:
            // Correctly formatted time is fine then return a copy of it. (Not sanitizing it as, in case it needs it, it is not a valid timestring)
            // If partially correct e.g.: "1:234" => Error

            if (this.IsValidTimeString(input))
                return new string(input);

            var inputValidator = new Regex(@"^([+-]?)(0?)(([1-9]|1[0-9]|2[0-3])?)(([0-9]|[1-5][0-9])?)(([0-9]|[1-5][0-9])?)(([1-9]\*)|[1-9][0-9])$");
            if (!inputValidator.IsMatch(input))
                throw new ArgumentException($"{nameof(input)} cannot be partially correct, it must be either fully correct e.g.: '1:23.45' or formattable e.g.: '1234' or '1234*'.");

            // Indexes Last <-> Last - 2 => Milliseconds
            (string baseString, int baseProcessedCharCount) = this.ParseBaseString(input);
            var builder = new StringBuilder(baseString);

            // Indexes Last - 3 <-> Last - 5 => Seconds
            string secondsToProcess = string.Concat(input.Take(input.Length - baseProcessedCharCount));
            if (this.TryParseSeconds(secondsToProcess, out string seconds, out int secondsProcessedCharCount))
            {
                builder.Insert(0, seconds);
                // Indexes Last - 6 <-> Last - 8 => Minutes
                string minutesToProcess = string.Concat(secondsToProcess.Take(secondsToProcess.Length - secondsProcessedCharCount));
                if (this.TryParseMinutes(minutesToProcess, out string minutes, out int minutesProcessedCharCount))
                {
                    builder.Insert(0, $"{minutes}:");
                    // Indexes Last - 9 <-> Last - 11 => Hours
                    string hoursToProcess = string.Concat(minutesToProcess.Take(minutesToProcess.Length - minutesProcessedCharCount));
                    if (this.TryParseHours(hoursToProcess, out string hours, out int hoursProcessedCharCount))
                        builder.Insert(0, $"{hours}:");
                }
            }

            // Index 0 => Possible prefix 
            if (input.First() == '+' || input.First() == '-')
                builder.Insert(0, input.First());

            return builder.ToString();
        }

        private bool IsValidTimeString(string input)
        {
            // Rule:
            // [+-]? ([0-23]:)? ([0-59]:)? ([0-59].[1-999])


            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns either ".xy" or "-xx" || "+xx" where x is a digit and y is also a digit or a '*'
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private (string, int) ParseBaseString(string input)
        {
            // Condition input is string with content and is atleast 3 chars long

            // <-
            // ms

            // These were already validated
            var lastIndex = input.Length - 1; // Either a digit or a * => Validated before
            var decimalIndex = input.Length - 2; // Must be a digit => Validated before

            // If this is a digit then a 0 decimal value is allowed (301 -> 3.01), if it is a prefix then it is the second value (-02 -> -0.2), not allowed if last index is a '*'
            var indicatorIndex = input.Length - 3;
            bool hasPrefix = input[indicatorIndex] == '+' || input[indicatorIndex] == '-';
            int processedCharCount = hasPrefix ? 3 : 2;

            return hasPrefix ? ($"{input[indicatorIndex]}{input[decimalIndex]}.{input[lastIndex]}", processedCharCount) : ($".{input[decimalIndex]}{input[lastIndex]}", processedCharCount);
        }

        private bool TryParseSeconds(string input, out string seconds, out int processedCharCount)
        {
            if (string.IsNullOrEmpty(input))
            {
                seconds = string.Empty;
                processedCharCount = 0;
                return false;
            }

            // <-
            // {ss}ms

            throw new NotImplementedException();
        }

        private bool TryParseMinutes(string input, out string minutes, out int processedCharCount)
        {
            if (string.IsNullOrEmpty(input))
            {
                minutes = string.Empty;
                processedCharCount = 0;
                return false;
            }

            // <-
            // {mm}ssms
            throw new NotImplementedException();
        }

        private bool TryParseHours(string input, out string hours, out int processedCharCount)
        {
            if (string.IsNullOrEmpty(input))
            {
                hours = string.Empty;
                processedCharCount = 0;
                return false;
            }

            // <-
            // {hh}mmssms
            throw new NotImplementedException();
        }
    }
}
