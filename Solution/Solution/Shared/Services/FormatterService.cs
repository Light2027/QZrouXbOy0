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
                throw new ArgumentException($"'{nameof(input)}' cannot be null or whitespace.");

            // Input must contain some digits
            if (!input.Any(c => char.IsDigit(c)))
                throw new ArgumentException($"{nameof(input)}: '{input}' is incorrect.");

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
            // [+-]? ([0-23]:)? ([0-59]:)? ([0-59].[0-999]) *?
            string prefix = this.IsPrefix(input.First()) ? string.Concat(input.First()) : string.Empty;
            string prefixlesInput = string.IsNullOrEmpty(prefix) ? input : this.StringSkip(input, 1);
            string star = prefixlesInput.Last() == '*' ? string.Concat(prefixlesInput.Last()) : string.Empty;
            string cleanInput = string.IsNullOrEmpty(star) ? prefixlesInput : this.StringTake(prefixlesInput, prefixlesInput.Length - 1);
            string reversedCleanInput = StringReverse(cleanInput);
            if (!reversedCleanInput.All(c => char.IsDigit(c)))
            {
                // Zero accuracy case => hh:mm:ss
                if (!reversedCleanInput.Contains('.'))
                {
                    if (!reversedCleanInput.Contains(':'))
                        throw new ArgumentException($"Invalid input, minimal zero accuracy time string must be have at least zero minutes (0:ss).");

                    // [ss, mm, hh] || [ss,mm]
                    string[] zeroAccuracySplitReversedCleanInput =
                        reversedCleanInput.Split(':', 3);

                    if (!zeroAccuracySplitReversedCleanInput.All(x => x.All(c => char.IsDigit(c))))
                        throw new ArgumentException($"Non digit character found in '{input}', these are not allowed between the zero accuracy separator (':').");

                    if (zeroAccuracySplitReversedCleanInput.All(x => x.All(c => c == '0')))
                        throw new ArgumentException("All zeros (0) time strings are not valid.");

                    if (zeroAccuracySplitReversedCleanInput.Any(x => x.Length > 2))
                        throw new ArgumentException($"Invalid digit count in input: {input}");

                    if (!zeroAccuracySplitReversedCleanInput.Take(zeroAccuracySplitReversedCleanInput.Length - 1).All(x => x.Length == 2))
                        throw new ArgumentException($"Intermediate units (':xx:') must always consist of 2 digits.");

                    switch (zeroAccuracySplitReversedCleanInput.Length)
                    {
                        case 2:
                            {
                                string seconds = this.StringReverse(zeroAccuracySplitReversedCleanInput.First());
                                if (!this.StringIntInRange(seconds, 0, 59))
                                    throw new ArgumentException("Minutes are out of range, valid range in this context is 0 to 59.");

                                string minutes = this.StringReverse(zeroAccuracySplitReversedCleanInput[1]);
                                if (!this.StringIntInRange(minutes, 0, 59))
                                    throw new ArgumentException("Minutes are out of range, valid range in this context is 0 to 59.");

                                return true;
                            }
                        case 3:
                            {
                                string seconds = this.StringReverse(zeroAccuracySplitReversedCleanInput.First());
                                if (!this.StringIntInRange(seconds, 0, 59))
                                    throw new ArgumentException("Minutes are out of range, valid range in this context is 0 to 59.");

                                string minutes = this.StringReverse(zeroAccuracySplitReversedCleanInput[1]);
                                if (!this.StringIntInRange(minutes, 0, 59))
                                    throw new ArgumentException("Minutes are out of range, valid range in this context is 0 to 59.");

                                string hours = this.StringReverse(zeroAccuracySplitReversedCleanInput[2]);
                                if (!this.StringIntInRange(minutes, 1, 23))
                                    throw new ArgumentException("Minutes are out of range, valid range in this context is 1 to 23.");

                                if (
                                    hours.Length == 2
                                    && hours[0] == '0'
                                    )
                                    throw new ArgumentException($"Zero (0) padding '{hours}' is not allowed.");

                                return true;
                            }
                    }
                }

                // Accuracy of zero is also valid!
                string[] splitReversedCleanInput = this.SplitTimeString(reversedCleanInput);

                if (splitReversedCleanInput.Any(x => string.IsNullOrEmpty(x)))
                    throw new ArgumentException($"Value missing in '{input}', double seperation (.. or ::) is not allowed.");

                if (splitReversedCleanInput.All(x => x.All(c => c == '0')))
                    throw new ArgumentException("All zeros (0) time strings are not valid.");

                if (!splitReversedCleanInput.All(x => x.All(c => char.IsDigit(c))))
                    throw new ArgumentException($"Non digit character found in '{input}', these are not allowed between separators ('.',':').");

                if (splitReversedCleanInput.Last().Length > 3)
                    throw new ArgumentException("Millisecond accuracy must be at most 3 decimal places.");

                if(splitReversedCleanInput.Reverse().Skip(1).Any(x => x.Length > 2))
                    throw new ArgumentException($"Invalid digit count in input: {input}");

                if (!splitReversedCleanInput.Skip(1).Take(splitReversedCleanInput.Length - 2).All(x => x.Length == 2))
                    throw new ArgumentException($"Intermediate units (':xx(:/.)') must always consist of 2 digits.");

                string firstUnit = this.StringReverse(splitReversedCleanInput.First());
                if (
                   firstUnit.Length == 2
                   && firstUnit.First() == '0' // 0... => Is not allowed
                   )
                    throw new ArgumentException($"Zero (0) padding '{firstUnit}' is not allowed.");

                switch (splitReversedCleanInput.Length)
                {
                    // 1 is not possible here
                    case 2:
                        {
                            string seconds = this.StringReverse(splitReversedCleanInput.First());
                            string milliSeconds = this.StringReverse(splitReversedCleanInput[1]);

                            if (!this.StringIntInRange(seconds, 0, 59))
                                throw new ArgumentException("Seconds are out of range, valid range is 0 to 59.");

                            return true;
                        }
                    case 3:
                        {
                            string minutes = this.StringReverse(splitReversedCleanInput.First());
                            if (!this.StringIntInRange(minutes, 1, 59))
                                throw new ArgumentException("Minutes are out of range, valid range in this context is 1 to 59.");

                            string seconds = this.StringReverse(splitReversedCleanInput[1]);
                            if (!this.StringIntInRange(seconds, 0, 59))
                                throw new ArgumentException("Seconds are out of range, valid range is 0 to 59.");

                            string milliSeconds = this.StringReverse(splitReversedCleanInput[2]);

                            return true;
                        }
                    case 4:
                        {
                            string hours = this.StringReverse(splitReversedCleanInput.First());
                            if (!this.StringIntInRange(hours, 1, 23))
                                throw new ArgumentException("Hours are out of range, valid range is 1 to 23.");

                            string minutes = this.StringReverse(splitReversedCleanInput[1]);
                            if (!this.StringIntInRange(minutes, 0, 59))
                                throw new ArgumentException("Minutes are out of range, valid range in this context is 0 to 59.");

                            string seconds = this.StringReverse(splitReversedCleanInput[2]);
                            if (!this.StringIntInRange(seconds, 0, 59))
                                throw new ArgumentException("Seconds are out of range, valid range is 0 to 59.");

                            string milliSeconds = this.StringReverse(splitReversedCleanInput[3]);

                            return true;
                        }
                }
            }

            return false;
        }

        private string[] SplitTimeString(string reversedCleanInput)
        {
            // ms.ss:mm:hh
            // [ms, (ss:mm:hh)]
            var milliSecondsSplit = reversedCleanInput.Split('.', 2);
            if (milliSecondsSplit[1].Contains(':'))
            {
                // [ss, (mm:hh)]
                var secondSplit = milliSecondsSplit[1].Split(':', 2);
                if (secondSplit[1].Contains(':'))
                {
                    // [mm,hh]
                    var minuteSplit = secondSplit[1].Split(':');
                    
                    // [h,mm,ss,ms]
                    string[] h_m_s_ms = new string[4];
                    h_m_s_ms[0] = minuteSplit[1];
                    h_m_s_ms[1] = minuteSplit[0];
                    h_m_s_ms[2] = secondSplit[0];
                    h_m_s_ms[3] = milliSecondsSplit[0];
                    return h_m_s_ms;
                }

                // [mm,ss,ms]
                string[] m_s_ms = new string[3];
                m_s_ms[0] = secondSplit[1];
                m_s_ms[1] = secondSplit[0];
                m_s_ms[2] = milliSecondsSplit[0];
                return m_s_ms;
            }

            // [s,ms]
            string[] s_ms = new string[2];
            s_ms[0] = milliSecondsSplit[1];
            s_ms[1] = milliSecondsSplit[0];
            return s_ms;
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
                    throw new ArgumentException($"Invalid character '{reversedCleanInput[0]}' in input."); // This should not be possible, but still...
            }

            baselessCleanInput = this.StringSkip(reversedCleanInput, 2);
            return StringTake(reversedCleanInput, 2);
        }
    }
}
