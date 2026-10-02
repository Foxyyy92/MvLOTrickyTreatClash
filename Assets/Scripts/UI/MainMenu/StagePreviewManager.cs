using NSMB.Utilities;
using Quantum;
using System;
using UnityEngine;

namespace NSMB.UI.MainMenu {
    public class StagePreviewManager : MonoBehaviour {

        //---Serialized Variables
        [SerializeField] private Camera targetCamera;
        [SerializeField] private GameObject noPreviewPrefab;

        //---Private Variables
        private AssetRef<VersusStageData> currentActiveStageData;
        private GameObject currentActivePrefab;

        public void Start() {
            PreviewRandomStage();
            QuantumEvent.Subscribe<EventPlayerAdded>(this, OnPlayerAdded);
            QuantumEvent.Subscribe<EventRulesChanged>(this, OnRulesChanged);
        }

        public void PreviewRandomStage() {
            UnityEngine.Random.InitState((int) DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var maps = AssetRepository<Map>.AllAssetRefs;
            PreviewStage(maps[UnityEngine.Random.Range(0, maps.Count)]);
        }

        public void PreviewStage(VersusStageData stage) {
            if (currentActiveStageData == stage) {
                return;
            }
            currentActiveStageData = stage;

            if (currentActivePrefab) {
                Destroy(currentActivePrefab);
            }

            GameObject prefabToSpawn;
            if (stage && stage.MainMenuPreviewPrefab) {
                prefabToSpawn = stage.MainMenuPreviewPrefab;
            } else {
                prefabToSpawn = noPreviewPrefab;
            }
            
            currentActivePrefab = Instantiate(prefabToSpawn, transform);
            var origin = currentActivePrefab.GetComponentInChildren<MainMenuCameraOrigin>();
            if (origin) {
                targetCamera.transform.position = origin.transform.position;
            } else {
                targetCamera.transform.position = Vector3.zero;
            }
        }

        public void PreviewStage(AssetRef<Map> map) {
            PreviewStage(QuantumUnityDB.GetGlobalAsset<VersusStageData>(QuantumUnityDB.GetGlobalAsset(map).UserAsset.Id));
        }

        private unsafe void ChangeStage(QuantumGame game) {
            Frame f = game.Frames.Predicted;
            if (f.Global->Rules.ChooseMode == StageChooseMode.Random) {
                PreviewStage((VersusStageData) null);
            } else {
                PreviewStage(f.Global->Rules.Stage);
            }
        }

        private unsafe void OnPlayerAdded(EventPlayerAdded e) {
            if (e.Game.PlayerIsLocal(e.Player)) {
                ChangeStage(e.Game);
            }
        }

        private unsafe void OnRulesChanged(EventRulesChanged e) {
            ChangeStage(e.Game);
        }
    }
}
