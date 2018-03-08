using Dapper;
using Models.LotteryLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace LotteryLibrary
{
    public class NumbersInLine
    {

        private static List<ResultLineModel> allLines;


        /// <summary>
        /// this orders the allLines List<ResultLineModel> 
        /// </summary>
        /// <param name="l"></param>
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
        /// this looks for instances where main numbers and star numbers are the same
        /// </summary>
        /// <param name="rl"></param>
        /// <returns></returns>
        public static string FindInLineNumbers(List<ResultLineModel> rl)
        {

            List<InLineNumbersModel> il = new List<InLineNumbersModel>();
            List<string> lines = new List<string>();
            IndexAllLines(rl);

            ///this load if if statements are needed because switch requires constant values ( thats shite the good old Select case statement did not.)
            ///the idea is to record instances where the main draw numbers are repeted in the star number draw.
            ///knowing which col the number was in and which star it was will be good for later analysis.
            ///the fact that both star bumbers could match the main numbers complicates things.
            ///right now my idea is to find matches between col and star numbers then use the ResultLine index to find where both numbers matched.
            ///


            //RelationshipType = 1 is (1 col number = 1 star number)
            foreach (ResultLineModel r in allLines)
            {

                if (r.Star1 == r.Col1)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star1 = r.Star1,
                        Col1 = r.Col1,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);
                }

                if (r.Star1 == r.Col2)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star1 = r.Star1,
                        Col2 = r.Col2,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);
                }

                if (r.Star1 == r.Col3)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star1 = r.Star1,
                        Col3 = r.Col3,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

                if (r.Star1 == r.Col4)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star1 = r.Star1,
                        Col4 = r.Col4,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

                if (r.Star1 == r.Col5)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star1 = r.Star1,
                        Col5 = r.Col5,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

                if (r.Star2 == r.Col1)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star2 = r.Star2,
                        Col1 = r.Col1,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

                if (r.Star2 == r.Col2)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star2 = r.Star2,
                        Col2 = r.Col2,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

                if (r.Star2 == r.Col3)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star2 = r.Star2,
                        Col3 = r.Col3,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

                if (r.Star2 == r.Col4)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star2 = r.Star2,
                        Col4 = r.Col4,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }
                if (r.Star2 == r.Col5)
                {
                    InLineNumbersModel i = new InLineNumbersModel
                    {
                        Star2 = r.Star2,
                        Col5 = r.Col5,
                        ResultsLineIndex = r.ResultIndex,
                        DrawDate = r.DrawDate,
                        RelationshipType = 1
                    };
                    il.Add(i);

                }

            }// this bit kicks off looking for draws where both star numbers came out in the main draw
            //it gets back a list called addBothStars which is added to the mainlist.
            List<InLineNumbersModel> addBothStars = new List<InLineNumbersModel>();
            addBothStars = LookForDoubles(il);
            il.AddRange(addBothStars);

            /// This bit kickes off looking for sequential draw numbers 
            //it gets back a list called addSequences which is added to the mainlist.
            List<InLineNumbersModel> addSequences = new List<InLineNumbersModel>();
            addSequences = LookforSquences();
            il.AddRange(addSequences);


            /// this bit kicks off making textFiles
            ListToTextFile(il);

            // this reports back to the form
            return ($"{il.Count} found ");

        }



        /// <summary>
        /// this bit looks for draws where both star numbers were in the main number draw it then passes the list back to be added to the list.
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public static List<InLineNumbersModel> LookForDoubles(List<InLineNumbersModel> il)
        {

            List<InLineNumbersModel> dl = new List<InLineNumbersModel>();

            foreach (InLineNumbersModel m1 in il)
            {
                List<InLineNumbersModel> q = il;
                q = il.Where(x => x.ResultsLineIndex == m1.ResultsLineIndex && x.Star1 != m1.Star1).ToList();

                foreach (InLineNumbersModel qm in q)
                {
                    if (q.Count > 0)
                    {
                        InLineNumbersModel found2 = new InLineNumbersModel();

                        found2.RelationshipType = 2;
                        found2.ResultsLineIndex = qm.ResultsLineIndex;
                        found2.DrawDate = qm.DrawDate;

                        if (qm.Col1 != 0)
                        {
                            found2.Col1 = qm.Col1;
                        }
                        else
                        {
                            found2.Col1 = m1.Col1;
                        }

                        if (qm.Col2 != 0)
                        {
                            found2.Col2 = qm.Col2;
                        }
                        else
                        {
                            found2.Col2 = m1.Col2;
                        }

                        if (qm.Col3 != 0)
                        {
                            found2.Col3 = qm.Col3;
                        }
                        else
                        {
                            found2.Col3 = m1.Col3;
                        }

                        if (qm.Col4 != 0)
                        {
                            found2.Col4 = qm.Col4;
                        }
                        else
                        {
                            found2.Col4 = m1.Col4;
                        }

                        if (qm.Col5 != 0)
                        {
                            found2.Col5 = qm.Col5;
                        }
                        else
                        {
                            found2.Col5 = m1.Col5;
                        }
                        if (qm.Star1 != 0)
                        {
                            found2.Star1 = qm.Star1;
                        }
                        else
                        {
                            found2.Star1 = m1.Star1;
                        }

                        if (qm.Star2 != 0)
                        {
                            found2.Star2 = qm.Star2;
                        }
                        else
                        {
                            found2.Star2 = m1.Star2;
                        }


                        dl.Add(found2);

                    }
                }



            }


            return dl;
        }

        /// <summary>
        /// this looks for sequential numbers in a draw. See thhe InLineNumbersModel class for explination of the RelationshipType
        /// </summary>
        /// <returns></returns>
        public static List<InLineNumbersModel> LookforSquences()
        {
            List<InLineNumbersModel> sl = new List<InLineNumbersModel>();

            foreach (ResultLineModel r in allLines)
            {
                int x = 0;

                for (x = 1; x <= 5; x++)
                {
                    if (r.Col1 + x == r.Col2)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 10 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col1 = r.Col1,
                            Col2 = r.Col2
                        };
                        sl.Add(s);
                    }

                    if (r.Col2 + x == r.Col3)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 20 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col2 = r.Col2,
                            Col3 = r.Col3
                        };
                        sl.Add(s);
                    }

                    if (r.Col3 + x == r.Col4)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 30 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col3 = r.Col3,
                            Col4 = r.Col4
                        };
                        sl.Add(s);
                    }

                    if (r.Col4 + x == r.Col5)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 40 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col4 = r.Col4,
                            Col5 = r.Col5
                        };
                        sl.Add(s);
                    }

                    if (r.Star1 + x == r.Star1)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 50 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Star1 = r.Star1,
                            Star2 = r.Star2
                        };
                        sl.Add(s);
                    }

                    if (r.Col1 + x == r.Col2 && r.Col2 + x == r.Col3)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 60 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col1 = r.Col1,
                            Col2 = r.Col2,
                            Col3 = r.Col3
                        };
                        sl.Add(s);
                    }

                    if (r.Col2 + x == r.Col3 && r.Col3 + x == r.Col4)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 70 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col2 = r.Col2,
                            Col3 = r.Col3,
                            Col4 = r.Col4
                        };
                        sl.Add(s);

                    }

                    if (r.Col3 + x == r.Col4 && r.Col4 + x == r.Col5)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 80 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col3 = r.Col3,
                            Col4 = r.Col4,
                            Col5 = r.Col5
                        };
                        sl.Add(s);

                    }

                    if (r.Col1 + x == r.Col2 && r.Col2 + x == r.Col3 && r.Col3 + x == r.Col4)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 90 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col1 = r.Col1,
                            Col2 = r.Col2,
                            Col3 = r.Col3,
                            Col4 = r.Col4
                        };
                        sl.Add(s);
                    }

                    if (r.Col2 + x == r.Col3 && r.Col3 + x == r.Col4 && r.Col4 + x == r.Col5)
                    {
                        InLineNumbersModel s = new InLineNumbersModel
                        {
                            RelationshipType = 100 + x,
                            ResultsLineIndex = r.ResultIndex,
                            DrawDate = r.DrawDate,
                            Col1 = r.Col1,
                            Col2 = r.Col2,
                            Col3 = r.Col3,
                            Col4 = r.Col4,
                            Col5 = r.Col5
                        };
                        sl.Add(s);
                    }



                }

            }
            return sl;
        }

        /// <summary>
        /// this bit prints the data to textFile "ColToStar.csv"
        /// </summary>
        /// <param name="il"></param>
        private static void ListToTextFile(List<InLineNumbersModel> il)
        {
            List<string> lines = new List<string>();

            foreach (InLineNumbersModel l in il)
            {
                lines.Add($"{l.RelationshipType}," +
                    $"{l.ResultsLineIndex},{l.DrawDate}," +
                    $"{l.Col1},{l.Col2},{l.Col3},{l.Col4},{l.Col5},{l.Star1},{l.Star2}");
            }

            File.WriteAllLines("NumbersInLine.csv".FullFilePath(), lines);
        }



    }
}
