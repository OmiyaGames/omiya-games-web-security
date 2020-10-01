namespace OmiyaGames.Web.Security
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="StateChangeEventArgs.cs" company="Omiya Games">
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
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.2.0-preview.1<br/>
    /// <strong>Date:</strong> 9/29/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Event arguments used to provide information on verify events. See
    /// <seealso cref="WebDomainVerifier.OnBeforeVerifyWebDomain"/> and
    /// <seealso cref="WebDomainVerifier.OnAfterVerifyWebDomain"/>.
    /// </summary>
    public class VerifyEventArgs : System.EventArgs
    {
        /// <summary>
        /// Constructs new argument, setting <see cref="StartTime"/>
        /// to <seealso cref="UnityEngine.Time.realtimeSinceStartup"/>.
        /// </summary>
        public VerifyEventArgs()
        {
            StartTime = UnityEngine.Time.realtimeSinceStartup;
        }

        /// <summary>
        /// The real-time time mark the verification process started on.
        /// </summary>
        public float StartTime
        {
            get;
        }

        /// <summary>
        /// The real-time mark the verification process ended.
        /// If still processing, this variable will be set to <see cref="float.NaN"/>.
        /// </summary>
        public float EndTime
        {
            get;
            internal set;
        } = float.NaN;

        /// <summary>
        /// How long the verification process took, in seconds.
        /// If still processing, this variable will be set to <see cref="float.NaN"/>.
        /// </summary>
        public float VerificationDurationSeconds
        {
            get
            {
                float returnDurationSeconds = float.NaN;
                if ((float.IsNaN(EndTime) == false) && (EndTime > StartTime))
                {
                    returnDurationSeconds = EndTime - StartTime;
                }
                return returnDurationSeconds;
            }
        }
    }
}
