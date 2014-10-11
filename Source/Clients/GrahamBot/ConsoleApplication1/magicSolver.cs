using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class magicSolver
    {
        public string GetResult(string input){
            string[] lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int numberOfTestCases = ParseLineToSingleInt(lines[0]);
            List<string> results = new List<string>();
            for (int i = 0; i < numberOfTestCases; i++)
            {
                int offSet = 1 + (i*5);
                int firstGuess = ParseLineToSingleInt(lines[offSet]);
                int secondGuess = ParseLineToSingleInt(lines[offSet+5]);
                IEnumerable<int> firstGuessRow = GetRowInts(lines[offSet + firstGuess]);
                IEnumerable<int> secondGuessRow = GetRowInts(lines[offSet +5 + secondGuess]);
                string cheated = "Volunteer cheated!";
                string badMagician = "Bad magician!";
                int possibleAnswers = 0;
                for (int j = 1; j < 17; j++)
                {
                    if (firstGuessRow.Contains(j) && secondGuessRow.Contains(j))
                        possibleAnswers++;
                }
                string answer;
                if (possibleAnswers > 1) answer = badMagician;
                else if (possibleAnswers == 0) answer = cheated;
                else answer = firstGuessRow.FirstOrDefault(a => secondGuessRow.Contains(a)).ToString();
                results.Add("Case #" + i + ": " + answer);
            }
            return string.Join(Environment.NewLine, results);
        }

        private int ParseLineToSingleInt(string line){
            int result;
            Int32.TryParse(line, out result);
            return result;
        }

        IEnumerable<int> GetRowInts(string row)
        {
            string[] strings = row.Split(' ');
            return strings.Select(s => ParseLineToSingleInt(s));
        }

    }
}
