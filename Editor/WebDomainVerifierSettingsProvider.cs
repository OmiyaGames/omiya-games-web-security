using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using OmiyaGames.Common.Editor;
using OmiyaGames.Global.Editor;
using UnityEditor.UIElements;
using System;

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
        public const string AssetFileName = "WebDomainVerifier.asset";
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
            WebDomainVerifierSettingsProvider returnProvider =
                new WebDomainVerifierSettingsProvider("Project/Omiya Games/Web Security", SettingsScope.Project);

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
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.omiyagames.web.security/Editor/WebDomainVerifier.uxml");
            VisualElement fullTree = visualTree.CloneTree();
            rootElement.Add(fullTree);

            // Grab the DownloadDomainListUrl text field, and bind it to the appropriate field.
            TextField serializedTextField = fullTree.Query<TextField>("DownloadDomainListUrl").First();
            serializedTextField.bindingPath = "remoteDomainListUrl";

            // Grab the RedirectUrl text field, and bind it to the appropriate field.
            serializedTextField = fullTree.Query<TextField>("RedirectUrl").First();
            serializedTextField.bindingPath = "redirectURL";

            // Grab Decrypter object field, and bind it to the appropriate field.
            ObjectField serializedObjectField = fullTree.Query<ObjectField>("DomainListDecrypter").First();
            // Note: can't figure out how to get this type in XML properly, so in-code it is.
            serializedObjectField.objectType = typeof(Cryptography.StringCryptographer);
            serializedObjectField.bindingPath = "domainDecrypter";

            // FIXME: grab checkbox groups.  Somehow.

            // Bind the UXML to a serialized object
            // Note: this must be done last
            webDomainVerifier = new SerializedObject(Asset);
            rootElement.Bind(webDomainVerifier);
        }
    }
}
