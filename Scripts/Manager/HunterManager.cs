using Godot;

namespace MonsterHunterIdle;

public partial class HunterManager : Node
{
   // public HunterManager()
   // {
   //    // Check if a save file of the hunter exists.
   //    // If yes then fill in the Hunter variable otherwise just create a new Hunter object
   // }

   public Hunter Hunter = new Hunter();
   public HunterFileLoader _hunterFileLoader = new  HunterFileLoader();

   public void AddHunterPoints(int progressAmount)
   {
      Hunter.Points += progressAmount;
   }

   public void AddHunterZenny(int zennyAmount)
   {
      Hunter.Zenny += zennyAmount;
   }

   private void CheckHunterProgress()
   {
      if (Hunter.Points < Hunter.MaxPoints) return;
      if (Hunter.Rank >= Hunter.MaxRank) return;

      Hunter.Rank++; // Increase Rank
      Hunter.Points -= Hunter.MaxPoints; // Reset Points
      IncreaseHunterProgress();
   }

   private void IncreaseHunterProgress()
   {
      int pointsThreshold = 100;

      if (Hunter.Rank < pointsThreshold) Hunter.MaxPoints += pointsThreshold;
   }
}