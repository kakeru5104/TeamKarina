import audio_rec
import SpeechToText_GamingPC as stg


try:
    REC = audio_rec.REC()

    while input("Start recording? (y only): ").strip().lower() != 'y':
        pass
    REC.start_rec()

    while input("Recording Now... End recording? (y only): ").strip().lower() != 'y':
        pass
    REC.end_rec()

finally:
    REC.close()

print(stg.STT_GamingPC())