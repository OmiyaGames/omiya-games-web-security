using System.Text;

namespace OmiyaGames.Web.Security
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="DomainList.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2016-2020 Omiya Games
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
    /// <strong>Version:</strong> 0.1.0-preview.1<br/>
    /// <strong>Date:</strong> 5/20/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper debug methods for OmiyaGames.Web.Security namespace.
    /// </summary>
    public static class DebugWebLocation
    {
        /// <summary>
        /// Generates a debug message based on info about <paramref name="webChecker"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="webChecker"></param>
        /// <returns></returns>
        public static string GetDebugMessage(StringBuilder builder, WebLocationChecker webChecker)
        {
            builder.AppendLine("Information according to the WebLocationChecker:");

            // Indicate the object's state
            int bulletNumber = 1;
            builder.Append(bulletNumber);
            builder.AppendLine(") the WebLocationChecker state is:");
            builder.AppendLine(webChecker.CurrentState.ToString());

            // Indicate the current domain information
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") this game's domain is:");
            builder.AppendLine(webChecker.RetrievedHostName);

            // List entries from the default domain list
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") the default domain list is:");
            int index = 0;
            for (; index < webChecker.DefaultDomainList.Length; ++index)
            {
                builder.Append("- ");
                builder.AppendLine(webChecker.DefaultDomainList[index]);
            }

            // Check if there's a download URL to list
            if (string.IsNullOrEmpty(webChecker.DownloadDomainsUrl) == false)
            {
                // Print that URL
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") downloaded a list of domains from:");
                builder.AppendLine(webChecker.DownloadDomainsUrl);

                // Check if there are any downloaded domains
                if (webChecker.DownloadedDomainList != null)
                {
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloaded the following domains:");
                    for (index = 0; index < webChecker.DownloadedDomainList.Length; ++index)
                    {
                        builder.Append("- ");
                        builder.AppendLine(webChecker.DownloadedDomainList[index]);
                    }
                }
                else
                {
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloading that list failed, however. The reason:");
                    builder.AppendLine(webChecker.DownloadErrorMessage);
                }
            }

            // Show unique list of domains
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") together, the full domain list is as follows:");
            foreach (string domain in webChecker.AllUniqueDomains.Keys)
            {
                builder.Append("- ");
                builder.AppendLine(domain);
            }

            // Show any errors
            if (string.IsNullOrEmpty(webChecker.DownloadErrorMessage) == false)
            {
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") Errors messages:");
                builder.AppendLine(webChecker.DownloadErrorMessage);
            }

            // Return URL
            return builder.ToString();
        }
    }
}
