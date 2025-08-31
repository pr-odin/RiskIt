using RiskIt.Main.Models;

namespace RiskIt.Main.Tests
{
    public class AreaCanReach
    {
        [Test]
        public void CanReach_OtherDirectConnection_returnsTrue()
        {
            Player player = new Player
            {
                Id = 0,
            };

            Area<string> a = new Area<string>("a")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> b = new Area<string>("b")
            {
                Troops = 2,
                Player = player,
            };

            a.AddConnection(b);

            Assert.That(a.CanReach(b), Is.True);
        }

        [Test]
        public void CanReach_Other2ndConnection_returnsTrue()
        {
            Player player = new Player
            {
                Id = 0,
            };

            Area<string> a = new Area<string>("a")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle = new Area<string>("middle")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> b = new Area<string>("b")
            {
                Troops = 2,
                Player = player,
            };

            a.AddConnection(middle);
            b.AddConnection(middle);

            Assert.That(a.CanReach(b), Is.True);
        }

        [Test]
        public void CanReach_OtherIsSeparatedByEnemyPlay_returnsFalse()
        {
            Player player = new Player
            {
                Id = 0,
            };

            Area<string> a = new Area<string>("a")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle = new Area<string>("middle")
            {
                Troops = 2,
                Player = new Player { Id = 1 },
            };

            Area<string> b = new Area<string>("b")
            {
                Troops = 2,
                Player = player,
            };

            a.AddConnection(middle);
            b.AddConnection(middle);

            Assert.That(a.CanReach(b), Is.False);
        }

        [Test]
        public void CanReach_OtherIs5Down_returnsTrue()
        {
            Player player = new Player
            {
                Id = 0,
            };

            Area<string> a = new Area<string>("a")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle = new Area<string>("middle")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle1 = new Area<string>("middle1")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle2 = new Area<string>("middle2")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle3 = new Area<string>("middle3")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middle4 = new Area<string>("middle4")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> b = new Area<string>("b")
            {
                Troops = 2,
                Player = player,
            };

            a.AddConnection(middle);
            middle.AddConnection(middle1);
            middle1.AddConnection(middle2);
            middle2.AddConnection(middle3);
            middle3.AddConnection(middle4);
            b.AddConnection(middle4);

            Assert.That(a.CanReach(b), Is.True);
        }

        [Test]
        public void CanReach_Other2PathsOnly1IsFriendly_returnsTrue()
        {
            Player player = new Player
            {
                Id = 0,
            };

            Area<string> a = new Area<string>("a")
            {
                Troops = 2,
                Player = player,
            };

            Area<string> middleEnemy = new Area<string>("middleEnemy")
            {
                Troops = 2,
                Player = new Player { Id = 1 },
            };

            Area<string> middleFriendly = new Area<string>("middleFriendly")
            {
                Troops = 2,
                Player = player,
            };


            Area<string> b = new Area<string>("b")
            {
                Troops = 2,
                Player = player,
            };

            a.AddConnection(middleEnemy);
            a.AddConnection(middleFriendly);
            middleEnemy.AddConnection(middleFriendly);
            middleFriendly.AddConnection(b);

            Assert.That(a.CanReach(b), Is.True);
        }
    }
}

