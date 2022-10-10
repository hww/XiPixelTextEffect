# XiPixelTextEffect _The text effect for Unity 3D_

![](https://img.shields.io/badge/unity-2018.3%20or%20later-green.svg)
[![⚙ Build and Release](https://github.com/hww/XiPixelTextEffect/actions/workflows/ci.yml/badge.svg)](https://github.com/hww/XiPixelTextEffect/actions/workflows/ci.yml)
[![openupm](https://img.shields.io/npm/v/com.hww.xipixeltexteffect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.hww.xipixeltexteffect/)
[![](https://img.shields.io/github/license/hww/XiPixelTextEffect.svg)](https://github.com/hww/XiPixelTextEffect/blob/master/LICENSE)
[![semantic-release: angular](https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release)](https://github.com/semantic-release/semantic-release)


A very simple visual effect in the style of classic arcade machines. It can be used for header screens or to show the progress score, or even as a count down counter.

![Demo Video](/Docs/demo-video.gif)

## Install

The package is available on the openupm registry. You can install it via openupm-cli.

```bash
openupm add com.hww.xipixeltexteffect
```
You can also install via git url by adding this entry in your manifest.json

```bash
"com.hww.xicore": "https://github.com/hww/XiPixelTextEffect.git#upm"
```

## Usage

```C#
pixelTexts.SetText("Hello");
pixelTexts.Animate(PixelText.EAnmiation.MakeVisible);
yield return new WaitForSeconds(3);
pixelTexts.Animate(PixelText.EAnmiation.MakeInvisible);
```
