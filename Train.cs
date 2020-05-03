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
using System.Collections.Generic;

namespace DetailManager {

    /// <summary>このプラグインによってシミュレートされる列車を表すクラス</summary>
    internal class Train {

        // --- パネルとサウンド ---
        /// <summary>パネルに渡す値</summary>
        internal int[] Panel;


        // --- コンストラクタ ---
        /// <summary>新しいインスタンスを作成する</summary>
        /// <param name="panel">パネルに渡す値</param>
        internal Train(int[] panel) {
            this.Panel = panel;
        }


        // --- 関数 ---
        /// <summary>1フレームごとに呼び出される関数</summary>
        /// <param name="data">The data.</param>
        internal void Elapse(ElapseData data, List<LoadProperties> properties) {
            if (data.ElapsedTime.Seconds > 0.0 && data.ElapsedTime.Seconds < 1.0) {
                // --- パネル初期化 ---
                for (int i = 0; i < this.Panel.Length; i++) {
                    this.Panel[i] = 0;
                }

                // --- パネル ---
                for (int i = 0; i < properties.Count; i++) {
                    if (properties[i].Panel != null) {
                        for (int j = 0; j < properties[i].Panel.Length; j++) {
                            if (properties[i].Panel[j] != 0) {
                                this.Panel[j] = properties[i].Panel[j];
                            }
                        }
                    }
                }
            }
        }
    }
}
