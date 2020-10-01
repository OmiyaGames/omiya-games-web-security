using UnityEngine;
using System.Text;
using System.Collections;

// Namespace WebDomainVerifier is in
using OmiyaGames.Web.Security;

namespace Sample
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebDomainInfo.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2020 Omiya Games
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
    /// <date>5/15/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Provides debugging information for the WebGL build, and its host information.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Date:</strong> 6/13/2018<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// <item>
    /// <term>
    /// <strong>Date:</strong> 9/27/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Updated to use <see cref="WebDomainVerifier"/>.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class WebDomainInfo : MonoBehaviour
    {
        const string ForWebGlMessage = "This menu is meant to provide information for WebGL builds.";
        const string LoadingMessage = "Loading web information...";

        [SerializeField]
        UnityEngine.UI.Text infoLabel;
        /// <summary>
        /// This field is auto-set by <see cref="WebDomainVerifier"/>'s <see cref="UnityEditor.PropertyDrawer"/>.
        /// </summary>
        [SerializeField]
        WebDomainVerifier domainVerifier;

        // Use this for initialization
        IEnumerator Start()
        {
            // By default, set the label to indicate this isn't a web build.
            infoLabel.text = ForWebGlMessage;

            // Check if this is a web build
            if ((domainVerifier != null) && (Application.platform == RuntimePlatform.WebGLPlayer))
            {
                // Print that we're loading
                infoLabel.text = LoadingMessage;

                // Start the verification process
                yield return StartCoroutine(domainVerifier.VerifyWebDomain());

                // Update the reason for this dialog to appear
                infoLabel.text = DebugWebDomainInfo.GetDebugMessage(domainVerifier, new StringBuilder());
            }
        }
    }
}
