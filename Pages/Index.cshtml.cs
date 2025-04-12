using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.Json;

namespace CalendarApp.Pages
{
    public class EventDay
    {
        public List<string> Descriptions { get; set; } = new();
    }

    public class IndexModel : PageModel
    {
        public DateTime CurrentMonth { get; set; } = DateTime.Now;
        public List<int> Days { get; set; } = new();
        public Dictionary<string, EventDay> Events { get; set; } = new();
        public string? SelectedDate { get; set; }
        public int StartOffset { get; set; }

        private readonly string _eventsFile = "events.json";

        public void OnGet(int? monthOffset = null, string? currentMonth = null, string? selectedDate = null)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                Response.Redirect("/Login");
                return;
            }

            if (!string.IsNullOrEmpty(currentMonth) && DateTime.TryParse(currentMonth + "-01", out DateTime parsed))
            {
                CurrentMonth = parsed;
            }
            else
            {
                CurrentMonth = DateTime.Now;
            }

            if (monthOffset.HasValue)
            {
                CurrentMonth = CurrentMonth.AddMonths(monthOffset.Value);
            }

            SelectedDate = selectedDate;

            LoadEvents();
            InitializeCalendar(CurrentMonth);
        }



        public void OnPost()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                Response.Redirect("/Login");
                return;
            }

            {
                LoadEvents();

                var date = Request.Form["date"].ToString();
                var description = Request.Form["description"].ToString();
                var deleteDate = Request.Form["deleteDate"];
                var deleteDescription = Request.Form["deleteDescription"];
                var currentMonthStr = Request.Form["currentMonth"].ToString();

                if (DateTime.TryParse(currentMonthStr + "-01", out DateTime parsedMonth))
                {
                    CurrentMonth = parsedMonth;
                }
                else
                {
                    CurrentMonth = DateTime.Now;
                }

                
                if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(description))
                {
                    if (!Events.ContainsKey(date))
                    {
                        Events[date] = new EventDay { Descriptions = new List<string>() };
                    }
                    Events[date].Descriptions.Add(description);
                    SaveEvents();
                    SelectedDate = date;
                }

                
                if (!string.IsNullOrEmpty(deleteDate) && !string.IsNullOrEmpty(deleteDescription))
                {
                    if (Events.ContainsKey(deleteDate))
                    {
                        Events[deleteDate].Descriptions.Remove(deleteDescription);
                        if (Events[deleteDate].Descriptions.Count == 0)
                        {
                            Events.Remove(deleteDate);
                        }
                        SaveEvents();
                    }
                    SelectedDate = deleteDate;
                }

                InitializeCalendar(CurrentMonth);
            }
        }
 


        private void InitializeCalendar(DateTime baseMonth)
        {
            Days.Clear();
            int daysInMonth = DateTime.DaysInMonth(baseMonth.Year, baseMonth.Month);
            StartOffset = ((int)new DateTime(baseMonth.Year, baseMonth.Month, 1).DayOfWeek + 1) % 7;

            for (int i = 1; i <= daysInMonth; i++)
            {
                Days.Add(i);
            }
        }

        private void LoadEvents()
        {
            if (System.IO.File.Exists(_eventsFile))
            {
                var json = System.IO.File.ReadAllText(_eventsFile);
                Events = JsonSerializer.Deserialize<Dictionary<string, EventDay>>(json)
                         ?? new Dictionary<string, EventDay>();
            }
        }

        private void SaveEvents()
        {
            var json = JsonSerializer.Serialize(Events);
            System.IO.File.WriteAllText(_eventsFile, json);
        }
    }
}
