using NSMB.UI.Translation;
using NSMB.Utilities.Extensions;
using Quantum;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NSMB.UI.MainMenu.Submenus.InRoom {

    public class StageChooseModeChangeableRule : ChangeableRule {
        
        //---Static Variables
        private static readonly StageChooseMode[] Values = (StageChooseMode[]) Enum.GetValues(typeof(StageChooseMode));

        //---Properties
        public override bool CanIncreaseValue => (byte) value < Values.Length - 1;
        public override bool CanDecreaseValue => (byte) value > 0;

        //---Serializable Variables
        [SerializeField] private StageChooseModeTranslationKeys[] translationKeys;
        [SerializeField] private ScrollRect scroll;

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            scroll.verticalNormalizedPosition = scroll.ScrollToCenter((RectTransform) transform, false);
        }

        protected override void IncreaseValueInternal() {
            byte byteValue = (byte) value;
            value = (byte) Mathf.Min(byteValue + 1, Values.Length - 1);

            if (byteValue != (byte) value) {
                cursorSfx.Play();
                SendCommand();
            }
        }

        protected override void DecreaseValueInternal() {
            byte byteValue = (byte) value;
            value = (byte) Mathf.Max(byteValue - 1, 0);

            if (byteValue != (byte) value) {
                cursorSfx.Play();
                SendCommand();
            }
        }

        protected override void UpdateLabel() {
            TranslationManager tm = GlobalController.Instance.translationManager;
            StageChooseMode enumValue = (StageChooseMode) (byte) value;
            label.text = labelPrefix + tm.GetTranslation(translationKeys.First(tk => tk.ChooseMode == enumValue).Key);
        }

        private unsafe void SendCommand() {
            CommandChangeRules cmd = new CommandChangeRules {
                EnabledChanges = ruleType,
                ChooseMode = (StageChooseMode) (byte) value
            };

            QuantumGame game = QuantumRunner.DefaultGame;
            PlayerRef host = game.Frames.Predicted.Global->Host;
            if (game.PlayerIsLocal(host)) {
                game.SendCommand(game.GetLocalPlayerSlots()[game.GetLocalPlayers().IndexOf(host)], cmd);
            }
        }
    }

    [Serializable]
    public struct StageChooseModeTranslationKeys {
        public StageChooseMode ChooseMode;
        public string Key;
    }
}