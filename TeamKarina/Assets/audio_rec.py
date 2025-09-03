# -*- coding: utf-8 -*-
import sys
sys.dont_write_bytecode = True

import pyaudio
import wave
import time
import threading

CHUNK = 1024
FORMAT = pyaudio.paInt16
CHANNELS = 1
RATE = 44100
OUTPUT_FILENAME = "./audio_file/user_audio.wav"

class REC:
    def __init__(self):
        self.python_audio = pyaudio.PyAudio()
        self.stream = self.python_audio.open(
            format=FORMAT,
            channels=CHANNELS,
            rate=RATE,
            input=True,
            frames_per_buffer=CHUNK
        )
        self.frames = []
        self.is_recording = False
        self.recording_thread = None
        self.start_time = 0

    def _record_loop(self):
        """バックグラウンドで実行される録音ループ"""
        while self.is_recording:
            data = self.stream.read(CHUNK)
            self.frames.append(data)

    def start_rec(self):
        """録音を開始する（外部から呼び出す）"""
        if self.is_recording:
            return {"status": "error", "message": "Already recording."}

        self.is_recording = True
        self.frames = [] # 前回の録音データをクリア
        self.start_time = time.time()
        
        # 録音ループを別スレッドで開始
        self.recording_thread = threading.Thread(target=self._record_loop)
        self.recording_thread.start()
        
        return {"status": "success", "message": "Recording started."}

    def end_rec(self):
        """録音を停止し、ファイルを保存する（外部から呼び出す）"""
        if not self.is_recording:
            return {"status": "error", "message": "Not recording."}

        self.is_recording = False
        # 録音スレッドが終了するのを待つ
        self.recording_thread.join()
        
        finish_time = time.time()
        duration = finish_time - self.start_time
        
        with wave.open(OUTPUT_FILENAME, 'wb') as wf:
            wf.setnchannels(CHANNELS)
            wf.setsampwidth(self.python_audio.get_sample_size(FORMAT))
            wf.setframerate(RATE)
            wf.writeframes(b''.join(self.frames))
        
        return {
            "status": "success",
            "message": "Recording saved.",
            "filename": OUTPUT_FILENAME,
            "duration": round(duration, 2)
        }

    def close(self):
        """リソースを解放する"""
        self.stream.stop_stream()
        self.stream.close()
        self.python_audio.terminate()

    # maintainメソッドは変更なし
    def maintain(self):
        # ... (元のコードと同じ)
        pass

# test code
if __name__ == "__main__":
    recorder = REC()
    
    print("Press Enter to start recording...")
    input()
    recorder.start_rec()

    print("Recording... Press Enter to stop.")
    input()
    result = recorder.end_rec()
    print(result)
    
    recorder.close()