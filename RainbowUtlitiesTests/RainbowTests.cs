using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramUpdater.RainbowUtlities;

namespace RainbowUtlitiesTests
{
    [TestClass]
    public class RainbowTests
    {
        private readonly Random rnd = new();

        private IEnumerable<Update> IterRandomData(
            int seed, int count, (int MinimumId, int MaximumId) idRange)
        {
            foreach (var update in Enumerable.Range(seed, count)
                .Select(x => new Update(x, rnd.Next(idRange.MinimumId, idRange.MaximumId))))
            {
                yield return update;
            }
        }

        private IEnumerable<Update> IterData(int seed, int count)
        {
            foreach (var update in Enumerable.Range(seed, count)
                .Select(x => new Update(x, 10000 + x)))
            {
                yield return update;
            }
        }

        [TestMethod]
        [DataRow(2, 1000)]
        [DataRow(5, 1000)]
        [DataRow(10, 1000)]
        [DataRow(100, 100)]
        public async Task DataWithSameIdShouldProcessSequentiallyAsync(
            int dataCount, int handlingTakesMs = 1000)
        {
            var rainbow = new Rainbow<int, Update>(
                maximumParallel: 1,
                idResolver: x => x.OwnerId);

            var start = DateTime.Now;

            foreach (var update in IterRandomData(0, dataCount, (1, 1)))
            {
                await rainbow.WriteAsync(update);
            }

            while (true)
            {
                await Task.Delay(10);

                if (rainbow.IsIdle)
                {
                    break;
                }
            }

            Assert.IsTrue((DateTime.Now - start).TotalMilliseconds >= handlingTakesMs * dataCount);
        }

        [TestMethod]
        [DataRow(2, 2000)]
        [DataRow(5, 2000)]
        [DataRow(10, 2000)]
        [DataRow(100, 2000)]
        public async Task DataWithDifferentIdsShouldProcessParallelAsync(
            int dataCount, int handlingTakesMs = 1000)
        {
            var rainbow = new Rainbow<int, Update>(
                maximumParallel: dataCount,
                idResolver: x => x.OwnerId);

            var start = DateTime.Now;

            foreach (var update in IterData(0, dataCount))
            {
                await rainbow.WriteAsync(update);
            }

            while (true)
            {
                await Task.Delay(10);

                if (rainbow.IsIdle)
                {
                    break;
                }
            }

            Assert.IsTrue((DateTime.Now - start).Seconds <= handlingTakesMs / 1000);
        }

        [TestMethod]
        [DataRow(100)]
        public async Task AllUpdatesShouldBeHandledAsync(int dataCount)
        {
            var handled = 0;

            var rainbow = new Rainbow<int, Update>(
                maximumParallel: 10,
                idResolver: x => x.OwnerId);

            var start = DateTime.Now;

            foreach (var update in IterRandomData(0, dataCount, (10000, 20000)))
            {
                await rainbow.WriteAsync(update);
            }

            while (true)
            {
                await Task.Delay(10);

                if (rainbow.IsIdle)
                {
                    break;
                }
            }

            Assert.AreEqual(dataCount, handled);
        }
    }
}