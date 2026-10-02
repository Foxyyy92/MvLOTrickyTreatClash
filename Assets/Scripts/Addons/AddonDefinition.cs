using Newtonsoft.Json;
using NSMB.Utilities;
using System;
using UnityEngine;

namespace NSMB.Addons {
    [CreateAssetMenu(fileName = "New Addon Definition", menuName = "Addon/Addon Definition", order = 0)]
    public class AddonDefinition : ScriptableObject {
        public string AddonId;
        public string DisplayName;
        public string Author;
        [TextArea(4, 100)] public string Description;
        public Texture2D IconTexture;

        [HideInInspector] public string LastVersion;

        public AddonBuildDefinition ToBuildDefinition(Guid guid, GameVersion version) {
            return new AddonBuildDefinition {
                ReleaseGuid = guid,
                GameVersion = version,
                AddonId = AddonId,
                DisplayName = DisplayName,
                Author = Author,
                ReleaseVersion = LastVersion,
                Description = Description,
                IconTexture = IconTexture,
                iconNeedsDisposal = false,
            };
        }
    }

    [Serializable]
    public class AddonBuildDefinition : IEquatable<AddonBuildDefinition>, IDisposable {
        public Guid ReleaseGuid { get; set; }
        public GameVersion GameVersion { get; set; }
        public string AddonId { get; set; }
        public string DisplayName { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string ReleaseVersion { get; set; }
        [JsonIgnore] public Texture2D IconTexture { get; set; }
        [JsonIgnore] public string FullName => $"{DisplayName} ({ReleaseVersion})";

        [JsonIgnore] internal bool iconNeedsDisposal = true;

        ~AddonBuildDefinition() {
            if (IconTexture && iconNeedsDisposal) {
                Debug.LogError($"Memory Leak! AddonReleaseDefinition ({DisplayName}) IconTexture was not disposed!");
                Dispose();
            }
        }

        public bool Equals(AddonBuildDefinition other) {
            return ReleaseGuid == other.ReleaseGuid;
        }

        public void Dispose() {
            if (IconTexture && iconNeedsDisposal) {
                UnityEngine.Object.Destroy(IconTexture);
            }
        }
    }
}