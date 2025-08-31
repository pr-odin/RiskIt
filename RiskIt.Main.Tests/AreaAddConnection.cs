using RiskIt.Main.Models;

namespace RiskIt.Main.Tests
{
    /*
     * I welcome our test lord and savior, ChatGPT
     */
    public class AreaAddConnection
    {
        [Test]
        public void AddConnection_AddSameConnectionTwice_NoDuplicateEffect()
        {
            // Arrange
            var area1 = new Area<int>(1);
            var area2 = new Area<int>(2);

            // Act
            area1.AddConnection(area2);
            area1.AddConnection(area2);

            // Assert
            Assert.IsTrue(area1.IsAdjecent(area2));
            Assert.IsTrue(area2.IsAdjecent(area1));
            // The adjacency should still only exist once in each direction
            Assert.AreEqual(true, area1.IsAdjecent(area2));
            Assert.AreEqual(true, area2.IsAdjecent(area1));
        }

        [Test]
        public void AddConnection_AddConnection_ExistsBothWays()
        {
            // Arrange
            var area1 = new Area<int>(1);
            var area2 = new Area<int>(2);

            // Act
            area1.AddConnection(area2);

            // Assert
            Assert.IsTrue(area1.IsAdjecent(area2), "Area1 should be adjacent to Area2");
            Assert.IsTrue(area2.IsAdjecent(area1), "Area2 should be adjacent to Area1");
        }

        [Test]
        public void AddConnection_AddMultipleConnections_AllExist()
        {
            // Arrange
            var area1 = new Area<int>(1);
            var area2 = new Area<int>(2);
            var area3 = new Area<int>(3);

            // Act
            area1.AddConnection(area2);
            area1.AddConnection(area3);

            // Assert
            Assert.IsTrue(area1.IsAdjecent(area2), "Area1 should be adjacent to Area2");
            Assert.IsTrue(area1.IsAdjecent(area3), "Area1 should be adjacent to Area3");
            Assert.IsTrue(area2.IsAdjecent(area1), "Area2 should be adjacent to Area1");
            Assert.IsTrue(area3.IsAdjecent(area1), "Area3 should be adjacent to Area1");
        }

        [Test]
        public void AddConnection_AreaNotConnected_IsNotAdjacent()
        {
            // Arrange
            var area1 = new Area<int>(1);
            var area2 = new Area<int>(2);

            // Act
            // no connection added

            // Assert
            Assert.IsFalse(area1.IsAdjecent(area2));
            Assert.IsFalse(area2.IsAdjecent(area1));
        }
    }
}


