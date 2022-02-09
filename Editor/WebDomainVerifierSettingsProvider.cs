using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using OmiyaGames.Global.Editor;
using OmiyaGames.Global.Settings.Editor;

namespace OmiyaGames.Web.Security.Editor
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="WebDomainVerifierSettingsProvider.cs" company="Omiya Games">
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
    /// <strong>Date:</strong> 6/29/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor for <see cref="WebDomainVerifier"/>.
    /// Appears under the Properties window.
    /// </summary>
    public class WebDomainVerifierSettingsProvider : SettingsProvider
    {
        public const string AssetFileName = "WebSecuritySettings.asset";
        public const string UxmlPath = "Packages/com.omiyagames.web.security/Editor/WebDomainVerifier.uxml";
        public const string DescriptionMessage = "Any domain string received from any" +
            " sources (in the list below or" +
            " a file downloaded from the \"" +
            WebDomainVerifier.RemoteDomainListHeader + "\" fields) will be compared" +
            " to the hostname of the website this application's WebGL build is" +
            " running on. For example, the hostname for \"www.google.com/search?q=help\"" +
            " is \"www.google.com\", while \"google.com/?search?q=help\" is" +
            " \"google.com\". The status of this comparison will be set to this script's" +
            " CurrentState property, and optional redirect the player to" +
            " a specified website.\n\n" +
            "Domain string can contain wild cards: * matches a string of characters," +
            " while ? matches zero or one character. For example, \"*.google.com\"" +
            " will match \"www.google.com\", \"o.google.com\", and \".google.com\"," +
            " while \"?.google.com\" will only match \"o.google.com\", and" +
            " \".google.com\"";

        private static readonly HelpBox DomainMustContainDescription = new HelpBox(DescriptionMessage, HelpBoxMessageType.None);
        /// <summary>
        /// The actual asset we're modifying.
        /// </summary>
        private SerializedObject webDomainVerifier;

        private class Styles
        {
            public static readonly GUIContent domainMustContain = new GUIContent("Domain Must Contain");

            public static readonly GUIContent forceRedirectIfDomainDoesntMatch = new GUIContent("Force Redirect If Domain Doesn't Match");
            public static readonly GUIContent redirectURL = new GUIContent("Redirect To");

            public static readonly GUIContent remoteDomainListUrl = new GUIContent("Remote Domain List URL");
            public static readonly GUIContent domainDecrypter = new GUIContent("Domain Decrypter");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scope"></param>
        public WebDomainVerifierSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        { }

        /// <summary>
        /// Registers this <see cref="SettingsProvider"/>.
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // Check if the asset file is available
            if (IsSettingsAvailable == false)
            {
                // Create the asset here.
                SettingsHelpers.CreateOmiyaGamesSettings<WebDomainVerifier>(AssetFileName);
            }

            // Create the settings provider
            var returnProvider = new WebDomainVerifierSettingsProvider(WebDomainVerifier.SidebarDisplayPath, SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            returnProvider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return returnProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public static WebDomainVerifier Asset
        {
            get => SettingsHelpers.GetOmiyaGamesSettings<WebDomainVerifier>(AssetFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsSettingsAvailable
        {
            get => File.Exists(SettingsHelpers.GetFullOmiyaGamesSettingsPath(AssetFileName));
        }

        /// <inheritdoc/>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);

            // Apply the UXML to the root element
            VisualElement fullTree = visualTree.CloneTree();
            rootElement.Add(fullTree);

            // Update class of helpbox to match DomainMustContain
            PropertyField domainMustContain = fullTree.Q<PropertyField>("DomainMustContain");
            foreach (string className in domainMustContain.GetClasses())
            {
                if (DomainMustContainDescription.ClassListContains(className) == false)
                {
                    DomainMustContainDescription.AddToClassList(className);
                }
            }

            // Insert the info box right above the "domain must contain" list
            VisualElement uiGroup = domainMustContain.parent;
            int indexToInsert = uiGroup.IndexOf(domainMustContain);
            uiGroup.Insert(indexToInsert, DomainMustContainDescription);

            // Update the toggles to enable/disable UI groups
            BindToggleToUiGroup(fullTree, "IsDownloadDomainListEnabled", "DownloadPropertiesGroup");
            BindToggleToUiGroup(fullTree, "IsRedirectingOnFail", "RedirectToGroup");

            // Bind the UXML to a serialized object
            // Note: this must be done last
            webDomainVerifier = new SerializedObject(Asset);
            rootElement.Bind(webDomainVerifier);
        }

        private static void BindToggleToUiGroup(VisualElement fullTree, string toggleName, string uiGroupName)
        {
            // Grab the respective controls
            Toggle checkbox = fullTree.Q<Toggle>(toggleName);
            VisualElement uiGroup = fullTree.Q<VisualElement>(uiGroupName);

            // First, update the UI group to match the toggle's state
            uiGroup.SetEnabled(checkbox.value);

            // Register the checkbox event to update the controls
            checkbox.RegisterValueChangedCallback((ChangeEvent<bool> isEnabled) => uiGroup.SetEnabled(isEnabled.newValue));
        }
    }
}
