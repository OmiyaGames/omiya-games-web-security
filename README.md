# [Omiya Games](https://www.omiyagames.com/) - Web Security

[![openupm](https://img.shields.io/npm/v/com.omiyagames.web.security?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.omiyagames.web.security/) [![Documentation](https://github.com/OmiyaGames/omiya-games-web-security/workflows/Host%20DocFX%20Documentation/badge.svg)](https://omiyagames.github.io/omiya-games-web-security/) [![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I51KS8F)

**Web Security** is a Github package [Omiya Games](https://www.omiyagames.com/) uses to peform various security features for WebGL builds.  This includes:

### Web Location Checker

A script that verifies the build is running on the correct host.  Attach to a `GameObject` like any other `MonoBehavior`, then call the coroutine, `CheckDomainList()`, from another script.  For example, one can create a script with the method below, then attach it to the same `GameObject` the `WebLocationChecker` is on:

```csharp
IEnumerator Start()
{
    // Verify the domain
    WebLocationChecker checker = GetComponent<WebLocationChecker>();
    yield return StartCoroutine(checker.CheckDomainList());

    // Check if the domain was verified
    if(checker.CurrentState == WebLocationChecker.State.DomainMatched)
    {
        // Change scene to the main menu
        SceneManager.LoadScene("Main menu");
    }
}
```

The script contains the following inspector fields:

![Inspector](https://omiyagames.github.io/omiya-games-web-security/resources/web-location-checker.png)

| Field                     	|           Required?          	| Description 	|
|---------------------------	|:----------------------------:	|-------------	|
| Remote Domain List Url    	|               No             	| The path to download a [`DomainList`](https://omiyagames.github.io/omiya-games-cryptography/manual/domain-list.html), relative to the root of the WebGL build (typically where `index.html` is in). The strings in the `DomainList` will be used to match the domain the build is running on, *in addition to* strings listed in `Domain Must Contain`. Leave empty if no file should be downloaded.	|
| Domain Decrypter          	|              No              	| The [`StringCryptographer`](https://omiyagames.github.io/omiya-games-cryptography/manual/string-cryptographer.html) to use to decrypt the content of the downloaded [`DomainList`](https://omiyagames.github.io/omiya-games-cryptography/manual/domain-list.html). Entirely optional, especially if the downloaded file is not expected to be encrypted.	|
| GameObjects to Deactivate 	|               No             	| A list of `GameObjects` to deactivate while the script verifies the domain the build is running on.	|
| Domain Must Contain       	|              Yes             	| A list of strings to verify whether the domain the game is running on matches. This script supports `?` and `*` wildcards (former any single character, while the latter matches a series of character).	|
| Force Redirect            	|              No              	| If checked, *and* the domain did not match, prompts the script to redirect to a different website. If this build is embedded in an `iframe` with redirect permission restrictions (which most browsers enable by default), the redirect may fail with an `AccessDenied` error.	|
| Redirect URL              	| If `Force Redirect` is checked 	| The URL to redirect to if the doamin did not match. This URL should include `https://`	|

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

Copyright (c) 2019-2021 Omiya Games
