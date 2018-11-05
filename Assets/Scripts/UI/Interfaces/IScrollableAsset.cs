using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing
{
    public interface IScrollableAsset
    {
        IEnumerable<IDisplayAsset> assets { get; }
    }

    public interface IDisplayAsset{
        string displayName { get; }
    }
}