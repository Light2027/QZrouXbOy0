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

            // Check if it is already valid
            //if (this.IsValidTimeString(input))
            //    return new string(input);

            // If not then return
            // Division Section
            string prefix = this.IsPrefix(input.First()) ? string.Concat(input.First()) : string.Empty;
            string prefixlesInput = string.IsNullOrEmpty(prefix) ? input : this.StringSkip(input, 1);
            string star = prefixlesInput.Last() == '*' ? string.Concat(prefixlesInput.Last()) : string.Empty;
            string cleanInput = string.IsNullOrEmpty(star) ? prefixlesInput : this.StringTake(prefixlesInput, prefixlesInput.Length - 1);
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


                // Edge case: 0x => where x is a between 1-9 => result = 0.x
                throw new NotImplementedException();
            }

            string milliSeconds = StringReverse(ExtractMilliseconds(reversedCleanInput, !string.IsNullOrEmpty(star), out string baselessCleanInput));
            
            string seconds = StringReverse(StringTake(baselessCleanInput, 2));

            string minutesRaw = StringSkip(baselessCleanInput, 2);
            string minutes = StringReverse(StringTake(minutesRaw, 2));
            
            string hoursRaw = StringSkip(minutesRaw, 2);
            string hours = StringReverse(StringTake(hoursRaw, 2));
            
            string remainder = StringSkip(hoursRaw,2);
            if (remainder.Length != 0)
                throw new ArgumentException($"{remainder} in {input} cannot be interpreted.");

            // Rule machine section
            var builder = new StringBuilder(milliSeconds);
            if (!string.IsNullOrEmpty(seconds))
            {
                if(!StringIntInRange(seconds, 0, 59))
                    throw new ArgumentException($"{seconds} in {input} must be between 01 and 59.");

                string dot = string.IsNullOrEmpty(milliSeconds) ? string.Empty : ".";
                if (
                    string.IsNullOrEmpty(minutes)
                    && seconds[0] == '0'
                    )
                        builder.Insert(0,$"{seconds[1]}{dot}");
                else
                    builder.Insert(0, $"{seconds}{dot}");

                if (!string.IsNullOrEmpty(minutes))
                {
                    if (!StringIntInRange(minutes, 0, 59))
                        throw new ArgumentException($"{minutes} in {input} must be between 01 and 59.");

                    if (
                    string.IsNullOrEmpty(hours)
                    && minutes[0] == '0'
                    )
                        builder.Insert(0,$"{minutes[1]}:");
                    else
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
