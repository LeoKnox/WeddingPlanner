
namespace WeddingPlanner.Models
{
    public class Guest
    {
        public int GuestId {get; set;}
        public int UserId {get; set;}
        public int WeddingId {get; set;}
        public User User {get; set;}
        public Wedding Wedding {get; set;}
    }
}