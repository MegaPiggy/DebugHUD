using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using UnityEngine;

namespace DebugHUD {
    public class DebugHUD : ModBehaviour {
        private void Awake() {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        private void Start() {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(DebugHUD)} is loaded!", MessageType.Success);

            // Wait for scene load
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene == OWScene.SolarSystem || loadScene == OWScene.EyeOfTheUniverse) {
                    enableDebugHUD();
                } else {
                    disableDebugHUD();
                }
            };

        }

        private void disableDebugHUD() {
            debugMode = false;
            ModHelper.Console.WriteLine($"Debug HUD disabled", MessageType.Warning);
        }

        private void enableDebugHUD() {
            LoadConfig();
            debugMode = true;
            ModHelper.Console.WriteLine($"Debug HUD enabled", MessageType.Success);

            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) {
                this._playerForceDetector = playerObject.GetRequiredComponentInChildren<ForceDetector>();
                this._playerController = playerObject.GetRequiredComponent<PlayerCharacterController>();
            }
            GlobalMessenger.AddListener("DisableGUI", new Callback(this.OnDisableGUI));
        }

        private bool debugMode = false;
        private List<DebugText> debugTexts = new List<DebugText>();
        private Dictionary<string, bool> Config = new Dictionary<string, bool>();

        // Subscribe to config changes
        public override void Configure(IModConfig config) {
            UpdateDebugTexts(config);
        }

        private void UpdateDebugTexts(IModConfig config) {
            bool timeScale = config.GetSettingsValue<bool>("Time Scale");
            bool timeRemaining = config.GetSettingsValue<bool>("Time Remaining");
            bool loopCount = config.GetSettingsValue<bool>("Loop Count");
            bool pauseFlags = config.GetSettingsValue<bool>("Pause Flags");
            bool inputMode = config.GetSettingsValue<bool>("Input Mode");
            bool inputModeStack = config.GetSettingsValue<bool>("Input Mode Stack");
            bool netForceAccel = config.GetSettingsValue<bool>("Net Force Acceleration");
            bool gForce = config.GetSettingsValue<bool>("G-Force");
            bool loadTime = config.GetSettingsValue<bool>("Load Time");
            bool resolutionScale = config.GetSettingsValue<bool>("Resolution Scale");

            Config["Time Scale"] = timeScale;
            Config["Time Remaining"] = timeRemaining;
            Config["Loop Count"] = loopCount;
            Config["Pause Flags"] = pauseFlags;
            Config["Input Mode"] = inputMode;
            Config["Input Mode Stack"] = inputModeStack;
            Config["Net Force Acceleration"] = netForceAccel;
            Config["G-Force"] = gForce;
            Config["Load Time"] = loadTime;
            Config["Resolution Scale"] = resolutionScale;
        }

        private void LoadConfig() {
            bool timeScale = ModHelper.Config.GetSettingsValue<bool>("Time Scale");
            bool timeRemaining = ModHelper.Config.GetSettingsValue<bool>("Time Remaining");
            bool loopCount = ModHelper.Config.GetSettingsValue<bool>("Loop Count");
            bool pauseFlags = ModHelper.Config.GetSettingsValue<bool>("Pause Flags");
            bool inputMode = ModHelper.Config.GetSettingsValue<bool>("Input Mode");
            bool inputModeStack = ModHelper.Config.GetSettingsValue<bool>("Input Mode Stack");
            bool netForceAccel = ModHelper.Config.GetSettingsValue<bool>("Net Force Acceleration");
            bool gForce = ModHelper.Config.GetSettingsValue<bool>("G-Force");
            bool loadTime = ModHelper.Config.GetSettingsValue<bool>("Load Time");
            bool resolutionScale = ModHelper.Config.GetSettingsValue<bool>("Resolution Scale");

            Config["Time Scale"] = timeScale;
            Config["Time Remaining"] = timeRemaining;
            Config["Loop Count"] = loopCount;
            Config["Pause Flags"] = pauseFlags;
            Config["Input Mode"] = inputMode;
            Config["Input Mode Stack"] = inputModeStack;
            Config["Net Force Acceleration"] = netForceAccel;
            Config["G-Force"] = gForce;
            Config["Load Time"] = loadTime;
            Config["Resolution Scale"] = resolutionScale;
        }

        private List<DebugText> CreateDebugTexts() {
            List<DebugText> debugList = new();

            float y = 10f;

            if (Config["Time Scale"]) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Time Scale: " + Mathf.Round(Time.timeScale * 100f) / 100f));
                y += 15f;
            }
            if (Config["Time Remaining"]) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), string.Concat(new object[] {
                    "Time Remaining: ",
                    Mathf.Floor(TimeLoop.GetSecondsRemaining() / 60f),
                    ":",
                    Mathf.Round(TimeLoop.GetSecondsRemaining() % 60f * 100f / 100f)
                })));
                y += 15f;
            }
            if (Config["Loop Count"]) { 
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Loop Count: " + TimeLoop.GetLoopCount()));
                y += 15f;
            }
            if (Config["Pause Flags"]) {
                debugList.Add(new DebugText(new Rect(10f, y, 90f, 40f), "PauseFlags: "));
                debugList.Add(new DebugText(new Rect(100f, y, 50f, 40f), "MENU\n" + (OWTime.IsPaused(OWTime.PauseType.Menu) ? "TRUE " : "FALSE")));
                debugList.Add(new DebugText(new Rect(150f, y, 50f, 40f), "LOAD\n" + (OWTime.IsPaused(OWTime.PauseType.Loading) ? "TRUE " : "FALSE")));
                debugList.Add(new DebugText(new Rect(200f, y, 50f, 40f), "READ\n" + (OWTime.IsPaused(OWTime.PauseType.Reading) ? "TRUE " : "FALSE")));
                debugList.Add(new DebugText(new Rect(250f, y, 50f, 40f), "SLP\n" + (OWTime.IsPaused(OWTime.PauseType.Sleeping) ? "TRUE " : "FALSE")));
                debugList.Add(new DebugText(new Rect(300f, y, 50f, 40f), "INIT\n" + (OWTime.IsPaused(OWTime.PauseType.Initializing) ? "TRUE " : "FALSE")));
                debugList.Add(new DebugText(new Rect(350f, y, 50f, 40f), "STRM\n" + (OWTime.IsPaused(OWTime.PauseType.Streaming) ? "TRUE " : "FALSE")));
                debugList.Add(new DebugText(new Rect(400f, y, 50f, 40f), "SYS\n" + (OWTime.IsPaused(OWTime.PauseType.System) ? "TRUE " : "FALSE")));
                y += 30f;
            }
            if (Config["Input Mode"]) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Input Mode: " + OWInput.GetInputMode().ToString()));
                y += 15f;
            }
            if (Config["Input Mode Stack"]) {
                this._inputModeArray = OWInput.GetInputModeStack();
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Input Mode Stack: "));
                int num = 150;
                for (int i = 0; i < this._inputModeArray.Length; i++) {
                    GUI.Label(new Rect((float)num, y, 200f, 20f), this._inputModeArray[i].ToString());
                    num += 75;
                }
                y += 15f;
            }
            if (Config["Net Force Acceleration"] && this._playerForceDetector != null) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Net Force Accel: " + Mathf.Round(this._playerForceDetector.GetForceAcceleration().magnitude * 100f) / 100f));
                y += 15f;
            }
            if (Config["G-Force"]) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "G-Force: " + Mathf.Round(this._gForce * 100f) / 100f));
                y += 15f;
            }
            if (Config["Load Time"]) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Load Time: " + LoadTimeTracker.GetLatestLoadTime()));
                y += 15f;
            }
            if (Config["Resolution Scale"] && DynamicResolutionManager.isActive) {
                debugList.Add(new DebugText(new Rect(10f, y, 200f, 20f), "Resolution Scale: " + DynamicResolutionManager.currentResolutionScale));
            }


            return debugList;
        }

        private ForceDetector _playerForceDetector;
        private PlayerCharacterController _playerController;
        private InputMode[] _inputModeArray;
        private float _gForce;

        private void OnDestroy() {
            GlobalMessenger.RemoveListener("DisableGUI", new Callback(this.OnDisableGUI));
        }

        private void OnDisableGUI() {
            Object.Destroy(this);
        }

        private void LateUpdate() {
            if (this._playerController != null) {
                this._gForce = this._playerController.GetNormalAccelerationScalar();
            }
        }

        private void OnGUI() {
            if (GUIMode.IsHiddenMode() || PlayerState.UsingShipComputer()) {
                return;
            }

            if (debugMode) {
                foreach (DebugText debugText in CreateDebugTexts()) {
                    GUI.Label(debugText.Rect, debugText.Text);
                }
            }
        }
    }

    public class DebugText {
        public Rect Rect { get; set; }
        public string Text { get; set; }

        public DebugText(Rect rect, string text) {
            Rect = rect;
            Text = text;
        }
    }
}