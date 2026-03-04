using NUnit.Framework;
using Hearts.Core;

namespace Hearts.Tests
{
    [TestFixture]
    public class PassDirectionTests
    {
        [Test]
        public void PassDirection_Rotation_CyclesCorrectly()
        {
            Assert.AreEqual(PassDirection.Left, PassDirectionExtensions.ForRound(0));
            Assert.AreEqual(PassDirection.Right, PassDirectionExtensions.ForRound(1));
            Assert.AreEqual(PassDirection.Across, PassDirectionExtensions.ForRound(2));
            Assert.AreEqual(PassDirection.Hold, PassDirectionExtensions.ForRound(3));

            // Cycle repeats
            Assert.AreEqual(PassDirection.Left, PassDirectionExtensions.ForRound(4));
            Assert.AreEqual(PassDirection.Right, PassDirectionExtensions.ForRound(5));
            Assert.AreEqual(PassDirection.Across, PassDirectionExtensions.ForRound(6));
            Assert.AreEqual(PassDirection.Hold, PassDirectionExtensions.ForRound(7));
        }

        [Test]
        public void PassDirection_Left_TargetIsNextClockwise()
        {
            Assert.AreEqual(1, PassDirection.Left.GetTargetSeat(0));
            Assert.AreEqual(2, PassDirection.Left.GetTargetSeat(1));
            Assert.AreEqual(3, PassDirection.Left.GetTargetSeat(2));
            Assert.AreEqual(0, PassDirection.Left.GetTargetSeat(3)); // Wraps
        }

        [Test]
        public void PassDirection_Right_TargetIsPreviousClockwise()
        {
            Assert.AreEqual(3, PassDirection.Right.GetTargetSeat(0)); // Wraps
            Assert.AreEqual(0, PassDirection.Right.GetTargetSeat(1));
            Assert.AreEqual(1, PassDirection.Right.GetTargetSeat(2));
            Assert.AreEqual(2, PassDirection.Right.GetTargetSeat(3));
        }

        [Test]
        public void PassDirection_Across_TargetIsOpposite()
        {
            Assert.AreEqual(2, PassDirection.Across.GetTargetSeat(0));
            Assert.AreEqual(3, PassDirection.Across.GetTargetSeat(1));
            Assert.AreEqual(0, PassDirection.Across.GetTargetSeat(2));
            Assert.AreEqual(1, PassDirection.Across.GetTargetSeat(3));
        }

        [Test]
        public void PassDirection_Hold_TargetIsSelf()
        {
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(i, PassDirection.Hold.GetTargetSeat(i));
            }
        }
    }
}
