using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
using OmiyaGames.Cryptography;
using OmiyaGames.Global;

namespace OmiyaGames.Web.Security
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="WebLocationChecker.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Date:</strong> 5/15/2016<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item><item>
    /// <term>
    /// <strong>Date:</strong> 6/5/2018<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Removed plain-text support.</description>
    /// </item><item>
    /// <term>
    /// <strong>Date:</strong> 2/12/2019<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>
    /// Adding encryption support of the binary <see cref="DomainList"/>.
    /// </description>
    /// </item><item>
    /// <term>
    /// <strong>Version:</strong> 0.1.0-preview.1<br/>
    /// <strong>Date:</strong> 5/20/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>
    /// Converting to package. Improving documentation for DocFX support.
    /// </description>
    /// </item><item>
    /// <term>
    /// <strong>Version:</strong> 0.1.1-preview.1<br/>
    /// <strong>Date:</strong> 5/22/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>
    /// Fixing documentation to actually be XML-compliant
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <para>
    /// Original code by andyman from Github:<br/>
    /// https://gist.github.com/andyman/e58dea85cce23cccecff
    /// </para><para>
    /// Extra modifications by jcx from Github:<br/>
    /// https://gist.github.com/jcx/93a3fc93531911add8a8
    /// </para><para>
    /// Add this script to an object in the first scene of your game.
    /// For WebGL builds, this script grabs the domain the game is running on,
    /// and verifies it against two lists:
    /// <list type="number">
    /// <item><description>
    /// The list of strings in <see cref="DefaultDomainList"/>.
    /// </description></item>
    /// <item><description>
    /// Optionally, if <see cref="RemoteDomainListUrl"/> isn't an empty string
    /// (or null), this script will attempt to download from the specified url,
    /// and read it as a <see cref="DomainList"/>. If successful, the globs
    /// listed in the <see cref="DomainList"/> will be used to match the domain
    /// as well. Remember that if the <see cref="DomainList"/> is encrypted,
    /// the <see cref="domainDecrypter"/> needs to be set in the Unity inspector
    /// to help decrypt the content of the list.
    /// </description></item>
    /// </list>
    /// Don't forget to run the <see cref="CheckDomainList()"/> coroutine! When
    /// it finished, the <see cref="CurrentState"/> will be set, indicating
    /// whether a match was found or not. Here's an example:
    /// <code>
    /// IEnumerator Start()
    /// {
    ///     WebLocationChecker checker = GetComponent&lt;WebLocationChecker&gt;();
    ///     yield return StartCoroutine(checker.CheckDomainList());
    ///     Debug.Log(checker.CurrentState);
    /// }
    /// </code>
    /// If the script is attached to a <see cref="GameObject"/> with
    /// <see cref="Singleton"/> already attached, the above example code
    /// will run automatically on <see cref="SceneAwake"/>.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public class WebLocationChecker : ISingletonScript
    {
        /// <summary>
        /// Header string for the Unity Inspector.
        /// </summary>
        public const string RemoteDomainListHeader = "Remote Domain List";

        /// <summary>
        /// Indicates progression and result of this script.
        /// </summary>
        public enum State : short
        {
            /// <summary>
            /// Indicates the script hasn't verified the domain yet.
            /// </summary>
            NotUsed = -1,
            /// <summary>
            /// Indicates the script is in the middle of evaluating
            /// the domain the WebGL build is running on. This process
            /// can include downloading a <see cref="DomainList"/>.
            /// </summary>
            InProgress = 0,
            /// <summary>
            /// Indicates there were trouble verifying the domain.
            /// Most likely, this is due to being unable to download
            /// a <see cref="DomainList"/> for verification.
            /// </summary>
            EncounteredError,
            /// <summary>
            /// Indicates the domain this WebGL build is running on
            /// is valid, matching an expected glob/string.
            /// </summary>
            DomainMatched,
            /// <summary>
            /// Indicates the domain this WebGL build is running on
            /// is <em>not</em> valid:
            /// it doesn't match any expected glob/string.
            /// </summary>
            DomainDidntMatch
        }

#if UNITY_WEBGL
        /// <summary>
        /// Redirects a WebGL build to a specific page.
        /// This will <em>not</em> open a new tab or window.<br/>
        /// Note: depending on the <iframe> setting, this may cause
        /// an Access Denied error.
        /// </summary>
        /// <param name="url">URL to redirect to.</param>
        [DllImport("__Internal")]
        public static extern void RedirectTo(string url);
#endif

        ///<summary>
        /// If it is a webplayer, then the domain must contain any
        /// one or more of these strings, or it will be redirected.
        /// This array is ignored if empty (i.e. no redirect will occur).
        ///</summary>
        [SerializeField]
        private string[] domainMustContain;

        ///<summary>
        /// [optional] The URL to fetch a list of domains
        ///</summary>
        [Header(RemoteDomainListHeader)]
        [SerializeField]
        [Tooltip("[optional] The URL to fetch a list of domains")]
        private string remoteDomainListUrl;
        /// <summary>
        /// [optional] <see cref="StringCryptographer"/> to decrypt
        /// the downloaded <see cref="DomainList"/>.
        /// </summary>
        [SerializeField]
        [Tooltip("[optional] The cryptographer that decrypts the encrypted strings in the list of domains")]
        private StringCryptographer domainDecrypter;
        ///<summary>
        /// [Optional] GameObjects to deactivate while the domain checking is happening
        ///</summary>
        [SerializeField]
        [Tooltip("[Optional] GameObjects to deactivate while the domain checking is happening")]
        private GameObject[] waitObjects;

        ///<summary>
        /// If true, the game will force the webplayer to redirect to
        /// the URL below
        ///</summary>
        [Header("Redirect Options")]
        [SerializeField]
        private bool forceRedirectIfDomainDoesntMatch = true;
        ///<summary>
        /// This is where to redirect the webplayer page if none of
        /// the strings in domainMustContain are found.
        ///</summary>
        [SerializeField]
        private string redirectURL;

        private string retrievedHostName = null;

        #region Properties
        /// <summary>
        /// Indicates the state this script is in.
        /// <seealso cref="State"/>
        /// </summary>
        public State CurrentState
        {
            get;
            private set;
        } = State.NotUsed;

        /// <summary>
        /// Indicates if a <see cref="DomainList"/> was
        /// downloaded or not.
        /// </summary>
        public bool IsDomainListSuccessfullyDownloaded
        {
            get
            {
                return ((DownloadedDomainList != null) && (string.IsNullOrEmpty(DownloadDomainsUrl) == false));
            }
        }

        /// <summary>
        /// The domain this WebGL build is running on.
        /// </summary>
        public string RetrievedHostName
        {
            get
            {
                return retrievedHostName;
            }
        }

        /// <summary>
        /// The default list of domains to match <see cref="RetrievedHostName"/> against.
        /// This list excludes the globs stored in a <see cref="DomainList"/>.
        /// <seealso cref="RemoteDomainListUrl"/>
        /// </summary>
        public string[] DefaultDomainList
        {
            get
            {
                return domainMustContain;
            }
        }

        /// <summary>
        /// The list of globs in a <see cref="DomainList"/>, if successfully downloaded.
        /// </summary>
        public string[] DownloadedDomainList
        {
            get;
            private set;
        } = null;

        /// <summary>
        /// The URL used to download a <see cref="DomainList"/>. Slightly randomized
        /// to prevent predictablity.
        /// </summary>
        public string DownloadDomainsUrl
        {
            get;
            private set;
        } = null;

        /// <summary>
        /// The <see cref="Dictionary{TKey, TValue}"/> of all unique domains.
        /// Keys are the globs, and values are the regular expression equivalent.
        /// <seealso cref="DomainList.ConvertToRegex(string, StringBuilder)"/>
        /// </summary>
        public Dictionary<string, Regex> AllUniqueDomains
        {
            get;
        } = new Dictionary<string, Regex>();

        /// <summary>
        /// Indicates the error when attempting to download a <see cref="DomainList"/>.
        /// </summary>
        public string DownloadErrorMessage
        {
            get;
            private set;
        } = null;

        /// <summary>
        /// The original URL to download a <see cref="DomainList"/> from.
        /// <seealso cref="DownloadDomainsUrl"/>
        /// </summary>
        public string RemoteDomainListUrl => remoteDomainListUrl;
        #endregion

        /// <inheritdoc/>
        public override void SingletonAwake()
        {
            if (Singleton.Instance.IsWebApp == true)
            {
                // Shoot a coroutine if not in editor, and making Webplayer or WebGL
                StartCoroutine(CheckDomainList());
            }
        }

        /// <inheritdoc/>
        public override void SceneAwake()
        {
            // Do nothing
        }

        /// <summary>
        /// Makes the build redirect the browser to <see cref="redirectURL"/>.
        /// <seealso cref="RedirectTo(String)"/>
        /// </summary>
        public void ForceRedirect()
        {
#if UNITY_WEBGL
            if (string.IsNullOrEmpty(redirectURL) == false)
            {
                RedirectTo(redirectURL);
            }
#endif
        }

        /// <summary>
        /// Coroutine to start the whole verification process.
        /// Don't forget to call it with
        /// <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>, like the example below:
        /// <code>
        /// WebLocationChecker checker = GetComponent<WebLocationChecker>();
        /// StartCoroutine(checker.CheckDomainList());
        /// </code>
        /// </summary>
        /// <returns>An enumerator for a coroutine.</returns>
        public IEnumerator CheckDomainList()
        {
            // Setup variables
            StringBuilder buf = new StringBuilder();
            DownloadedDomainList = null;
            DownloadDomainsUrl = null;
            DownloadErrorMessage = null;

            // Update state
            CurrentState = State.InProgress;

            // Deactivate any objects
            SetWaitObjectActive(false);

            // Grab a domain list remotely
            if (string.IsNullOrEmpty(RemoteDomainListUrl) == false)
            {
                // Grab remote domain list
                DownloadDomainsUrl = GenerateRemoteDomainList(buf);
                yield return StartCoroutine(DownloadRemoteDomainList(buf, DownloadDomainsUrl));
            }

            // Setup hashset
            PopulateAllUniqueDomains(buf, AllUniqueDomains, DefaultDomainList, DownloadedDomainList);

            // Make sure there's at least one domain we need to check
            CurrentState = GetNewState(AllUniqueDomains, out retrievedHostName);

            // Reactivate any objects
            SetWaitObjectActive(true);

            // Check if we should force redirecting the player
            if ((forceRedirectIfDomainDoesntMatch == true) && (IsDomainInvalid(CurrentState) == true))
            {
                ForceRedirect();
            }
        }

        #region Helper Static Methods
        static State GetNewState(Dictionary<string, Regex> allUniqueDomains, out string retrievedHostName)
        {
            State newState = State.NotUsed;
            retrievedHostName = null;
            if (allUniqueDomains.Count > 0)
            {
                // parse the page's address
                bool isErrorEncountered = false;
                newState = State.DomainDidntMatch;
                if (IsHostMatchingListedDomain(allUniqueDomains, out isErrorEncountered, out retrievedHostName) == true)
                {
                    // Update state
                    newState = State.DomainMatched;
                }
                else if (isErrorEncountered == true)
                {
                    newState = State.EncounteredError;
                }
            }
            return newState;
        }

        static void PopulateAllUniqueDomains(StringBuilder buf, Dictionary<string, Regex> allUniqueDomains, params string[][] allDomains)
        {
            // Setup variables
            int listIndex = 0, stringIndex = 0;

            // Clear the dictionary
            allUniqueDomains.Clear();

            if (allDomains != null)
            {
                // Go through all the domains
                for (; listIndex < allDomains.Length; ++listIndex)
                {
                    if (allDomains[listIndex] != null)
                    {
                        // Go through all strings in the list
                        for (stringIndex = 0; stringIndex < allDomains[listIndex].Length; ++stringIndex)
                        {
                            // Add the entry and its regular expression equivalent
                            if (string.IsNullOrEmpty(allDomains[listIndex][stringIndex]) == false)
                            {
                                allUniqueDomains.Add(allDomains[listIndex][stringIndex], DomainList.ConvertToRegex(allDomains[listIndex][stringIndex], buf));
                            }
                        }
                    }
                }
            }
        }

        static bool IsHostMatchingListedDomain(Dictionary<string, Regex> domainList, out bool encounteredError, out string retrievedHostName)
        {
            Uri uri;
            bool isTheCorrectHost = false;
            retrievedHostName = null;

            // Evaluate the URL
            encounteredError = true;
            if (Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out uri) == true)
            {
                // Indicate there were no errors
                encounteredError = false;
                retrievedHostName = uri.Host;

                // Check if the scheme isn't file (i.e. local file run on computer)
                isTheCorrectHost = true;
                if (uri.Scheme != "file")
                {
                    // Make sure host matches any one of the domains
                    isTheCorrectHost = false;
                    foreach (Regex expression in domainList.Values)
                    {
                        if (expression.IsMatch(retrievedHostName) == true)
                        {
                            isTheCorrectHost = true;
                            break;
                        }
                    }
                }
            }
            return isTheCorrectHost;
        }

        static string[] ConvertToDomainList(DomainList domainList, StringCryptographer decrypter)
        {
            string[] returnDomainList = null;
            if ((domainList != null) && (domainList.Count > 0))
            {
                // Create a new string array
                returnDomainList = DomainList.Decrypt(domainList, decrypter);
            }
            return returnDomainList;
        }

        static bool IsDomainInvalid(State state)
        {
            bool returnState = false;
            switch (state)
            {
                case State.EncounteredError:
                case State.DomainDidntMatch:
                    returnState = true;
                    break;
            }
            return returnState;
        }
        #endregion

        #region Helper Local Methods
        IEnumerator DownloadRemoteDomainList(StringBuilder buf, string remoteDomainUrl)
        {
            // Start downloading the remote file (never cache this file)
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(RemoteDomainListUrl))
            {
                // Wait until the asset is fully received
                yield return www.SendWebRequest();

                // Check if there were any errors
                if ((www.isNetworkError == true) || (www.isHttpError == true))
                {
                    DownloadErrorMessage = www.error;
                }
                else
                {
                    // If asset bundle, convert it into a list
                    try
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                        if (bundle != null)
                        {
                            DownloadedDomainList = ConvertToDomainList(DomainList.Get(bundle), domainDecrypter);
                        }
                        else
                        {
                            DownloadErrorMessage = "No domain list found";
                        }
                    }
                    catch (Exception ex)
                    {
                        DownloadErrorMessage = ex.Message;
                    }
                }
            }
        }

        string GenerateRemoteDomainList(StringBuilder buf)
        {
            buf.Length = 0;
            buf.Append(RemoteDomainListUrl);
            buf.Append("?r=");
            buf.Append(UnityEngine.Random.Range(0, int.MaxValue));
            return buf.ToString();
        }

        void SetWaitObjectActive(bool state)
        {
            for (int index = 0; index < waitObjects.Length; ++index)
            {
                waitObjects[index].SetActive(state);
            }
        }
        #endregion
    }
}
