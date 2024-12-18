using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class CustomMath
    {

        public static uint CalculateFilteredMean(List<uint> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            // 1. 정렬
            var sortedValues = values.OrderBy(x => x).ToList();

            // 2. 특이점 제거 (IQR 방법)
            uint q1 = Percentile(sortedValues, 25); // 1사분위수
            uint q3 = Percentile(sortedValues, 75); // 3사분위수
            uint iqr = q3 - q1; // IQR 계산
            uint lowerBound = (uint)Math.Max(0, q1 - 1.5 * iqr);
            uint upperBound = q3 + (uint)(1.5 * iqr);

            var filteredValues = sortedValues.Where(x => x >= lowerBound && x <= upperBound).ToList();

            if (filteredValues.Count == 0)
                return 0; // 특이점 제거 후 리스트가 비어 있는 경우

            // 3. 중간값 중심 평균 계산
            uint median = Median(filteredValues);
            int midIndex = filteredValues.Count / 2;

            if (filteredValues.Count % 2 == 0)
            {
                // 짝수일 경우 중간값 2개의 평균 계산
                return (filteredValues[midIndex - 1] + filteredValues[midIndex]) / 2;
            }
            else
            {
                // 홀수일 경우 중간값
                return filteredValues[midIndex];
            }
        }

        internal static uint Min(List<uint> rTTs)
        {
            if (rTTs == null || rTTs.Count == 1)
            {
                //throw new ArgumentException("List must contain at least two elements.");
                return rTTs[0];

            }

            uint min1 = uint.MaxValue; // 가장 작은 값
            uint min2 = uint.MaxValue; // 두 번째로 작은 값

            foreach (var value in rTTs)
            {
                if (value < min1)
                {
                    min2 = min1;
                    min1 = value;
                }
                else if (value < min2)
                {
                    min2 = value;
                }
            }

            return (min1 + min2) / 2;
        }

        static uint Median(List<uint> values)
        {
            int count = values.Count;
            if (count % 2 == 0)
            {
                return (values[count / 2 - 1] + values[count / 2]) / 2;
            }
            else
            {
                return values[count / 2];
            }
        }

        static uint Percentile(List<uint> sortedValues, double percentile)
        {
            int N = sortedValues.Count;
            double rank = (percentile / 100.0) * (N - 1);
            int lowerIndex = (int)Math.Floor(rank);
            int upperIndex = (int)Math.Ceiling(rank);

            if (lowerIndex == upperIndex)
            {
                return sortedValues[lowerIndex];
            }

            double weight = rank - lowerIndex;
            return (uint)(sortedValues[lowerIndex] * (1 - weight) + sortedValues[upperIndex] * weight);
        }

    }
}
