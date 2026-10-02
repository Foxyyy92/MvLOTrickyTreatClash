using System;
using System.Collections.Generic;
using UnityEngine;

namespace NSMB.UI.Translation {
    [CreateAssetMenu(fileName = "New Translation Source", menuName = "Addon/Translation Source", order = 1)]
    public class ScriptableTranslationSource : ScriptableObject, ITranslationSource {

        public int Priority => SourcePriority;
        public bool IsRTL => IsRtl;

        public int SourcePriority = 1;
        public string LocaleCode = "en-us";
        public bool IsRtl = false;
        public ScriptableTranslationEntry[] Translations;

        //---Private Variables
        private Dictionary<string, string> loadedTranslations;

        public void OnEnable() {
            Reload();
        }

        public bool TryGetTranslation(string key, out string result) {
            if (loadedTranslations.TryGetValue(key, out result)) {
                return true;
            }

            result = null;
            return false;
        }

        public void Reload() {
            if (loadedTranslations == null) {
                loadedTranslations = new(Translations.Length);
            } else {
                loadedTranslations.Clear();
            }

            foreach (var translation in Translations) {
                loadedTranslations[translation.Key] = translation.Text;
            }
        }
 
        public int CompareTo(object other) {
            if (other is not ITranslationSource otherSource) {
                return 0;
            }
            return Priority.CompareTo(otherSource.Priority);
        }

        public bool Equals(ITranslationSource other) {
            if (other is not ScriptableTranslationSource otherFileSource) {
                return false;
            }
            return this == otherFileSource;
        }

        [Serializable]
        public struct ScriptableTranslationEntry {
            public string Key;
            public string Text;
        }
    }
}