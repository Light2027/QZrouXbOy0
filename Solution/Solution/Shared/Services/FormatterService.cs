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

            // Check if it is already valid
            if (this.IsValidTimeString(input))
                return new string(input);

            // If not then return
            // Division Section
            string prefix = this.IsPrefix(input.First()) ? string.Concat(input.First()) : string.Empty;
            string prefixlesInput = string.IsNullOrEmpty(prefix) ? input : this.StringSkip(input, 1);
            string star = prefixlesInput.Last() == '*' ? string.Concat(prefixlesInput.Last()) : string.Empty;
            string cleanInputWithPadding = string.IsNullOrEmpty(star) ? prefixlesInput : this.StringTake(prefixlesInput, prefixlesInput.Length - 1);
            string cleanInput = cleanInputWithPadding.Length < 3 ? cleanInputWithPadding : cleanInputWithPadding.TrimStart('0');
            if (string.IsNullOrEmpty(cleanInput))
                throw new ArgumentException($"Core input: '{nameof(cleanInput)}' cannot solely consist of zeros (0).");

            string reversedCleanInput = StringReverse(cleanInput);
            if (!reversedCleanInput.All(c => char.IsDigit(c)))
                throw new ArgumentException($"The core part of the input ('{StringReverse(reversedCleanInput)}') must consists solely of digits.");

            // Handle short inputs Length < 3
            if (reversedCleanInput.Length < 3)
            {
                switch (reversedCleanInput.Length)
                {
                    case 1:
                        if (!string.IsNullOrEmpty(star))
                            return $"{prefix}0.{reversedCleanInput}{star}";
                        else
                            return $"{prefix}0.0{reversedCleanInput}";
                    case 2:
                        if (!string.IsNullOrEmpty(star))
                            return $"{prefix}{StringReverse(reversedCleanInput)}{star}";
                        else
                            return $"{prefix}{reversedCleanInput[1]}.{reversedCleanInput[0]}";
                }
            }

            string milliSeconds = StringReverse(ExtractMilliseconds(reversedCleanInput, !string.IsNullOrEmpty(star), out string baselessCleanInput));

            string seconds = StringReverse(StringTake(baselessCleanInput, 2));

            string minutesRaw = StringSkip(baselessCleanInput, 2);
            string minutes = StringReverse(StringTake(minutesRaw, 2));

            string hoursRaw = StringSkip(minutesRaw, 2);
            string hours = StringReverse(StringTake(hoursRaw, 2));

            string remainder = StringSkip(hoursRaw, 2);
            if (remainder.Length != 0)
                throw new ArgumentException($"{remainder} in {input} cannot be interpreted.");

            // Rule machine section
            var builder = new StringBuilder(milliSeconds);
            if (!string.IsNullOrEmpty(seconds))
            {
                if (!StringIntInRange(seconds, 0, 59))
                    throw new ArgumentException($"{seconds} in {input} must be between 01 and 59.");

                builder.Insert(0, $"{seconds}.");

                if (!string.IsNullOrEmpty(minutes))
                {
                    if (!StringIntInRange(minutes, 0, 59))
                        throw new ArgumentException($"{minutes} in {input} must be between 01 and 59.");

                    builder.Insert(0, $"{minutes}:");

                    if (!string.IsNullOrEmpty(hours))
                    {
                        if (!StringIntInRange(hours, 1, 23))
                            throw new ArgumentException($"{minutes} in {input} must be between 1 and 23.");

                        builder.Insert(0, $"{hours.TrimStart('0')}:");
                    }
                }
            }

            builder.Insert(0, prefix);
            builder.Append(star);

            return builder.ToString();
        }

        private string StringSkip(string input, int toSkip) => string.Concat(input.Skip(toSkip));

        private string StringTake(string input, int toTake) => string.Concat(input.Take(toTake));

        private string StringReverse(string input) => string.Concat(input.Reverse());

        private bool StringIntInRange(string input, int lower, int upper)
        {
            var parsed = int.Parse(input);
            return parsed >= lower && parsed <= upper;
        }
        private bool IsPrefix(char c) => c == '+' || c == '-';

        private bool IsValidTimeString(string input)
        {
            // Rule:
            // [+-]? ([0-23]:)? ([0-59]:)? ([0-59].[0-999])
            string prefix = this.IsPrefix(input.First()) ? string.Concat(input.First()) : string.Empty;
            string prefixlesInput = string.IsNullOrEmpty(prefix) ? input : this.StringSkip(input, 1);
            string star = prefixlesInput.Last() == '*' ? string.Concat(prefixlesInput.Last()) : string.Empty;
            string cleanInput = string.IsNullOrEmpty(star) ? prefixlesInput : this.StringTake(prefixlesInput, prefixlesInput.Length - 1);
            string reversedCleanInput = StringReverse(cleanInput);
            if (
                !reversedCleanInput.All(c => char.IsDigit(c))
                && this.TrySplitTimeString(reversedCleanInput, out string[] splitReversedCleanInput)
                )
            {
                if (splitReversedCleanInput.Any(x => string.IsNullOrEmpty(x)))
                    throw new ArgumentException($"Value missing in '{input}', double seperation (.. or ::) is not allowed.");

                if (splitReversedCleanInput.All(x => x.All(c => c == '0')))
                    throw new ArgumentException("All zeros (0) time strings are not valid.");

                if (!splitReversedCleanInput.All(x => x.All(c => char.IsDigit(c))))
                    throw new ArgumentException($"Non digit character found in '{input}', these are not allowed between separators ('.',':').");


                if (splitReversedCleanInput.Last().Length > 3)
                    throw new ArgumentException("Millisecond accuracy must be at most 3 decimal places.");

                switch (splitReversedCleanInput.Length)
                {
                    // 1 is not possible here
                    case 2:
                        {
                            if (splitReversedCleanInput[0].Length > 2)
                                throw new ArgumentException("Extra numbers in seconds are not allowed.");

                            string seconds = this.StringReverse(splitReversedCleanInput.First());
                            if (
                                seconds.Length == 2
                                && seconds.First() == '0' // 0s:ms => Is not allowed
                                )
                                throw new ArgumentException($"Zero (0) padding '{seconds}' is not allowed.");

                            string milliSeconds = this.StringReverse(splitReversedCleanInput[1]);

                            throw new NotImplementedException("More checks needed.");

                            return true;
                        }
                    case 3:
                        {
                            if (splitReversedCleanInput[0].Length > 2)
                                throw new ArgumentException("Extra numbers in minutes are not allowed.");

                            string minutes = this.StringReverse(splitReversedCleanInput.First());
                            if (
                                minutes.Length == 2
                                && minutes.First() == '0' // 0m:ss:ms => Is not allowed
                                )
                                throw new ArgumentException($"Zero (0) padding '{minutes}' is not allowed.");

                            string seconds = this.StringReverse(splitReversedCleanInput[1]);
                            string milliSeconds = this.StringReverse(splitReversedCleanInput[2]);

                            throw new NotImplementedException("More checks needed.");

                            return true;
                        }
                    case 4:
                        {
                            if (splitReversedCleanInput[0].Length > 2)
                                throw new ArgumentException("Extra numbers in hours are not allowed.");

                            string hours = this.StringReverse(splitReversedCleanInput.First());
                            if (
                                hours.Length == 2
                                && hours.First() == '0' // 0h:mm:ss:ms => Is not allowed
                                )
                                throw new ArgumentException($"Zero (0) padding '{hours}' is not allowed.");

                            string minutes = this.StringReverse(splitReversedCleanInput[1]);
                            string seconds = this.StringReverse(splitReversedCleanInput[2]);
                            string milliSeconds = this.StringReverse(splitReversedCleanInput[3]);

                            throw new NotImplementedException("More checks needed.");

                            return true;
                        }
                }
            }

            return false;
        }

        private bool TrySplitTimeString(string cleanInput, out string[] splitCleanInput)
        {
            throw new NotImplementedException();
        }

        private string ExtractMilliseconds(string reversedCleanInput, bool hasStar, out string baselessCleanInput)
        {
            if (hasStar)
            {
                if (char.IsDigit(reversedCleanInput[0]))
                {
                    baselessCleanInput = this.StringSkip(reversedCleanInput, 1);
                    return this.StringTake(reversedCleanInput, 1);
                }
                else
                    throw new ArgumentException("Invalid character '{reversedInput[0]}' in input."); // This should not be possible, but still...
            }

            baselessCleanInput = this.StringSkip(reversedCleanInput, 2);
            return StringTake(reversedCleanInput, 2);
        }
    }
}
