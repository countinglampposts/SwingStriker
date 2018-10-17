using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Swing.Character
{
    [CreateAssetMenu(menuName = "Swing/CharacterCollection")]
    public class CharacterCollection : ScriptableObject, IScrollableAsset
    {
        public CharacterAsset[] characters;

        public IEnumerable<IDisplayAsset> assets
        {
            get
            {
                return characters.Select(asset => asset as IDisplayAsset);
            }
        }
    }
}