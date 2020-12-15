using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AoC2
{
    class Program
    {
        private static readonly CancellationTokenSource s_cancelSrc = new CancellationTokenSource();

        static void Main(string[] args)
        {
            //var match = GetStep(270, 4, 57, 1);
            Console.CancelKeyPress += (o, e) => s_cancelSrc.Cancel();
            var isStep2 = args?.Length == 0 || args[0] != "2";
            Task.Factory.StartNew(o => Run((bool)o), isStep2, TaskCreationOptions.LongRunning).Wait(s_cancelSrc.Token);

        }

        private static void Run(bool isStep2)
        {
            var busIds = Setup(out int departure);
            Console.WriteLine($"Departure: {departure}, busIds: {string.Join(",", busIds)}");

            var result = isStep2 ?
                AoC2_1(departure, busIds.Where(_ => _.HasValue).Select(_ => _.Value).ToArray()) :
                AoC2_2(busIds);
            Console.WriteLine($"Result: {result}");
        }

        private static long AoC2_1(int departure, int[] busIds)
        {
            var d = (long)departure;
            var busDepartures = from id in busIds
                                let next = GetNext(d, id)
                                let info = (Id: id, Next: next, Wait: next - departure)
                                orderby info.Wait
                                select info;
            var best = busDepartures.First();
            return best.Id * best.Wait;
        }

        private static long AoC2_2(int?[] busIds)
        {
            var busInfos = busIds
                .Select((busId, idx) => new BusInfo(busId, idx))
                .Where(_ => _.Id != BusInfo.InvalidId)
                .OrderByDescending(_ => _.Id)
                .ToArray();
            var largest = busInfos[0];
            var second = busInfos[1];
            var start = GetStartAndInterval(largest, second, out long step);
            var time = start - largest.Index;
            var sw = Stopwatch.StartNew();

            while (true)
            {
                s_cancelSrc.Token.ThrowIfCancellationRequested();

                if (busInfos.All(_ => _.IsInSchedule(time)))
                {
                    break;
                }
                time += step;

                if (sw.Elapsed.TotalSeconds > 1)
                {
                    Console.WriteLine(time);
                    sw.Restart();
                }
            }
            return time;
        }

        private static long ShowCommon(long start, (int Step, int Offset) serie1, (int Step, int Offset) serie2)
        {
            var s1 = serie1;
            var s2 = serie2;

            if (start > 0)
            {
                s1 = (Step: s1.Step, Offset: 0);
                s2 = (Step: s2.Step, Offset: 0);
            }

            var common = GetCommon(start, s1, s2);
            var m1 = (common - serie1.Offset) / serie1.Step;
            var m2 = (common - serie2.Offset) / serie2.Step;
            Console.WriteLine($"Common: {common}");
            Console.WriteLine($"{serie1.Step}*{m1}+{serie1.Offset}={serie1.Step * m1 + serie1.Offset}");
            Console.WriteLine($"{serie2.Step}*{m2}+{serie2.Offset}={serie2.Step * m2 + serie2.Offset}");
            return common;
        }

        private static long GetCommon(long start, (int Step, int Offset) serie1, (int Step, int Offset) serie2)
        {
            var s1 = start + serie1.Step + serie1.Offset;
            var s2 = start + serie2.Step + serie2.Offset;

            while (s1 != s2)
            {
                if (s1 < s2)
                    s1 += serie1.Step;
                else
                    s2 += serie2.Step;
            }
            return s1;
        }

        [DebuggerDisplay("{ToDebugString()}")]
        private struct BusInfo
        {
            public const int InvalidId = -1;

            public BusInfo(int? busId, int idx)
            {
                Id = busId.HasValue ? busId.Value : InvalidId;
                Index = idx;
            }

            public int Id { get; }
            public int Index { get; }

            public bool IsInSchedule(long time)
            {
                var result = time % Id == (Id - Index);

                if (result)
                {
                    Console.WriteLine($"{time} % {Index}) == ({Id} - {Index}), {result}");
                }
                return result;
            }

            public string ToDebugString() => $"{Id} @ {Index}";
        }

        public static long GetNext(long time, long busId)
        {
            var factor = (long)Math.Ceiling((double)time / busId);
            return factor * busId;
        }

        public static long GetPrevious(long time, long busId)
        {
            var factor = (long)Math.Floor((double)time / busId);
            return factor * busId;
        }


        private static int?[] Setup(out int departure)
        {
            var lines = File.ReadAllLines("input.txt");
            departure = int.Parse(lines[0].Trim());
            return lines[1].Split(',').Select(_ =>
            {
                if (int.TryParse(_, out int busId))
                {
                    return (int?)busId;
                }
                return null;
            }).ToArray();
        }
    }
}