// Copyright 2018 S520
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met :
//
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and / or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using OpenBveApi.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DetailManager {

    /// <summary>プラグインによって実装されるインタフェース。</summary>
    public class Plugin : IRuntime {

        // --- メンバ ---
        private List<IRuntime> plugins_;
        private List<LoadProperties> properties_list_;
        private Train train_;


        // --- インターフェース関数 ---

        /// <summary>プラグインが読み込まれたときに呼び出されます。</summary>
        /// <param name="properties">読み込み時にプラグインに提供されるプロパティ</param>
        /// <returns>プラグインが正常にロードされたかどうか。</returns>
        public bool Load(LoadProperties properties) {
            string dll_path = Assembly.GetExecutingAssembly().Location;
            string cfg_path = Path.ChangeExtension(dll_path, ".cfg");
            List<string> win32Dlls = new List<string>();
            LoadConfig load_config = LoadConfig.GetInstance();
            load_config.LoadCfgFile(cfg_path);
            plugins_ = new List<IRuntime>();
            properties_list_ = new List<LoadProperties>();
            for (int i = 0; i < load_config.plugin_path_.Count; i++) {
                Assembly plugin;
                try {
                    plugin = Assembly.LoadFile(load_config.plugin_path_[i]);
                } catch (BadImageFormatException) {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                        win32Dlls.Add(load_config.plugin_path_[i]);
                        continue;
                    } else {
                        return false;
                    }
                } catch {
                    return false;
                }
                Type[] types;
                try {
                    types = plugin.GetTypes();
                } catch {
                    return false;
                }
                foreach (Type type in types) {
                    if (typeof(IRuntime).IsAssignableFrom(type)) {
                        if (type.FullName == null) {
                            return false;
                        }
                        plugins_.Add(plugin.CreateInstance(type.FullName) as IRuntime);
                        properties_list_.Add(new LoadProperties(properties.PluginFolder, properties.TrainFolder, properties.PlaySound, properties.PlayCarSound, properties.AddMessage, properties.AddScore));
                    }
                }
            }
            if (win32Dlls.Count > 0) {
                string rockOnPath = Path.Combine(properties.PluginFolder, Win32DetailManagerPInvoke.MODULE_NAME);
                if (!File.Exists(rockOnPath)) {
                    File.WriteAllBytes(rockOnPath, Properties.Resources.RockOnDetailManager);
                }
                StringBuilder sb = new StringBuilder();
                string detailModulePath = Path.Combine(properties.PluginFolder, "detailmodules.txt");
                foreach (string dllPath in win32Dlls) {
                    string folder = properties.PluginFolder;
                    Uri pathUri = new Uri(dllPath);
                    if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                        folder += Path.DirectorySeparatorChar;
                    }
                    Uri folderUri = new Uri(folder);
                    Uri relativeUri = folderUri.MakeRelativeUri(pathUri);
                    string relativePath = Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
                    sb.AppendLine(relativePath);
                }
                if (File.Exists(detailModulePath)) {
                    string currentContent = File.ReadAllText(detailModulePath, Encoding.UTF8);
                    if (currentContent.Trim() != sb.ToString().Trim()) {
                        File.WriteAllText(detailModulePath, sb.ToString(), Encoding.UTF8);
                    }
                } else {
                    File.WriteAllText(detailModulePath, sb.ToString(), Encoding.UTF8);
                }
                plugins_.Add(new Win32DetailManagerPlugin());
                properties_list_.Add(new LoadProperties(properties.PluginFolder, properties.TrainFolder, properties.PlaySound, properties.PlayCarSound, properties.AddMessage, properties.AddScore));
            }
            for (int i = 0; i < plugins_.Count; i++) {
                if (!plugins_[i].Load(properties_list_[i])) { return false; }
            }
            int panel_length = 0;
            for (int i = 0; i < properties_list_.Count; i++) {
                if (properties_list_[i].Panel != null) {
                    panel_length += properties_list_[i].Panel.Length - panel_length;
                }
            }
            properties.Panel = new int[panel_length];
            this.train_ = new Train(properties.Panel);
            return true;
        }

        /// <summary>プラグインが解放されたときに呼び出されます。</summary>
        public void Unload() {
            foreach (IRuntime plugin in plugins_) {
                plugin.Unload();
            }
        }

        /// <summary>車両読み込み時に呼び出されます。</summary>
        /// <param name="specs">車両諸元</param>
        public void SetVehicleSpecs(VehicleSpecs specs) {
            foreach (IRuntime plugin in plugins_) {
                plugin.SetVehicleSpecs(specs);
            }
        }

        /// <summary>プラグインを初期化または再初期化する必要があるときに呼び出されます。</summary>
        /// <param name="mode">初期化モード</param>
        public void Initialize(InitializationModes mode) {
            foreach (IRuntime plugin in plugins_) {
                plugin.Initialize(mode);
            }
        }

        /// <summary>1フレームごとに呼び出されます。</summary>
        /// <param name="data">プラグインへ渡されるデータ</param>
        public void Elapse(ElapseData data) {
            foreach (IRuntime plugin in plugins_) {
                plugin.Elapse(data);
            }
            this.train_.Elapse(data, properties_list_);
        }

        /// <summary>レバーサが扱われたときに呼び出されます。</summary>
        /// <param name="reverser">新しいレバーサ位置</param>
        public void SetReverser(int reverser) {
            foreach (IRuntime plugin in plugins_) {
                plugin.SetReverser(reverser);
            }
        }

        /// <summary>運転者が力行ノッチを扱った際に呼び出されます。</summary>
        /// <param name="powerNotch">新しい力行ノッチ</param>
        public void SetPower(int powerNotch) {
            foreach (IRuntime plugin in plugins_) {
                plugin.SetPower(powerNotch);
            }
        }

        /// <summary>運転者がブレーキノッチを扱った際に呼び出されます。</summary>
        /// <param name="brakeNotch">新しいブレーキノッチ</param>
        public void SetBrake(int brakeNotch) {
            foreach (IRuntime plugin in plugins_) {
                plugin.SetBrake(brakeNotch);
            }
        }

        /// <summary>ATSキーが押されたときに呼び出されます。</summary>
        /// <param name="key">押されたATSキー</param>
        public void KeyDown(VirtualKeys key) {
            foreach (IRuntime plugin in plugins_) {
                plugin.KeyDown(key);
            }
        }

        /// <summary>ATSキーが離されたときに呼び出されます。</summary>
        /// <param name="key">離されたATSキー</param>
        public void KeyUp(VirtualKeys key) {
            foreach (IRuntime plugin in plugins_) {
                plugin.KeyUp(key);
            }
        }

        /// <summary>警笛が扱われたときに呼び出されます。</summary>
        /// <param name="type">警笛のタイプ</param>
        public void HornBlow(HornTypes type) {
            foreach (IRuntime plugin in plugins_) {
                plugin.HornBlow(type);
            }
        }

        /// <summary>Is called when the state of the doors changes.</summary>
        /// <param name="oldState">The old state of the doors.</param>
        /// <param name="newState">The new state of the doors.</param>
        public void DoorChange(DoorStates oldState, DoorStates newState) {
            foreach (IRuntime plugin in plugins_) {
                plugin.DoorChange(oldState, newState);
            }
        }

        /// <summary>現在の閉塞または次のいずれかの閉塞の信号が変更されたとき、または閉塞境界を越えたときに呼び出されます。</summary>
        /// <param name="signal">セクションごとの信号情報。 配列では、インデックス0は現在の閉塞、インデックス1は今後の閉塞などです。</param>
        /// <remarks>シグナル配列には少なくとも1つの要素があることが保証されています。 インデックス0以外の要素にアクセスする場合は、まず配列の境界をチェックする必要があります。</remarks>
        public void SetSignal(SignalData[] signal) {
            foreach (IRuntime plugin in plugins_) {
                plugin.SetSignal(signal);
            }
        }

        /// <summary>地上子を越えたときに呼び出されます。</summary>
        /// <param name="beacon">車上子で受け取った情報</param>
        public void SetBeacon(BeaconData beacon) {
            foreach (IRuntime plugin in plugins_) {
                plugin.SetBeacon(beacon);
            }
        }

        /// <summary>Is called when the plugin should perform the AI.</summary>
        /// <param name="data">The AI data.</param>
        public void PerformAI(AIData data) {
            foreach (IRuntime plugin in plugins_) {
                plugin.PerformAI(data);
            }
        }
    }
}
