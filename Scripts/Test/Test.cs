
using Sonic853.Texture2String;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853
{
    public class Test : UdonSharpBehaviour
    {
        public Texture2D texture;
        public bool useLength;
        public int mode;
        void Start()
        {
            switch(mode)
            {
                case 0:
                {
                    TestToText();
                }
                break;
                case 1:
                {
                    TestRGBAToText();
                }
                break;
                case 2:
                {
                    TestAlphaToText();
                }
                break;
            }
        }
        public void TestToText()
        {
            Debug.Log(texture.ToText(useLength));
        }
        public void TestRGBAToText()
        {
            Debug.Log(texture.RGBAToText(useLength));
        }
        public void TestAlphaToText()
        {
            Debug.Log(texture.AlphaToText(useLength));
        }
    }
}
