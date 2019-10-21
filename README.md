# ![Icon](Media/winleafs.ico) Winleafs

[![Build status](https://dev.azure.com/winleafs1/Winleafs/_apis/build/status/Winleafs-.NET%20Desktop%20with%20SonarCloud-CI)](https://dev.azure.com/winleafs1/Winleafs/_build/latest?definitionId=2)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=winleafs-wpf&metric=alert_status)](https://sonarcloud.io/dashboard?id=winleafs-wpf)
![GitHub tag (latest by date)](https://img.shields.io/github/tag-date/StijnOostdam/Winleafs.svg?label=Latest%20Release)

 Windows application for Nanoleaf lights.

Installers can be found on the [release page](https://github.com/StijnOostdam/Winleafs/releases
).

Canvas support currently not guaranteed.

## Features
- Make multiple weekly schedules for your Nanoleaf lights
- Supports multiple devices simultaneously
- Add time based triggers to automatically set your lights to an effect and brightness at your chosen time
- Add triggers based on sunrise and sunset times (requires location)
- Screen mirror, project your screen (like Netflix or games) onto your lights and your lights will be the same color as their position on your monitor
- Ambilight, make your lights the average color of your monitor
- Start the program at Windows startup
- Automatically turn off your lights when your PC shuts down
- Override the schedule at any time
- All settings are saved locally

## TODOs & Future features

TODOs and future features can be found on the [GitHub projects board](https://github.com/StijnOostdam/Winleafs/projects/1). Any issue or card in the approved column will be part of the next release (probably).

## Support

You can [donate to us using Paypal](https://www.paypal.me/winleafs). 
Note: all donations will go towards buying more Nanoleaf products for *"Testing purposes"*.

You can also make a fork of this project and pick up a task from our projects board.
We would HIGHLY appreciate if you'd contact us before picking up a task so we can avoid doing double work.

## Showcase
![Screenshot](Media/Screenshot.png)

### Screen Mirror
![Screenshot](Media/ScreenMirror.jpeg)

- [Beat Saber Ambilight](https://www.youtube.com/watch?v=CRe0o0TNlT4)
- [Screen mirror](https://www.youtube.com/watch?v=iT_jQnQLVQA)

## Bugs
If you are experiencing problems, try deleting the settings file from %appdata%/Winleafs. Note: this removes all settings and schedules.

1. The Nanoleaf device must be connected to the same router as your Windows device. This means that 2.4Ghz and 5Ghz networks are also seperated as the Nanoleaf connects to 2.4Ghz

## Projects and APIs used

- [Icanhazip](http://icanhazip.com) To retrieve your IP address
- [IP-API](http://ip-api.com/) To get your location based on your IP address
- [Sunset Sunrise](https://sunrise-sunset.org/api) To get sunrise and sunset times based on your location
- [Modern UI Icons](http://modernuiicons.com/) Icons in this project
