using Dapper;
using LotteryLibrary.Models;
using Models.LotteryLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryLibrary
{
    public class RepeatNumbers
    {

        private static List<RepeatModel> allRepeats = new List<RepeatModel>();
        private static List<ResultLineModel> allLines;

        /// this bit sets up the allLines List<ResultLineModel> to order them in date order and add an index
        /// Thinking about this I am of the opinion that setting an index at the start is the best way to go.
        /// as the next date is an unknown, finding it by "Where > " is not the best option.
        private static void IndexAllLines(List<ResultLineModel> l)
        {
            allLines = l.OrderBy(x => x.DrawDate).ToList();
            int i = 1;
            foreach (ResultLineModel r in allLines)
            {
                r.ResultIndex = i;
                i++;
            }
        }

        /// <summary>
        /// this has been disabled till I figure out how to report back to the progress bar from here.
        /// TODO-Figure out how to report back to the progress bar from a class.
        /// Ths really does nothing with the List<ResultLineModel> but is needed to pas it to numbers in a row.
        /// This uses a loop to determine how many weeks ahead to look for the next draw of a number.
        /// </summary>
        /// <param name="rl"></param>
        /// <returns></returns>
        public static string SkipWeekInRow(List<ResultLineModel> rl)
        {
            string output = "";

            //for (int skipWeek = 1; skipWeek < 5; skipWeek++)
            //{
            //    output = NumberInRow(rl, skipWeek);
            //}
            return output;
        }

        public static string NumberInRow(List<ResultLineModel> rl, int skipWeek)
        {
            // this function is called from two buttons on the form (so far)
            //
            // the int skipWeek determines how many draws to skip while looking for repeat numbers.

            IndexAllLines(rl);

            foreach (ResultLineModel l in allLines)
            {

                // wish i could have got this working, but no chance... thats why i neede the i index :-(
                // int nextRecord = allLines.IndexOf(l) + 1;
                // List<ResultLineModel next = allLines.Where((allLines.IndexOf(l) == nextRecord)).ToString();

                /// this makes sure the records does not go out of range
                int index = allLines.IndexOf(l);
                if (index <= (allLines.Count - 2))
                {
                    /// this sends each number to the CheckFor2 and starts the checking process
                    CheckForTwo(l.Col1, 1, l.ResultIndex, l.DrawDate, skipWeek);
                    CheckForTwo(l.Col2, 1, l.ResultIndex, l.DrawDate, skipWeek);
                    CheckForTwo(l.Col3, 1, l.ResultIndex, l.DrawDate, skipWeek);
                    CheckForTwo(l.Col4, 1, l.ResultIndex, l.DrawDate, skipWeek);
                    CheckForTwo(l.Col5, 1, l.ResultIndex, l.DrawDate, skipWeek);

                    CheckForTwo(l.Star1, 2, l.ResultIndex, l.DrawDate, skipWeek);
                    CheckForTwo(l.Star2, 2, l.ResultIndex, l.DrawDate, skipWeek);

                }
            }
            /// this kicks off the textFile
            string output = RepeatsToTextFile(skipWeek);
            return output;
        }

        /// <summary>
        /// this checks for the second time in a row that a number comes out.
        /// note calling second repeat / third repeat gets relaay confusing, is a second repeat the third time a number comes out or is it the forth ?
        /// to hell with language second in a row is what is looked for here , dont care, its two in a row.
        /// </summary>
        /// <param name="searchNo"></param>
        /// <param name="numberType"></param>
        /// <param name="resultIndex"></param>
        /// <param name="drawDate"></param>

        public static void CheckForTwo(int searchNo, int numberType, int resultIndex, DateTime drawDate, int skipWeek)
        {


            ResultLineModel n = new ResultLineModel();
            RepeatModel r = new RepeatModel();
            // this works out the next record to check
            int i = resultIndex + skipWeek;

            // this if statement  makes sure the query stays in range
            int c = allLines.Where(x => x.ResultIndex == i).Count();
            if (c > 0 && allLines.Count > (allLines.Count - i))
            {

                n = allLines.Where(x => x.ResultIndex == i).ToList().First();

                if (numberType == 1)
                {
                    //this loads main numbers to the RepeatModel
                    if (n.Col1 == searchNo || n.Col2 == searchNo || n.Col3 == searchNo || n.Col4 == searchNo || n.Col5 == searchNo)
                    {
                        r.ResultsLineIndex = n.ResultIndex;
                        r.DrawNumber = searchNo;
                        r.OrigionalDrawDate = drawDate;
                        r.MainNumberSecondRepeatDate = n.DrawDate;
                        r.MainNumberSecondRepeatIndicator = 2;
                        r.NumberTypeIndicator = numberType;
                        allRepeats.Add(r);
                        CheckForThree(r, skipWeek);
                    }
                }
                if (numberType == 2)
                {

                    if (n.Star1 == searchNo || n.Star2 == searchNo)
                    {
                        r.ResultsLineIndex = n.ResultIndex;
                        r.DrawNumber = searchNo;
                        r.OrigionalDrawDate = drawDate;
                        r.StarNumberSecondRepeatDate = n.DrawDate;
                        r.StarNumberSecondRepeatIndicator = 2;
                        r.NumberTypeIndicator = numberType;
                        allRepeats.Add(r);
                        CheckForThree(r, skipWeek);

                    }

                }
            }


        }

        /// <summary>
        ///  see CheckForTwo and extropliate :-)
        /// </summary>
        /// <param name="r"></param>
        private static void CheckForThree(RepeatModel r, int skipWeek)
        {
            ResultLineModel n = new ResultLineModel();

            // this works out the next record to check
            int i = r.ResultsLineIndex + skipWeek;

            // this if statement  makes sure the query stays in range
            int c = allLines.Where(x => x.ResultIndex == i).Count();
            if (c > 0 && allLines.Count > (allLines.Count - i))
            {

                n = allLines.Where(x => x.ResultIndex == i).ToList().First();

                if (r.NumberTypeIndicator == 1)
                {
                    if (n.Col1 == r.DrawNumber || n.Col2 == r.DrawNumber || n.Col3 == r.DrawNumber || n.Col4 == r.DrawNumber || n.Col5 == r.DrawNumber)
                    {
                        r.MainNumberThirdRepeatDate = n.DrawDate;
                        r.MainNumberThirdRepeatIndicator = 3;
                        CheckForFour(r, skipWeek);
                    }
                }
                if (r.NumberTypeIndicator == 2)
                {
                    if (n.Star1 == r.DrawNumber || n.Star2 == r.DrawNumber)
                    {
                        r.StarNumberThirdRepeatDate = n.DrawDate;
                        r.StarNumberThirdRepeatIndicator = 3;
                        CheckForFour(r, skipWeek);
                    }
                }

            }

        }

        /// <summary>
        ///  see CheckForTwo and extropliate :-)
        /// </summary>
        public static void CheckForFour(RepeatModel r, int skipWeek)
        {
            ResultLineModel n = new ResultLineModel();

            // this works out the next record to check
            int i = r.ResultsLineIndex + (skipWeek * 2);

            // this if statement  makes sure the query stays in range
            int c = allLines.Where(x => x.ResultIndex == i).Count();
            if (c > 0 && allLines.Count > (allLines.Count - i))
            {

                n = allLines.Where(x => x.ResultIndex == i).ToList().First();

                if (r.NumberTypeIndicator == 1)
                {
                    if (n.Col1 == r.DrawNumber || n.Col2 == r.DrawNumber || n.Col3 == r.DrawNumber || n.Col4 == r.DrawNumber || n.Col5 == r.DrawNumber)
                    {
                        r.MainNumberFourthRepeatDate = n.DrawDate;
                        r.MainNumberFourthRepeatIndicator = 4;
                        CheckForFive(r, skipWeek);
                    }
                }
                if (r.NumberTypeIndicator == 2)
                {
                    if (n.Star1 == r.DrawNumber || n.Star2 == r.DrawNumber)
                    {
                        r.StarNumberFourthRepeatDate = n.DrawDate;
                        r.StarNumberFourthRepeatIndicator = 4;
                        CheckForFive(r, skipWeek);
                    }
                }

            }

        }

        /// <summary>
        ///  see CheckForTwo and extropliate :-)
        /// </summary>
        public static void CheckForFive(RepeatModel r, int skipWeek)
        {
            ResultLineModel n = new ResultLineModel();

            // this works out the next record to check
            int i = r.ResultsLineIndex + (skipWeek * 3);

            // this if statement  makes sure the query stays in range
            int c = allLines.Where(x => x.ResultIndex == i).Count();
            if (c > 0 && allLines.Count > (allLines.Count - i))
            {
                n = allLines.Where(x => x.ResultIndex == i).ToList().First();

                if (r.NumberTypeIndicator == 1)
                {
                    if (n.Col1 == r.DrawNumber || n.Col2 == r.DrawNumber || n.Col3 == r.DrawNumber || n.Col4 == r.DrawNumber || n.Col5 == r.DrawNumber)
                    {
                        r.MainNumberFifthDrawDate = n.DrawDate;
                        r.MainNumberFifthRepeatIndicator = 5;
                    }
                }
                if (r.NumberTypeIndicator == 2)
                {
                    if (n.Star1 == r.DrawNumber || n.Star2 == r.DrawNumber)
                    {
                        r.StarNumberFifthRepeatDate = n.DrawDate;
                        r.StarNumberFifthRepeatIndicator = 5;
                    }
                }

            }

        }

        /// <summary>
        /// I was going to format the dates here but decided to leave them alone incase I need to use the textFile.
        /// </summary>

        public static string RepeatsToTextFile(int skipWeek)
        {
            /// this sorts the Reeat number data by draw number 
            allRepeats = allRepeats.OrderBy(x => x.NumberTypeIndicator).ToList();

            string fileName = "";
            List<string> lines = new List<string>();
            foreach (RepeatModel l in allRepeats)
            {

                if (l.NumberTypeIndicator == 1)
                {
                    switch (skipWeek)
                    {
                        case 1:
                            fileName = "MainRepeatOneWeekFile.csv";
                            break;
                        case 2:
                            fileName = "MainRepeatTwoWeekFile.csv";
                            break;
                        case 3:
                            fileName = "MainRepeatThreeWeekFile.csv";
                            break;
                        case 4:
                            fileName = "MainRepeatFourWeekFile.csv";
                            break;
                    }

                    lines.Add($"{l.DrawNumber}," +
                    $"{l.NumberTypeIndicator}," +
                    $"{l.OrigionalDrawDate}," +
                    $"{l.MainNumberSecondRepeatIndicator},{l.MainNumberSecondRepeatDate}," +
                    $"{l.MainNumberThirdRepeatIndicator},{l.MainNumberThirdRepeatDate}," +
                    $"{l.MainNumberFourthRepeatIndicator},{l.MainNumberFourthRepeatDate}," +
                    $"{l.MainNumberFifthRepeatIndicator},{l.MainNumberFifthDrawDate}");

                    File.WriteAllLines(fileName.FullFilePath(), lines);

                }

            }


            fileName = "";
            lines.Clear();

            foreach (RepeatModel l in allRepeats)
            {

                if (l.NumberTypeIndicator == 2)
                {
                    switch (skipWeek)
                    {
                        case 1:
                            fileName = "StarMainRepeatOneWeekFile.csv";
                            break;
                        case 2:
                            fileName = "StarMainRepeatTwoWeekFile.csv";
                            break;
                        case 3:
                            fileName = "StarMainRepeatThreeWeekFile.csv";
                            break;
                        case 4:
                            fileName = "StarMainRepeatFourWeekFile.csv";
                            break;
                    }

                    lines.Add($"{l.DrawNumber}," +
                    $"{l.NumberTypeIndicator}," +
                    $"{l.OrigionalDrawDate}," +
                    $"{l.StarNumberSecondRepeatIndicator},{l.StarNumberSecondRepeatDate}," +
                    $"{l.StarNumberThirdRepeatIndicator},{l.StarNumberThirdRepeatDate}," +
                    $"{l.StarNumberFourthRepeatIndicator},{l.StarNumberFourthRepeatDate}," +
                    $"{l.StarNumberFifthRepeatIndicator},{l.StarNumberFifthRepeatDate}");

                    File.WriteAllLines(fileName.FullFilePath(), lines);

                }

            }
            fileName = "";
            lines.Clear();


            ///this reports back to the results form, crap message I know but at least its an indication that the process is complete.
            string output = ($"{allRepeats.Count}  Found ");

            // this clears the List<RepeatModel> so I dont drive myself mad again wondering what is wrong !!!!
            allRepeats.Clear();
            return output;
        }


    }
}


