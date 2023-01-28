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
}