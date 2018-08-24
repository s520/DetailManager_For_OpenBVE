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

using System.Collections.Generic;
using System.IO;

namespace DetailManager {

    /// <summary>
    /// 設定を読み込むクラス
    /// </summary>
    internal class LoadConfig {

        // --- メンバ ---
        private static LoadConfig load_config_;
        internal List<string> plugin_path_ { get; private set; }


        // --- コンストラクタ ---
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static LoadConfig() {
            load_config_ = new LoadConfig();
        }

        /// <summary>
        /// 新しいインスタンスを作成する
        /// </summary>
        private LoadConfig() {
            plugin_path_ = new List<string>();
        }


        // --- 関数 ---
        /// <summary>
        /// インスタンスを取得する関数
        /// </summary>
        /// <returns>インスタンス</returns>
        internal static LoadConfig GetInstance() {
            return load_config_;
        }

        /// <summary>
        /// ファイルから設定を読み込む関数
        /// </summary>
        /// <param name="file_path">ファイルパス</param>
        internal void LoadCfgFile(string file_path) {
            if (!File.Exists(file_path)) { return; }
            string file_directory = System.IO.Path.GetDirectoryName(file_path);
            string[] lines = File.ReadAllLines(file_path);
            if (lines.Length == 0) { return; }
            for (int i = 0; i < lines.Length; i++) {
                string path = lines[i];
                path = path.Replace('/', Path.DirectorySeparatorChar);
                path = path.Replace('\\', Path.DirectorySeparatorChar);
                path = System.IO.Path.Combine(file_directory, path);
                if (File.Exists(path)) {
                    plugin_path_.Add(path);
                }
            }
        }
    }
}
