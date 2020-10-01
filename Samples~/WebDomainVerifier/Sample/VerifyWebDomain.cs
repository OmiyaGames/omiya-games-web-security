using UnityEngine;
using System.Text;
using System.Collections;

// Namespace WebDomainVerifier is in
using OmiyaGames.Web.Security;

namespace Sample
{
    ///-----------------------------------------------------------------------
    /// <copyright file="VerifyWebDomain.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2020-2020 Omiya Games
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
    /// <date>9/29/2020</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Updates labels on the status of verifying the .
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
    /// <strong>Date:</strong> 9/29/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class VerifyWebDomain : MonoBehaviour
    {
        const string ForWebGlMessage = "This menu is meant to provide information for WebGL builds.";
        const string VerificationStartMessage = "Started verification process!";
        const string VerificationEndMessage = "Verification finished!";
        const string StatusMessage = "Latest status: ";

        /// <summary>
        /// This field is auto-set by <see cref="WebDomainVerifier"/>'s <see cref="UnityEditor.PropertyDrawer"/>.
        /// </summary>
        [SerializeField]
        WebDomainVerifier domainVerifier;
        /// <summary>
        /// Label to indicate change in the coroutine process.
        /// </summary>
        [SerializeField]
        UnityEngine.UI.Text coroutineStatusLabel;
        /// <summary>
        /// Label to indicate status info made available via events.
        /// </summary>
        [SerializeField]
        UnityEngine.UI.Text eventsStatusLabel;

        readonly StringBuilder coroutineText = new StringBuilder();
        readonly StringBuilder eventsText = new StringBuilder();

        // Use this for initialization
        IEnumerator Start()
        {
            // By default, set the label to indicate this isn't a web build.
            coroutineStatusLabel.text = ForWebGlMessage;
            eventsStatusLabel.text = ForWebGlMessage;

            // Check if this is a web build
            if ((domainVerifier != null) && (Application.platform == RuntimePlatform.WebGLPlayer))
            {
                // Bind to the appropriate events
                domainVerifier.OnBeforeVerifyWebDomain += IndicateVerificationStarted;
                domainVerifier.OnAfterVerifyWebDomain += IndicateVerificationEnded;
                domainVerifier.OnAfterStateChange += IndicateStatusChanged;

                // Start building text that we started the verification process
                coroutineText.AppendLine(VerificationStartMessage);
                coroutineText.Append(StatusMessage);
                coroutineText.Append(domainVerifier.CurrentState);

                // Update the label
                coroutineStatusLabel.text = coroutineText.ToString();

                // Start the verification process
                yield return StartCoroutine(domainVerifier.VerifyWebDomain());

                // Append verification finished
                coroutineText.AppendLine();
                coroutineText.AppendLine(VerificationEndMessage);
                coroutineText.Append(StatusMessage);
                coroutineText.Append(domainVerifier.CurrentState);

                // Update the label
                coroutineStatusLabel.text = coroutineText.ToString();
            }

            // For the purpose of saving memory, clear out the string builders
            coroutineText.Clear();
            eventsText.Clear();
        }

        void IndicateVerificationStarted(WebDomainVerifier source, VerifyEventArgs args)
        {
            // Indicate verification started
            eventsText.AppendLine(VerificationStartMessage);

            // Append status info
            eventsText.Append(StatusMessage);
            eventsText.Append(source.CurrentState);
            eventsText.AppendLine();

            // Append time info
            eventsText.Append("Start time: ");
            eventsText.Append(args.StartTime);
            eventsText.Append(" seconds since app has started");

            // Apply text to label
            eventsStatusLabel.text = eventsText.ToString();
        }

        void IndicateVerificationEnded(WebDomainVerifier source, VerifyEventArgs args)
        {
            // Indicate verification finished
            eventsText.AppendLine();
            eventsText.AppendLine(VerificationEndMessage);

            // Append status info
            eventsText.Append(StatusMessage);
            eventsText.Append(domainVerifier.CurrentState);
            eventsText.AppendLine();

            // Append duration info
            eventsText.Append("Processing duration: ");
            eventsText.Append(args.VerificationDurationSeconds);
            eventsText.Append(" seconds");

            // Update the label
            eventsStatusLabel.text = eventsText.ToString();
        }

        void IndicateStatusChanged(WebDomainVerifier source, StateChangeEventArgs args)
        {
            // Indicate verification finished
            eventsText.AppendLine();
            eventsText.Append("Status changed to: ");
            eventsText.Append(args.NewState);

            // Apply text to label
            eventsStatusLabel.text = eventsText.ToString();
        }
    }
}
