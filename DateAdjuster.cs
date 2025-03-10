using System;
using System.Collections.Generic;
using System.Linq;

public class DateAdjuster
{
    public static Tuple<string, string> SubtractQuarters(Tuple<string, string> quarterAndYear, int quartersToSubtract)
    {
        int year = int.Parse(quarterAndYear.Item2);
        int quarter = int.Parse(quarterAndYear.Item1.Substring(1));
        
        // Convert the current quarter/year into a zero-based quarter index.
        int totalQuarterIndex = year * 4 + (quarter - 1);
        int newTotalQuarterIndex = totalQuarterIndex - quartersToSubtract;
        
        int newYear = newTotalQuarterIndex / 4;
        int newQuarter = (newTotalQuarterIndex % 4) + 1;
        
        // Return the result as a tuple, formatting the quarter with a "Q" prefix.
        return Tuple.Create("Q" + newQuarter.ToString(), newYear.ToString());
    }


    public static DateTime QuarterStartDate(Tuple<string, string> quarter)
    {
        int quarterNumber = int.Parse(quarter.Item1.Substring(1)); // Extracts "4" from "Q4");
        int year = int.Parse(quarter.Item2);

        // Calculate the start month of the quarter.
        int startMonth = (quarterNumber - 1) * 3 + 1;

        // Return the start date of the quarter.
        return new DateTime(year, startMonth, 1);
    }

    public static Tuple<string, string> DateToQuarter(DateTime runDate) {

        // Get the year and quarter.
        int year = runDate.Year;
        int quarter = (runDate.Month - 1) / 3 + 1;

        // Return the year and quarter as a tuple.
        return Tuple.Create(year.ToString(), quarter.ToString());
    }
    /// <summary>
    /// Adjusts the provided date so that it falls on a trading day.
    /// Trading days are defined as weekdays that are not among the US market holidays from 2010-2030.
    /// If the date falls on a weekend, it is adjusted accordingly.
    /// Then, if the resulting date is a holiday, it is moved to the previous day until it isn’t.
    /// </summary>
    public static DateTime AdjustWeekendAndHolidays(DateTime date)
    {
        // First adjust for weekends.
        if (date.DayOfWeek == DayOfWeek.Saturday)
        {
            date = date.AddDays(-1);
        }
        else if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }

        // Collect holidays for the date's year. (If you wish to cover multi-year spans,
        // you can combine holidays from adjacent years as needed.)
        var holidays = GetUSMarketHolidays(date.Year);

        // Adjust for holidays: move backwards until the date is not a holiday.
        while (holidays.Contains(date) || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(-1);
        }

        return date;
    }

    /// <summary>
    /// Returns a HashSet of US stock market holidays for the given year.
    /// Includes:
    /// - New Year's Day (observed)
    /// - Martin Luther King Jr. Day (3rd Monday in January)
    /// - Presidents' Day (3rd Monday in February)
    /// - Good Friday (2 days before Easter Sunday)
    /// - Memorial Day (last Monday in May)
    /// - Independence Day (observed)
    /// - Labor Day (1st Monday in September)
    /// - Thanksgiving Day (4th Thursday in November)
    /// - Christmas Day (observed)
    /// </summary>
    public static HashSet<DateTime> GetUSMarketHolidays(int year)
    {
        var holidays = new HashSet<DateTime>();

        // New Year's Day
        holidays.Add(ObservedHoliday(new DateTime(year, 1, 1)));

        // Martin Luther King Jr. Day: 3rd Monday in January
        holidays.Add(GetNthWeekdayOfMonth(year, 1, DayOfWeek.Monday, 3));

        // Presidents' Day: 3rd Monday in February
        holidays.Add(GetNthWeekdayOfMonth(year, 2, DayOfWeek.Monday, 3));

        // Good Friday: 2 days before Easter Sunday
        DateTime easter = GetEasterSunday(year);
        holidays.Add(easter.AddDays(-2));

        // Memorial Day: Last Monday in May
        holidays.Add(GetLastWeekdayOfMonth(year, 5, DayOfWeek.Monday));

        // Independence Day
        holidays.Add(ObservedHoliday(new DateTime(year, 7, 4)));

        // Labor Day: 1st Monday in September
        holidays.Add(GetNthWeekdayOfMonth(year, 9, DayOfWeek.Monday, 1));

        // Thanksgiving Day: 4th Thursday in November
        holidays.Add(GetNthWeekdayOfMonth(year, 11, DayOfWeek.Thursday, 4));

        // Christmas Day
        holidays.Add(ObservedHoliday(new DateTime(year, 12, 25)));

        return holidays;
    }

    /// <summary>
    /// Returns a HashSet of US market holidays for every year in the range [startYear, endYear].
    /// </summary>
    public static HashSet<DateTime> GetUSMarketHolidays(int startYear, int endYear)
    {
        var allHolidays = new HashSet<DateTime>();
        for (int year = startYear; year <= endYear; year++)
        {
            foreach (var holiday in GetUSMarketHolidays(year))
            {
                allHolidays.Add(holiday);
            }
        }
        return allHolidays;
    }

    /// <summary>
    /// Adjusts a fixed-date holiday for weekends.
    /// If the holiday falls on a Saturday, it is observed on the preceding Friday.
    /// If it falls on a Sunday, it is observed on the following Monday.
    /// Otherwise, returns the original date.
    /// </summary>
    public static DateTime ObservedHoliday(DateTime holiday)
    {
        if (holiday.DayOfWeek == DayOfWeek.Saturday)
        {
            return holiday.AddDays(-1);
        }
        else if (holiday.DayOfWeek == DayOfWeek.Sunday)
        {
            return holiday.AddDays(1);
        }
        return holiday;
    }

    /// <summary>
    /// Gets the nth occurrence of a specific weekday in a given month.
    /// For example, the 3rd Monday of January.
    /// </summary>
    public static DateTime GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int n)
    {
        // Start at the first day of the month.
        DateTime dt = new DateTime(year, month, 1);
        // Advance until we find the first desired weekday.
        while (dt.DayOfWeek != dayOfWeek)
        {
            dt = dt.AddDays(1);
        }
        // Then add 7 days (n-1) times.
        return dt.AddDays(7 * (n - 1));
    }

    /// <summary>
    /// Gets the last occurrence of a specific weekday in a given month.
    /// For example, the last Monday in May.
    /// </summary>
    public static DateTime GetLastWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek)
    {
        // Get the last day of the month.
        DateTime dt = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        // Move backward until we hit the desired weekday.
        while (dt.DayOfWeek != dayOfWeek)
        {
            dt = dt.AddDays(-1);
        }
        return dt;
    }

    /// <summary>
    /// Computes Easter Sunday for the given year using the Anonymous Gregorian algorithm.
    /// </summary>
    public static DateTime GetEasterSunday(int year)
    {
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateTime(year, month, day);
    }
}
