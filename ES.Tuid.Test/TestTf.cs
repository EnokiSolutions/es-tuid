using System;
using System.Linq;
using NUnit.Framework;

namespace ES.Tuid.Test
{
  [TestFixture]
  public class TestTf
  {
    static ulong BrokenClock()
    {
      return 0;
    }

    [Test]
    public void Test()
    {
      TestGenerator(new TuidGenerator());
      TestGenerator(new TuidGenerator(0));
      TestGenerator(new TuidGenerator(256, BrokenClock));
      TestGenerator(new TuidGenerator(0, BrokenClock));
    }

    private static void TestGenerator(ITuidGenerator g)
    {
      var t1 = g.Generate();
      var t2 = g.Generate();
      Assert.AreEqual(36, t1.Length);
      Assert.AreEqual(36, t2.Length);
      Assert.Greater(t2, t1);
      Assert.Greater(t2, "");

      var ts = Enumerable.Range(0, 1000).Select(_ => g.Generate()).ToArray();
      for (var i = 1; i < ts.Length; ++i)
      {
        Assert.Greater(ts[i], ts[i-1]);
      }
    }
  }
}