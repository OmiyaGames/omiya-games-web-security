# [Omiya Games](https://www.omiyagames.com/) - Web Security

[![openupm](https://img.shields.io/npm/v/com.omiyagames.web.security?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.omiyagames.web.security/) [![Documentation](https://github.com/OmiyaGames/omiya-games-web-security/workflows/Host%20DocFX%20Documentation/badge.svg)](https://omiyagames.github.io/omiya-games-web-security/) [![Mirroring](https://github.com/OmiyaGames/omiya-games-web-security/workflows/Mirroring/badge.svg)](https://bitbucket.org/OmiyaGames/omiya-games-web-security) [![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I51KS8F)

**Web Security** is a Github package [Omiya Games](https://www.omiyagames.com/) uses to confirm the WebGL build of their games are running on the correct host.  More documentation coming soon!

## Install

There are two common methods for installing this package.

### Through [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

Unity's own Package Manager supports [importing packages through a URL to a Git repo](https://docs.unity3d.com/Manual/upm-ui-giturl.html):

1. First, on this repository page, click the "Clone or download" button, and copy over this repository's HTTPS URL.  
2. Then click on the + button on the upper-left-hand corner of the Package Manager, select "Add package from git URL..." on the context menu, then paste this repo's URL!

While easy and straightforward, this method has a few major downside: it does not support dependency resolution and package upgrading when a new version is released.  To add support for that, the following method is recommended:

### Through [OpenUPM](https://openupm.com/)

Installing via [OpenUPM's command line tool](https://openupm.com/) is recommended because it supports dependency resolution, upgrading, and downgrading this package.

If you haven't already [installed OpenUPM](https://openupm.com/docs/getting-started.html#installing-openupm-cli), you can do so through Node.js's `npm` (obviously have Node.js installed in your system first):
```
npm install -g openupm-cli
```
Then, to install this package, just run the following command at the root of your Unity project:
```
openupm add com.omiyagames.web.security
```

## Resources

- [Documentation](https://omiyagames.github.io/omiya-games-web-security/)
- [Change Log](https://omiyagames.github.io/omiya-games-web-security/manual/changelog.md)

## LICENSE

Overall package is licensed under [MIT](https://github.com/OmiyaGames/omiya-games-web-security/blob/master/LICENSE.md), unless otherwise noted in the [3rd party licenses](https://github.com/OmiyaGames/omiya-games-web-security/blob/master/THIRD%20PARTY%20NOTICES.md) file and/or source code.

Copyright (c) 2019-2020 Omiya Games
