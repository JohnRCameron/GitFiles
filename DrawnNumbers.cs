using LotteryLibrary.Models;
using Models.LotteryLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LotteryLibrary
{
    public class DrawnNumbers
    {
        /// <summary>
        /// This works out how many times numbers have came out in the draw and puts the data into a csv textfile "RepeatNumbersAnalysisFile"
        /// file format Number,col1,col2,col3,col4,col5,numberTotal,numberPercent,star1,star2,starTotal,starPercent.
        /// the percentage is rounded down, but its good enough for now ==((totalQty / foundQty) * 100)
        /// </summary>
        /// <param name="allLines"></param>
        /// TODO -  "AnalyseDrawnNumbers" I really need to make a model to hold this data when I get to using the findings.
        public static string AnalyseDrawnNumbers(List<ResultLineModel> allLines)
        {
            string fileName = GlobalConfig.DrawnNumbersAnalysisFile;
            List<string> lines = new List<string>();

            /// this runs through numbers 1 to 50 and looks for results when the number was drawn, then adds them all up.
            for (int i = 1; i < 51; i++)
            {
                /// these hold the count during the loop.
                int c1 = 0; int c2 = 0; int c3 = 0; int c4 = 0; int c5 = 0;
                int s1 = 0; int s2 = 0;
                Double numberTotal = 0;
                Double numberPercent = 0;
                Double starTotal = 0;
                Double starPercent = 0;

                foreach (ResultLineModel l in allLines)
                {
                    if (l.Col1 == i)
                    {
                        c1++;
                    }
                    if (l.Col2 == i)
                    {
                        c2++;
                    }
                    if (l.Col3 == i)
                    {
                        c3++;
                    }
                    if (l.Col4 == i)
                    {
                        c4++;
                    }
                    if (l.Col5 == i)
                    {
                        c5++;
                    }
                    if (l.Star1 == i)
                    {
                        s1++;
                    }
                    if (l.Star2 == i)
                    {
                        s2++;
                    }
                }

                numberTotal = c1 + c2 + c3 + c4 + c5;
                numberPercent = (numberTotal / allLines.Count) * 100;
                numberPercent = Math.Round(numberPercent);
                starTotal = s1 + s2;
                starPercent = (starTotal / allLines.Count) * 100;
                starPercent = Math.Round(starPercent);
                lines.Add($"{i},{c1},{c2},{c3},{c4},{c5},{numberTotal},{numberPercent},{s1},{s2},{starTotal},{starPercent}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
            string output =($"{allLines.Count} processed");
            return output;

            /// TODO - AnalyseDrawnNumbers(List<ResultLineModel> allLines), make a Model for this so the results cam be sent to the user form
        }
    }
}

