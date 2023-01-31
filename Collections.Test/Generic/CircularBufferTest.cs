using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Collections.Test.Generic;

public class CircularBufferTest
{
    [Fact]
    public void CopyTo()
    {
        var p = new CircularBuffer<string>(10);
        for (int i = 0; i < 7; i++)
        {
            p.Add(i.ToString()); 
        }
        var a = p.ToArray();
        a.Should().HaveCount(7);
    }

    [Fact]
    public void OverrunTest()
    {
        var p = new CircularBuffer<int>(100);
        for (int i = 0; i < 100; i++)
        {
            p.Add(i);
        }
        p.First().Should().Be(0);
        p.Last().Should().Be(99);

        p.Add(101);

        p.First().Should().Be(1);
        p.Last().Should().Be(100);
    }
}