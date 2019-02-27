using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace project2
{
    public class SportData
    {
        private static void main(string[] args)
        {
            string filePath = Directory.GetCurrentDirectory();
            string starter = Directory.GetParent(filePath).ToString();
            string mid = Directory.GetParent(filePath).ToString();
            string last = Directory.GetParent(filePath).ToString();
            Console.WriteLine(last);

            if (File.Exists(filePath))
            {
                List<SuperBowl> sbList = new List<SuperBowl>();

                FileStream CSVFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(CSVFile);

                var parentDirectory = Directory.GetParent(filePath);
                Console.WriteLine("Enter a name for the new .txt file: ");
                string fileName = Console.ReadLine();

                string newFilePath = parentDirectory + "/" + fileName + ".txt";

                FileStream txtFile = new FileStream(newFilePath, FileMode.Append, FileAccess.Write);
                StreamWriter writer = new StreamWriter(txtFile);

                string row = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    row = reader.ReadLine();
                    string[] superBowl = row.Split(",");
                    SuperBowl bigSuperBowl = new SuperBowl(){
                             date = superBowl[0],
                             sb = superBowl[1],
                             attendance = Convert.ToInt32(superBowl[2]),
                             winnerQB = superBowl[3],
                             winnerCoach = superBowl[4],
                             winner = superBowl[5],
                             winnerPoints = Convert.ToInt32(superBowl[6]),
                             loserQB = superBowl[7],
                             loserCoach = superBowl[8],
                             loser = superBowl[9],
                             loserPoints = Convert.ToInt32(superBowl[10]),
                             MVP = superBowl[11],
                             stadium = superBowl[12],
                             city = superBowl[13],
                             state = superBowl[14]
                        
                        };
                    sbList.Add(bigSuperBowl);
                }

                writer.WriteLine("Super Bowl Winners:");
                string sbNum = "SB";
                string sbWinner = "Winner";
                string sbLoser = "Loser";
                string sbYear = "Year";
                string sbQuarterBack = "Quarter Back";
                string sbCoachOne = "Coach";
                string sbMVP = "Most Valuable Player";
                string sbPointDif = "Point Difference";
                string sbCity = "City";
                string sbState = "State";
                string sbStadium = "Stadium";

                writer.WriteLine("{0,-8}{1,-21}{2,-5}{3,-27}{4,-16}{5,-26}{6,-7}", sbNum, sbWinner, sbYear, sbQuarterBack, sbCoachOne, sbMVP, sbPointDif);
                writer.WriteLine(new string('-', 110));
                foreach (SuperBowl winner in sbList)
                {
                    double ptDifference = winner.winnerPoints - winner.loserPoints;
                    writer.WriteLine($"{winner.sb,-8}{winner.winner,-20} '{winner.date.Substring(winner.date.Length - 2),-3} {winner.winnerQB,-26} {winner.winnerCoach,-15} {winner.MVP,-25} {ptDifference,-7}");
                }

                var topAttendance = (from superBowl in sbList
                                     orderby superBowl.attendance descending
                                     select superBowl).Take(5);

                writer.WriteLine("Top 5 attended super bowls:");
                writer.WriteLine("{0,-5}{1,-20}{2,-20}{3,-10}{4,-11}{5,-16}", sbYear, sbWinner, sbLoser, sbCity, sbState, sbStadium);
                writer.WriteLine(new string('-', 81));

                foreach (SuperBowl superBowl in topAttendance)
                {
                    writer.WriteLine("'{0,-4}{1,-20}{2,-20}{3,-10}{4,-11}{5,-16}", superBowl.date.Substring(superBowl.date.Length - 2), superBowl.winner, superBowl.loser, superBowl.city, superBowl.state, superBowl.stadium);
                }

                var HostingState = sbList
                    .GroupBy(state => state.state)
                    .OrderByDescending(group => group.Count())
                    .First().Key;

                var HostingCity = sbList
                    .GroupBy(city => city.city)
                    .OrderByDescending(group => group.Count())
                    .First().Key;

                var HostingStadium = sbList
                    .GroupBy(stadium => stadium.stadium)
                    .OrderByDescending(group => group.Count())
                    .First().Key;

                writer.WriteLine("The state that hosted the most super bowl games:\n\t{0}\n" +
                                  "The city that hosted the most super bowl games:\n\t{1}\n" +
                                  "The stadium that hosted the most super bowl games:\n\t{2}",
                                  HostingState, HostingCity, HostingStadium);


                writer.WriteLine("These are the players who have won MVP multiple times: ");
                var mvpGroup = from superBowl in sbList
                               group superBowl by superBowl.MVP into mvps
                               where mvps.Count() > 1
                               orderby mvps.Key
                               select mvps;

                foreach (var player in mvpGroup)
                {
                    writer.WriteLine(player.Key);
                    foreach (var mvp in player)
                    {
                        writer.WriteLine("\tWon with {0} and beat {1}", mvp.winner, mvp.loser);
                    }
                }


                var coachLosses = sbList
                    .GroupBy(coach => coach.loserCoach)
                    .OrderByDescending(coach => coach.Count())
                    .First().Key;

                writer.WriteLine("Which coach lost the most super bowls?\n\t{0}", coachLosses);


                var coachWins = sbList
                    .GroupBy(coach => coach.winnerCoach)
                    .OrderByDescending(coach => coach.Count())
                    .First().Key;

                writer.WriteLine("Which coach won the most super bowls?\n\t{0}", coachWins);


                var teamWins = sbList
                    .GroupBy(team => team.winner)
                    .OrderByDescending(team => team.Count())
                    .First().Key;

                writer.WriteLine("Which team has won the most super bowls?\n\t{0}", teamWins);


                var teamLosses = sbList
                    .GroupBy(team => team.loser)
                    .OrderByDescending(team => team.Count())
                    .First().Key;

                writer.WriteLine("Which team lost the most super bowls?\n\t{0}", teamLosses);


                var ptDiffQ = from superBowl in sbList
                              orderby (superBowl.winnerPoints - superBowl.loserPoints) descending
                              select superBowl;

                writer.WriteLine("Which super bowl had the greatest point difference?\n\tSuper bowl {0}", ptDiffQ.First().sb);

                int maxAttendance = 0;
                foreach (SuperBowl superBowl in sbList)
                {
                    maxAttendance = maxAttendance + superBowl.attendance;
                }
                int averageAttendance = maxAttendance / sbList.Count();

                writer.WriteLine("What is the average attendance of all the super bowls? \n\t{0} people?", averageAttendance);

                CSVFile.Close();
                reader.Close();
                txtFile.Close();
            }
            else
            {
                Console.WriteLine("There is no file by that name... please try again!");
            }
            Console.WriteLine("Please check the directory of file, 'Super_Bowl_Project.csv'.Then, you may press <ENTER> to quit.");
            Console.ReadLine();
        }
    }


}