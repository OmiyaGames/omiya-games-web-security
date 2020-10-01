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
    /// Event arguments used to indicate how a state has changed. See
    /// <seealso cref="WebDomainVerifier.OnBeforeStateChange"/> and
    /// <seealso cref="WebDomainVerifier.OnAfterStateChange"/>.
    /// </summary>
    public class StateChangeEventArgs : System.EventArgs
    {
        /// <summary>
        /// Constructor to set the read-only properties.
        /// </summary>
        /// <param name="oldState">Sets <see cref="OldState"/>.</param>
        /// <param name="newState">Sets <see cref="NewState"/>.</param>
        public StateChangeEventArgs(WebDomainVerifier.State oldState, WebDomainVerifier.State newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        /// <summary>
        /// The state prior to the change.
        /// </summary>
        public WebDomainVerifier.State OldState
        {
            get;
        }

        /// <summary>
        /// The latest value this event is setting it's state into.
        /// </summary>
        public WebDomainVerifier.State NewState
        {
            get;
        }
    }
}
