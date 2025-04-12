using CalendarApp.Models;

namespace CalendarApp.Data
{
    public static class FakeUserStore
    {
        public static List<User> Users { get; } = new List<User>();
    }
}
