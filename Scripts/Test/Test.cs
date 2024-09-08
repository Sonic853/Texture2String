
using Sonic853.Texture2String;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Texture2String.Test
{
    public class Test : UdonSharpBehaviour
    {
        public Texture2D texture;
        public Text text;
        public UdonSharpBehaviour imageDisplayer;
        public bool useLength;
        public bool evenStorage;
        public int mode;
        void Start()
        {
            switch (mode)
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
            var t = texture.ToText(useLength, evenStorage);
            Debug.Log(t);
            text.text = t;
            if (imageDisplayer != null)
            {
                imageDisplayer.SetProgramVariable("texture", texture);
                imageDisplayer.SendCustomEvent("SetTexture");
            }
        }
        public void TestRGBAToText()
        {
            var t = texture.RGBAToText(useLength, evenStorage);
            Debug.Log(t);
            text.text = t;
            Debug.Log(texture.RGBAToText(useLength, evenStorage));
            if (imageDisplayer != null)
            {
                imageDisplayer.SetProgramVariable("texture", texture);
                imageDisplayer.SendCustomEvent("SetTexture");
            }
        }
        public void TestAlphaToText()
        {
            var t = texture.AlphaToText(useLength, evenStorage);
            Debug.Log(t);
            text.text = t;
            if (imageDisplayer != null)
            {
                imageDisplayer.SetProgramVariable("texture", texture);
                imageDisplayer.SendCustomEvent("SetTexture");
            }
        }
    }
}
