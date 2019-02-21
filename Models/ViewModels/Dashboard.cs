using System.Collections.Generic;
using WeddingPlanner.Models;

namespace WeddingPlanner
{
  public class Dashboard
  {
    public User User { get; set; }
    public List<Wedding> Weddings { get; set; }
  }
}