using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Amazon.S3.Model;

public static class S3ObjectExtensions
{
    public static bool IsXml(this S3Object s3Object)
    {
        return Path.GetExtension(s3Object.Key).Equals(".xml", StringComparison.InvariantCultureIgnoreCase);
    }
}

