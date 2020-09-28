using UnityEngine;
using UnityEditor;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Web.Security.Editor
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="WebDomainVerifierPropertyDrawer.cs" company="Omiya Games">
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
    /// <strong>Date:</strong> 9/27/2020<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <see cref="PropertyDrawer"/> for <see cref="WebDomainVerifier"/>.
    /// Auto-sets the variable if it's null.
    /// </summary>
    [CustomPropertyDrawer(typeof(WebDomainVerifier))]
    public class WebDomainVerifierPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent ResetButtonText = new GUIContent("Reset", "Click if this field is set incorrectly (e.g. <missing>).");
        public const float ResetButtonWidth = 50f;
        private static readonly GUIContent EditButtonText = new GUIContent("Edit", "Edits Domain Verifier via Project Settings dialog.");
        public const float EditButtonWidth = 50f;
        public const float HorizontalMargin = EditorHelpers.VerticalMargin;

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Check if the property has a reference
            if (property.objectReferenceValue == null)
            {
                // If there property is not set, reset
                Reset(property);
            }

            // Determine Rects for object field
            Rect objectPosition = position;
            objectPosition.width -= (HorizontalMargin + ResetButtonWidth + HorizontalMargin + EditButtonWidth);

            // Draw the (disabled) object field
            bool isGuiEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.ObjectField(objectPosition, property);
            GUI.enabled = isGuiEnabled;

            // Determine Rects for edit button
            Rect buttonPosition = position;
            buttonPosition.width = EditButtonWidth;
            buttonPosition.x = (objectPosition.xMax + HorizontalMargin);

            // Draw button
            if (GUI.Button(buttonPosition, EditButtonText) == true)
            {
                // Open Project Settings
                SettingsService.OpenProjectSettings(WebDomainVerifierSettingsProvider.ProjectSettingsPath);
            }

            // Determine Rects for reset button
            buttonPosition.width = ResetButtonWidth;
            buttonPosition.x += (EditButtonWidth + HorizontalMargin);

            // Draw button
            if (GUI.Button(buttonPosition, ResetButtonText) == true)
            {
                // Reset the asset
                Reset(property);
            }

            // End the property
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Sets the property to the current settings
        /// </summary>
        /// <param name="property"></param>
        private static void Reset(SerializedProperty property)
        {
            // Reset the asset
            property.objectReferenceValue = WebDomainVerifierSettingsProvider.Asset;
        }
    }
}
