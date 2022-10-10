using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiPixelTextEffect;

namespace XiPixelText
{
    public class DemoScene00 : MonoBehaviour
    {
        public PixelText[] pixelTexts;

        static string[] pixelStrings = new string[] { "CLASSIC", "ACADE", "EFFECT", "XiPixelText" };

        // Start is called before the first frame update
        void OnEnable()
        {
            XiSound.SoundSystem.PreInitialize();
            StartCoroutine("Demo");
        }

        void OnDisable()
        {
            XiSound.SoundSystem.DeInitialize();
        }

        // Update is called once per frame
        void Update()
        {
            XiSound.SoundSystem.OnUpdate();
        }

        IEnumerator Demo()
        {
            int txIdx = 0;
            int fxIdx = 0;
            yield return new WaitForSeconds(2);
            while (true)
            {
                pixelTexts[fxIdx].SetText(pixelStrings[txIdx]);
                pixelTexts[fxIdx].Animate(PixelText.EAnmiation.MakeVisible);
                yield return new WaitForSeconds(3);
                pixelTexts[fxIdx].Animate(PixelText.EAnmiation.MakeInvisible);
                yield return new WaitForSeconds(3);
                fxIdx = (fxIdx + 1) % pixelTexts.Length;
                txIdx = (txIdx + 1) % pixelStrings.Length;
            }
        }
    }
}
