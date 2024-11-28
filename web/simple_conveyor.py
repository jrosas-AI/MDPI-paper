import threading
import os
import time
import sys
import base64
import msvcrt
import numpy as np

import csv

def log_data(fruitGiven, fruitRecognized, filename='sampleData.csv'):
    # Verifica se o arquivo já existe
    file_exists = os.path.isfile(filename)
    
    # Abre o arquivo em modo append
    with open(filename, mode='a', newline='') as file:
        writer = csv.writer(file)
        
        # Escreve o cabeçalho se o arquivo não existe
        if not file_exists:
            writer.writerow(['Timestamp', 'FruitGiven', 'FruitRecognized'])
        
        # Escreve a linha com timestamp e dados
        timestamp = datetime.now().isoformat()
        writer.writerow([timestamp, fruitGiven, fruitRecognized])


from datetime import datetime, timezone
import cv2
from sklearn.linear_model import LinearRegression
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import img_to_array

sys.path.append("simulators")
import simulators.system_model as system_model
import simulators.keyboard as keyboard


import plc


def decode_base64_image(base64_string):
    if base64_string is None:
        raise ValueError("Base64 string is None")
    
    img_data = base64.b64decode(base64_string)
    np_arr = np.frombuffer(img_data, np.uint8)
    img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
    
    if img is None:
        raise ValueError("Failed to decode image")
    
    return img

def detect_red_bounding_box(image):
    # Define more restrictive lower and upper bounds for the red color in RGB
    lower_red = np.array([0, 0, 150])
    upper_red = np.array([100, 100, 255])

    # Create a mask for red pixels
    mask = cv2.inRange(image, lower_red, upper_red)
    
    # Perform morphological operations to enhance the mask
    kernel = np.ones((3, 3), np.uint8)
    mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)
    mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, kernel)
    
    # Find contours in the mask
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    
    if contours:
        # Find the largest contour and draw a bounding box around it
        largest_contour = max(contours, key=cv2.contourArea)
        x, y, w, h = cv2.boundingRect(largest_contour)
        cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)
        
        # Return the x position of the bounding box
        return image, x
    
    return image, None

def save_image(image, filename):
    # Clip the image
    # height, width, _ = image.shape
    # clipped_image = image[100:height-100, 300:width-300]

    os.makedirs('images', exist_ok=True)
    file_path = os.path.join('images', filename)
    success = cv2.imwrite(file_path, image)
    if success:
        print(f"Image saved successfully: {file_path}")
    else:
        print(f"Failed to save image: {file_path}")

def sanitize_filename(filename):
    return filename.replace(":", "_")

def process_image(image, timestamp):
    # Detect red bounding box in the image
    processed_image, x_position = detect_red_bounding_box(image)

    # Save the processed image for debugging
    # save_image(processed_image, sanitize_filename(f'processed_image_{timestamp}.png'))

    return x_position

def calculate_velocity_from_positions(positions):
    if len(positions) < 2:
        return 0.0

    # Prepare data for linear regression
    times = np.array([(pos[1] - positions[0][1]).total_seconds() for pos in positions]).reshape(-1, 1)
    x_positions = np.array([pos[0] for pos in positions]).reshape(-1, 1)

    # Perform linear regression
    model = LinearRegression()
    model.fit(times, x_positions)

    # The slope of the regression line is the velocity
    velocity = model.coef_[0][0]

    return velocity

position_history_to_use = []

def webcam_detect_position():
    global position_history_to_use
    position_history = []

    while True:
        time.sleep(0.01)
       
        timestampNow = system_model.variableStates.get("cameraTimestampNow") or datetime.now(timezone.utc).strftime("%Y-%m-%dT%H_%M_%S.%f")

        cameraNow_base64 = system_model.variableStates.get("cameraNow")
        if cameraNow_base64:
            try:
                image_now = decode_base64_image(cameraNow_base64)
                x_position = process_image(image_now, timestampNow)
                
                if x_position is not None:
                    system_model.variableStates["xPosition"] = str(x_position)

                    # Store the x_position and timestamp in the history
                    timestamp = datetime.fromisoformat(timestampNow)

                    if position_history and x_position < position_history[-1][0]:
                        position_history = []
                    
                    position_history.append((x_position, timestamp))
                    
                    # Ensure the history contains only the last 20 measurements
                    if len(position_history) > 20:
                        position_history.pop(0)
                    
                    if len(position_history) >= 2:
                        if (position_history[-1][1] - position_history[0][1]).total_seconds() > 0:
                            position_history_to_use = position_history[:]

                    # Calculate the velocity based on the position history
                    velocity = calculate_velocity_from_positions(position_history_to_use)

                    # print(f"Calculated velocity: {velocity/800.0} meters per second")
                    system_model.variableStates["sensorSpeed"] = str(velocity/800.0)
                else:
                    print("No red material detected")
                    system_model.variableStates["xPosition"] = "0"
            except ValueError as e:
                print(f"Error decoding image: {e}")
            except Exception as e:
                print(f"An unexpected error occurred: {e}")



def webcam_save_pictures():
    #the_camera = 3
    timestampBefore = [0,0,0,0]
    timestampNow = [0,0,0,0]
    while True:
        for the_camera in range(1,4):
            time.sleep(0.01)       
            timestampNow[the_camera] = system_model.variableStates.get(f"camera_{the_camera}TimestampNow") or ""        
            if(timestampNow[the_camera] != timestampBefore[the_camera]):
                #print(f"camera_{the_camera}TimestampNow:{timestampNow[the_camera]}")
                #print(f"timestampBefore{the_camera}:{timestampBefore[the_camera]}")
                cameraNow_base64 = system_model.variableStates.get(f"camera_{the_camera}Now")            
                if cameraNow_base64:
                    try:
                        image_now = decode_base64_image(cameraNow_base64)
                        filename = sanitize_filename(f'processed_image_{timestampNow[the_camera]}.png')
                        save_image(image_now, filename)            
                        timestampBefore[the_camera] = timestampNow[the_camera]
                    except ValueError as e:
                        print(f"Error decoding image: {e}")
                    except Exception as e:
                        print(f"An unexpected error occurred: {e}")



def replace_white_background_with_black(image):
    # Create a mask where white pixels are 255 and everything else is 0
    mask = cv2.inRange(image, np.array([240, 240, 240]), np.array([255, 255, 255]))
    
    # Set the mask to black
    image[mask == 255] = [0, 0, 0]
    
    return image



def classificar(model, classes, image):
    # Replace white background with black
    image = replace_white_background_with_black(image)
    
    # Resize the image to the size expected by the model
    image = cv2.resize(image, (150, 150))
    image = img_to_array(image)
    image = np.expand_dims(image, axis=0)
    image = image / 255.0  # Normalize the image
    
    # Make prediction
    prediction = model.predict(image)
    class_idx = np.argmax(prediction, axis=1)[0]
    class_label = classes[class_idx]
    
    return class_label


def webcam_classify_fruit():
    # Carregar o modelo previamente treinado
    #model = load_model('fruit_classifier_model.keras')
    # model = load_model('fruit_classifier_model_new.keras')
    # model = load_model('fruit_classifier_model_minimal_2.keras')
    model = load_model('fruit_classifier_model_new_more_fruit.keras')
    
    # Classes do modelo
    classes = ['apple', 'empty','orange', 'pear']
    timestampBefore = ["", "", "", ""]
    while True:
        time.sleep(0.01)
        for i in range(1, 4):
            # print(i)
            timestampNow = system_model.variableStates.get("camera_"+str(i)+"TimestampNow") or ""        
            if(timestampNow != timestampBefore[i]):
                # Simular o recebimento de uma imagem
                cameraNow_base64 = system_model.variableStates.get("camera_"+ str(i)+"Now")  # buffer da imagem
                if(cameraNow_base64 is None):
                    continue
                image_now = decode_base64_image(cameraNow_base64)  # converter para imagem
            
                # Classificar a imagem
                resultado = classificar(model, classes, image_now)
                print(f"Image detected Camera_{i}: {resultado}")
                system_model.variableStates["camera_"+str(i)+"_fruitType"] = resultado
                
                timestampBefore[i] = timestampNow
                # empty the buffer
                system_model.variableStates["camera_"+ str(i)+"Now"] = None

                if(i == 1): #save telemetry for camera 1
                    fruitGiven = system_model.getVariableValue("fruitGiven")
                    fruitRecognized = resultado
                    log_data(fruitGiven, fruitRecognized, filename='sampleData.csv')
                    

def keyboard_input():
    while True:
        time.sleep(0.01)
        try:
            if keyboard.is_key_pressed():
                key = keyboard.getChar()
                if key == "1":
                    if system_model.variableStates["s1"] == "1":
                        system_model.variableStates["s1"] = "0"
                    else:
                        system_model.variableStates["s1"] = "1"
                if key == 'f':
                    os._exit(0)
                if key == 't':
                    print("about to test images...")
                    # test_images()
        except Exception as e:
            print(f"An error occurred: {e}")
            os._exit(0)





def clamp(value):
    return max(0, min(value, 1))

def pid_controller():
    Kp = 1.0; Ti = 0.2; Tt=Ti; Ts=0.0; ao = Ts/Tt
    uiBefore = 0
    ui = 0
    bi = Kp * Ts/Ti
    ao = Ts/Tt

    referenceSpeed = 0
    sensorSpeed = 0
    print("pid controller started...")
    while True:
        time.sleep(Ts)

        referenceSpeed = float(system_model.getVariableValue("referenceSpeed") or referenceSpeed)
        sensorSpeed = float(system_model.getVariableValue("sensorSpeed") or sensorSpeed)
        sensorSpeed = clamp(sensorSpeed)

        error = referenceSpeed - sensorSpeed

        up = Kp * error
        ud = 0
        utmp = up + uiBefore + ud
        u = clamp(utmp)
        ui = uiBefore + bi * error + ao * (u - utmp)
        uiBefore = ui
        
        system_model.variableStates["motorPower"] = u
        system_model.variableStates["error"] = error
        system_model.variableStates["ui"] = ui
        system_model.variableStates["uiBefore"] = uiBefore
        system_model.variableStates["error"] = error
        system_model.variableStates["up"] = up
        system_model.variableStates["utmp"] = utmp

        system_model.variableStates["speed"] = u
        #if "I11" in system_model.variableStates:
        #    print(f"pid={system_model.variableStates["I11"]}")






def fruit_plc_controller(plc):
    
    
    if plc["I11"] =="1" and plc["M0.1"]=="0":
        plc["M0.1"] = "1"
        plc["Q5"] = "1"

    if plc["I11"] =="0":
        plc["Q5"] = "0"


    # conveyor moving and timer off
    if plc["Q5"] == "1" and plc["TP1.Q"] == "0":              
       plc["TP1.PT"]    = "5000" 
       # print("r2")
       # print("   "+ "started")
                
    plc["TP1.IN"] = plc["Q5"]
    # esta está errada
    if plc["Q5"] == "0":
        plc["TP1.ET"] = "0"

    # print(plc["TP1.ET"])
    # move conveyor
    if plc["Q5"] == "1" and float(plc["TP1.ET"])>0 and float(plc["TP1.ET"])<4000:
        plc["Q4"]       =  "0"
        # print("um")
        # print("r2")
        # print("   "+ "q4=1")

    # return    
    # give fruit    
    if plc["Q5"] == "1" and float(plc["TP1.ET"])   > 4000:
        plc["Q4"]       =  "1"   
        # print("zero")  
        # print("r3")
        # print("   "+ plc["TP1.ET"])
        # print("   "+ "q4=0")

    # restart pulse
    if plc["Q5"] == "1" and float(plc["TP1.ET"])   >= 5000:       
        plc["TP1.IN"]   = "0"

    # apples
    if( plc["I21"]=="1") and plc["M1"]=="0":        
        plc["M1"]="1"
        plc["M0.0"] = str(max(int(plc["Q5"]), int(plc["M0.0"])))
        plc["Q5"] = "0"
        plc["Q1"] = "1"
        # print("stop q5")

    # apples
    if(plc["I5"]=="1") and plc["M1"]=="1":        
        plc["M1"]="2"
        plc["Q1"] = "0" 

    # apples
    if(plc["I4"]=="1") and plc["M1"]=="2":        
        plc["M1"]="3"
        plc["Q5"] = plc["M0.0"] 
        print("start q5")
                   
    # apples        
    if( plc["I21"]=="0") and plc["I4"]=="1" and plc["M1"]=="3":
        plc["M1"]="0"




    # orange
    if( plc["I22"]=="1") and plc["M2"]=="0":        
        plc["M2"]="1"
        plc["M0.0"] = str(max(int(plc["Q5"]), int(plc["M0.0"])))
        plc["Q5"] = "0"
        plc["Q2"] = "1"
        # print("stop q5")

    # orange
    if(plc["I7"]=="1") and plc["M2"]=="1":        
        plc["M2"]="2"
        plc["Q2"] = "0"     

    # orange
    if(plc["I6"]=="1") and plc["M2"]=="2":        
        plc["M2"]="3"
        plc["Q5"] = plc["M0.0"] 
        # print("start q5")
                   
    # orange        
    if( plc["I22"]=="0") and plc["I6"]=="1" and plc["M2"]=="3":
        plc["M2"]="0"




    #pears
    if( plc["I23"]=="1") and plc["M3"]=="0":        
        plc["M3"]="1"
        plc["M0.0"] = str(max(int(plc["Q5"]), int(plc["M0.0"])))
        plc["Q5"] = "0"
        plc["Q3"] = "1"
        print("stop q5")

    #pears
    if(plc["I9"]=="1") and plc["M3"]=="1":        
        plc["M3"]="2"
        plc["Q3"] = "0"        
    # pears
    if(plc["I8"]=="1") and plc["M3"]=="2":        
        plc["M3"]="3"
        plc["Q5"] = plc["M0.0"] 
        print("start q5")
                   
    # pears        
    if( plc["I23"]=="0") and plc["I8"]=="1" and plc["M3"]=="3":
        plc["M3"]="0"
 



if __name__ == "__main__":
    # start web server for the simulator
    server_thread = threading.Thread(target=system_model.run_server, args=('localhost', 8089)).start()

    # start keyboard interaction (optional)
    input_thread = threading.Thread(target=keyboard_input).start()

    threading.Thread(target=pid_controller).start()
    threading.Thread(target=webcam_detect_position).start()
    # threading.Thread(target=webcam_save_pictures).start()

    threading.Thread(target=webcam_classify_fruit).start()
    

    threading.Thread(target=plc.scan_cycle, args=(fruit_plc_controller, system_model.variableStates)).start()
