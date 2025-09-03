import requests, dotenv, os
import dotenv

dotenv.load_dotenv()
IPADDRESS = os.getenv("Gaming_PC_IPAdress")
FILE_NAME = "./audio_file/user_audio.wav"

#send audio to gaming PC Server and get text
def STT_GamingPC(fileName = FILE_NAME):
  
    #For making json, define data
    files = {'file': open(fileName, 'rb')}

    #send audio
    response = requests.post(f'http://{IPADDRESS}:5000/stt-poland/', files=files)
    response_data = response.json()
    try:
        return response_data["text"]
    except:
        return "TextToSpeech method has a Error!"
    

if __name__ == "__main__":

    import time

    run_s = time.time()
    print(STT_GamingPC())
    run_e = time.time()
    print("time : ", (run_e - run_s))