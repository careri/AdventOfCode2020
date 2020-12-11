using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2
{
    class Program
    {
        static void Main(string[] args)
        {
            var matches = Setup();
            var count = args?.Length == 0 || args[0] != "2" ?
                AoC2_1(matches) : AoC2_2(matches);
            Console.WriteLine($"Valid: {count} / {matches.Length}");
        }

        private static int AoC2_1(Match[] matches)
        {
            Func<Match, Regex> policyFactory = m => new Regex(m.Groups["char"].Value);
            Func<Match, MatchCollection, bool> policyCheck = (m, pm) =>
                pm.Count >= int.Parse(m.Groups["low"].Value) && pm.Count <= int.Parse(m.Groups["high"].Value);
            return CheckPolicy(matches, policyFactory, policyCheck);
        }

        private static int AoC2_2(Match[] matches)
        {
            Func<Match, Regex> policyFactory = m => new Regex(m.Groups["char"].Value);
            Func<Match, MatchCollection, bool> policyCheck = (m, pm) =>
            {
                var indices = pm.Select(_ => _.Index + 1).ToHashSet();
                return indices.Contains(int.Parse(m.Groups["low"].Value)) ^ indices.Contains(int.Parse(m.Groups["high"].Value));
            };
            return CheckPolicy(matches, policyFactory, policyCheck);
        }

        private static int CheckPolicy(
            Match[] matches, Func<Match, Regex> policyFactory, Func<Match, MatchCollection, bool> policyCheck)
        {
            Func<Match, Func<Match, Regex>, Func<Match, MatchCollection, bool>, bool> filter = (m, f, c) =>
            {
                if (!m.Success)
                {
                    throw new ApplicationException("Fail");
                }
                var policyRegex = f(m);
                var policyMatches = policyRegex.Matches(m.Groups["input"].Value);
                return c(m, policyMatches);
            };
            return matches.Where(_ => filter(_, policyFactory, policyCheck)).Count();
        }

        private static Match[] Setup()
        {
            var regex = new Regex(@"^(?<low>\d+)-(?<high>\d+)\s(?<char>\w):\s(?<input>.*)$");
            var matches = File.ReadAllLines("input.txt")
                .Select(_ => regex.Match(_)).ToArray();
            return matches;
        }
    }
}
