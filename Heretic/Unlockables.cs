using RoR2;
using RoR2.Achievements;

namespace Heretic
{

    [RegisterAchievement("MoffeinHereticUnlock", "Survivors.MoffeinHeretic", null, null)]
    public class HereticUnlockAchievement : BaseEndingAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("HereticBody");
        }
        public override bool ShouldGrant(RunReport runReport)
        {
            if (runReport.gameEnding && runReport.gameEnding.isWin && runReport.gameEnding == RoR2Content.GameEndings.MainEnding)
            {
                return true;
            }
            return false;
        }
    }
}
