using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode1
{
    class Program
    {
        static void Main(string[] args)
        {
            var values = new HashSet<int>();
            var wantedSum = 2020;
            File.ReadAllLines("input.txt")
                .Select(int.Parse).ToList()
                .ForEach(_ => values.Add(_));
            var arr = values.ToArray();
            int v1, v2, v3, s;
            v1 = v2 = v3 = -1;

            for (int i = 0; i < arr.Length - 1 && v3 == -1; i++)
            {
                v1 = arr[i];

                for (int j = i + 1; j < arr.Length - 1; j++)
                {
                    v2 = arr[j];
                    s = v1 + v2;

                    if (s < wantedSum)
                    {
                        v3 = wantedSum - s;

                        if (values.Contains(v3))
                        {
                            break;
                        }
                        v3 = -1;
                    } 
                }
            }
            Console.WriteLine($"{v1} * {v2} * {v3} = {v1 * v2 * v3}");
        }

        static void Main_Code1(string[] args)
        {
            var values = new HashSet<int>();
            var wantedSum = 2020;
            File.ReadAllLines("input.txt")
                .Select(int.Parse).ToList()
                .ForEach(_ => values.Add(_));
            var match = values.First(_ => values.Contains(wantedSum - _));
            var delta = 2020 - match;
            Console.WriteLine($"{match} * {delta} = {match * delta}");

        }
    }
}
