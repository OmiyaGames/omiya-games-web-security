using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using OmiyaGames.Global.Editor;

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
    /// Appears under inspector when selecting the asset.
    /// </summary>
    [CustomEditor(typeof(WebDomainVerifier))]
    public class WebDomainVerifierEditor : UnityEditor.Editor
    {
        public const string HelpInfo = "This is the common Web Domain Verifier asset used in both scripts and project settings window. Please do not move or delete it from the project. If using a version control tool, remember to add this asset into the project as well.";
        private static readonly GUIContent EditButtonText = new GUIContent("Edit in Project Settings");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Post help box
            EditorGUILayout.HelpBox(HelpInfo, MessageType.Info);

            // Draw button
            if (GUILayout.Button(EditButtonText) == true)
            {
                // Open Project Settings
                SettingsService.OpenProjectSettings(WebDomainVerifier.ProjectSettingsPath);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
