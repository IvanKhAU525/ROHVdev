using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace NotificationProcessor
{
    public static class CronJob                                                             //  | SEC | MIN | HOUR | DAY OF MONTH | MONTH | DAY OF WEEK | YEAR  | 
    {                                                                                       //  |_______________________________________________________________|
        private static readonly string _everyWeek = "{0} {1} {2} ? * {3} {4}/1";            //  | {0} | {1} | {2}  |      ?       |   *   |     {3}     | {4}/1 | 
        private static readonly string _everyMonth = "{0} {1} {2} {3} {4}/1 ? {5}/1";       //  | {0} | {1} | {2}  |     {3}      | {4}/1 |      ?      | {5}/1 |
        private static readonly string _every6Month = "{0} {1} {2} {3} {4}/6 ? {5}/1";      //  | {0} | {1} | {2}  |     {3}      | {4}/6 |      ?      | {5}/1 |
        private static readonly string _everyYear = "{0} {1} {2} {3} {4} ? {5}/1";          //  | {0} | {1} | {2}  |     {3}      |  {4}  |      ?      | {5}/1 |
        private static readonly string _everyMinute = "0/10 * * * * ?";                      //   ---------------------------------------------------------------
                                                                                            
        private static string GetEveryWeekCron(DateTime startDate) {
            return string.Format(_everyWeek,
                startDate.Second,   
                startDate.Minute,   
                startDate.Hour,     
                (int)startDate.DayOfWeek + 1,      
                startDate.Year      
                );
        }

        private static string GetEveryMonthCron(DateTime startDate) {
            return string.Format(_everyMonth,
                startDate.Second,
                startDate.Minute,
                startDate.Hour,
                startDate.Day,
                startDate.Month,
                startDate.Year
                );
        }

        private static string GetEvery6MonthCron(DateTime startDate) {
            return string.Format(_every6Month,
                startDate.Second,
                startDate.Minute,
                startDate.Hour,
                startDate.Day,
                startDate.Month,
                startDate.Year
                );
        }

        private static string GetEveryYearCron(DateTime startDate) {
            return string.Format(_everyYear,
                startDate.Second,
                startDate.Minute,
                startDate.Hour,
                startDate.Day,
                startDate.Month,
                startDate.Year
                );
        }

        private static string GetEveryMinuteCron() {
            return _everyMinute;
        }

        public static string GetCronExpression(Repeat repeatType, DateTime startDate) {
            switch (repeatType) {
                case Repeat.EveryWeek: return CronJob.GetEveryWeekCron(startDate);
                case Repeat.EveryMonth: return CronJob.GetEveryMonthCron(startDate);
                case Repeat.Every6Month: return CronJob.GetEvery6MonthCron(startDate);
                case Repeat.EveryYear: return CronJob.GetEveryYearCron(startDate);
                case Repeat.EveryMinute: return CronJob.GetEveryMinuteCron();
            }
            return null;
        }
    }

    public enum Repeat
    {
        Once = 1,
        EveryWeek = 2,
        EveryMonth = 3,
        Every6Month = 4,
        EveryYear = 5,
        EveryMinute = 6
    }
}
