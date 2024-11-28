import threading
import os
import time
import sys
import cv2
import numpy as np
import base64
from datetime import datetime, timezone
from sklearn.linear_model import LinearRegression
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import img_to_array

sys.path.append("simulators")
import simulators.system_model as system_model
import simulators.keyboard as keyboard


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
    timestampBefore = ""
    while True:
        time.sleep(0.1)       
        timestampNow = system_model.variableStates.get("camera_1TimestampNow") or ""        
        if(timestampNow != timestampBefore):
            print(f"camera_1TimestampNow:{timestampNow}")
            print(f"timestampBefore:{timestampBefore}")
            cameraNow_base64 = system_model.variableStates.get("camera_1Now")            
            if cameraNow_base64:
                try:
                    image_now = decode_base64_image(cameraNow_base64)
                    filename = sanitize_filename(f'processed_image_{timestampNow}.png')
                    save_image(image_now, filename)            
                    timestampBefore = timestampNow
                except ValueError as e:
                    print(f"Error decoding image: {e}")
                except Exception as e:
                    print(f"An unexpected error occurred: {e}")

def classificar(model, classes, image):
    # Redimensionar a imagem para o tamanho esperado pelo modelo
    # height, width, _ = image.shape
    # image = image[100:height-100, 300:width-300]
    image = cv2.resize(image, (150, 150))
    image = img_to_array(image)
    image = np.expand_dims(image, axis=0)
    image = image / 255.0  # Normalizar a imagem
    
    # Fazer a previsão
    prediction = model.predict(image)
    class_idx = np.argmax(prediction, axis=1)[0]
    class_label = classes[class_idx]
    
    return class_label

def webcam_classify_fruit():
    # Carregar o modelo previamente treinado
    model = load_model('fruit_classifier_model.keras')
    # Classes do modelo
    classes = ['apples', 'empty','lemons', 'onions', 'tomatoes']
    timestampBefore = ["", "", "", ""]
    while True:
        time.sleep(0.01)
        for i in range(1, 4):
            # print(i)
            timestampNow = system_model.variableStates.get("camera_"+str(i)+"TimestampNow") or ""        
            if(timestampNow != timestampBefore[i]):
                # Simular o recebimento de uma imagem
                cameraNow_base64 = system_model.variableStates.get("camera_"+ str(i)+"Now")  # buffer da imagem
                image_now = decode_base64_image(cameraNow_base64)  # converter para imagem
            
                # Classificar a imagem
                resultado = classificar(model, classes, image_now)
                print(f"Resultado da classificação_{i}: {resultado}")
                system_model.variableStates["camera_"+str(i)+"_fruitType"] = resultado
                timestampBefore[i] = timestampNow

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
                    test_images()
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
    print("pid controller started...")
    while True:
        time.sleep(Ts)

        referenceSpeed = float(system_model.getVariableValue("referenceSpeed") or "0")
        sensorSpeed = float(system_model.getVariableValue("sensorSpeed") or "0")
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

if __name__ == "__main__":
    # start web server for the simulator
    server_thread = threading.Thread(target=system_model.run_server, args=('localhost', 8089)).start()

    # start keyboard interaction (optional)
    input_thread = threading.Thread(target=keyboard_input).start()

    threading.Thread(target=pid_controller).start()
    threading.Thread(target=webcam_detect_position).start()
    # threading.Thread(target=webcam_save_pictures).start()

    threading.Thread(target=webcam_classify_fruit).start()
