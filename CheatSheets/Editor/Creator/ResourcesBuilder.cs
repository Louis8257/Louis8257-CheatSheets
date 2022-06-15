using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CheatSheets.Creator {
    internal static class ResourcesBuilder {

        const string ROOT_PATH = "CheatSheets/Core/Templates/";

        public const string FILE_EXTENSION_CS   = ".cs";
        public const string FILE_EXTENSION_UXML = ".uxml";

        /// <summary>
        /// <para>As text file.</para>
        /// </summary>
        public const string TEMPLATE_PATH_SCRIPT = ROOT_PATH + "script";
        
        /// <summary>
        /// <para>As text file.</para>
        /// </summary>
        public const string TEMPLATE_PATH_UXML = ROOT_PATH + "visualTree";

        static Dictionary<string, ScriptDraft> scriptDrafts;

        /// <summary>
        /// <para><b>Will always perform errors checks for protecting stored files!</b></para
        /// </summary>
        /// <returns>True if the operation was successfull.</returns>
        public static bool Create ( List<CheatSheet> cheatSheets ) {
            ResourcesBuilder.OnInitialize();

            #region "Check errors first!"
            bool hasErrors = false;
            foreach ( CheatSheet cheatSheet in cheatSheets ) {
                string[] errors = cheatSheet.CheckErrors();
                if ( errors != null && errors.Length > 0) {
                    string s = "";
                    foreach ( string error in errors ) {
                        s += error + "\n";
                    }
                    Debug.LogError(string.Format("[Cheat Sheets] Couldn't create {0}! Here's the errors:\n{1}", cheatSheet.name, s));
                    hasErrors = true;
                }
            }
            if ( hasErrors ) {
                return false;
            }
            #endregion

            #region "Create resources"
            // UXML (optimisation: will also create script drafts)
            TextAsset uxml = Resources.Load<TextAsset>(TEMPLATE_PATH_UXML);
            foreach ( CheatSheet cheatSheet in cheatSheets ) {
                string folderPath = ResourcesBuilder.GetFolder(cheatSheet);

                foreach ( CheatSheetPage page in cheatSheet.pages ) {
                    string uxmlPath = folderPath + Path.DirectorySeparatorChar + page.GetCleanName() + FILE_EXTENSION_UXML;
                    ResourcesBuilder.CreateFile(uxmlPath, uxml.text);
                    AssetDatabase.ImportAsset(ResourcesBuilder.ToRelativePath(uxmlPath));

                    ScriptDraft draft;
                    ResourcesBuilder.scriptDrafts.TryGetValue(cheatSheet.GetCleanName(false), out draft);
                    if (draft == null) {
                        draft = ResourcesBuilder.RegisterNewScript(folderPath, cheatSheet, page);
                    }
                    draft.attributes.Add(page.GetAttributeLine());
                }
            }

            // CS
            foreach ( ScriptDraft draft in scriptDrafts.Values ) {
                draft.Build();
            }
            #endregion
            
            ResourcesBuilder.OnFinish();
            return true;
        }

        static void OnInitialize () {
            ResourcesBuilder.scriptDrafts = new Dictionary<string, ScriptDraft>();
        }

        static void OnFinish () {
            ResourcesBuilder.scriptDrafts = null;
        }

        static ScriptDraft RegisterNewScript ( string folderPath, CheatSheet cheatSheet, CheatSheetPage page ) {
            string      name       = cheatSheet.GetCleanName(false);
            ScriptDraft draft      = new ScriptDraft(folderPath, name);
            ResourcesBuilder.scriptDrafts.Add(name, draft);
            return draft;
        }

        /// <summary>
        /// <para>Will get the folder path with the cheat sheet name.</para>
        /// <para>Will create a folder if can't be found.</para>
        /// </summary>
        /// <param name="cheatSheet"></param>
        /// <returns></returns>
        public static string GetFolder ( CheatSheet cheatSheet ) {
            string folderPath = cheatSheet.resourcePath.GetValue().Trim() + Path.DirectorySeparatorChar + cheatSheet.GetCleanName();
            if ( !Directory.Exists(folderPath) ) {
                Directory.CreateDirectory(folderPath);
                Debug.Log("[Cheat Sheets] Created new folder: " + folderPath);
            }
            return folderPath;
        }

        public static void CreateFile ( string path, string content ) { 
            StreamWriter scriptWriter = File.CreateText(path);
            scriptWriter.Write(content);
            scriptWriter.Close();
        }

        public static string ToRelativePath ( string absolutePath ) {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length); ;
        }

    }

    internal class ScriptDraft {

        static TextAsset textAsset;

        /// <summary>
        /// <para>Path of the folder.</para>
        /// </summary>
        public string folderPath = "";

        /// <summary>
        /// <para><b>Must have no whitespaces.</b></para>
        /// </summary>
        public string name = "";

        public List<string> attributes = new List<string>();

        /// <summary>
        /// <para>Path of the file.</para>
        /// </summary>
        string path {
            get {
                return folderPath + Path.DirectorySeparatorChar + this.name + "CheatSheet" + ResourcesBuilder.FILE_EXTENSION_CS;
            }
        }

        static string scriptTemplate {
            get {
                if ( textAsset == null ) {
                    textAsset = Resources.Load<TextAsset>(ResourcesBuilder.TEMPLATE_PATH_SCRIPT);
                }
                return textAsset.text;
            }
        }

        public ScriptDraft ( string folderPath, string name ) {
            this.folderPath = folderPath;
            this.name       = name;
        }

        public void Build () {
            string template = scriptTemplate;

            string attributes = "";

            bool createNewLine = false;
            foreach ( string attribute in this.attributes ) {
                attributes += (createNewLine ? "\n" : "") + attribute;
                createNewLine = true;
            }

            template = template.Replace("{0}", attributes);
            template = template.Replace("{1}", this.name);

            ResourcesBuilder.CreateFile(this.path, template);
            AssetDatabase.ImportAsset(ResourcesBuilder.ToRelativePath(this.path));
        }

    }
}