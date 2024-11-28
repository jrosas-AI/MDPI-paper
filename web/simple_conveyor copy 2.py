import threading
import os
import time
import sys
import cv2
import numpy as np
import base64
from datetime import datetime, timezone

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

def detect_red_objects(image):
    # Define more restrictive lower and upper bounds for the red color in RGB
    lower_red = np.array([0, 0, 150])
    upper_red = np.array([100, 100, 255])

    # Create a mask for red pixels
    mask = cv2.inRange(image, lower_red, upper_red)
    
    # Perform morphological operations to enhance the mask
    kernel = np.ones((3, 3), np.uint8)
    mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)
    mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, kernel)
    
    # Find contours
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    
    # Create an output image that is all white
    output_image = np.ones_like(image) * 255

    boxes = []
    for contour in contours:
        if cv2.contourArea(contour) > 100:  # Ignore small contours
            x, y, w, h = cv2.boundingRect(contour)
            boxes.append((x, y, w, h))
            cv2.rectangle(output_image, (x, y), (x + w, y + h), (0, 0, 255), 2)

    return output_image, boxes

def save_image(image, filename):
    os.makedirs('images', exist_ok=True)
    file_path = os.path.join('images', filename)
    success = cv2.imwrite(file_path, image)
    if success:
        print(f"Image saved successfully: {file_path}")
    else:
        print(f"Failed to save image: {file_path}")

def sanitize_filename(filename):
    return filename.replace(":", "_")

def calculate_velocity(image_before, image_now, timestamp_before, timestamp_now):
    # Detect red objects and get bounding boxes in both images
    red_strip_before, boxes_before = detect_red_objects(image_before)
    red_strip_now, boxes_now = detect_red_objects(image_now)

    # Save the masked images for debugging
    save_image(red_strip_before, sanitize_filename(f'red_strip_before_{timestamp_before}.png'))
    save_image(red_strip_now, sanitize_filename(f'red_strip_now_{timestamp_now}.png'))

    if not boxes_before or not boxes_now:
        print("No red objects detected")
        return 0.0

    # Use the first detected box for simplicity
    x_before, y_before, w_before, h_before = boxes_before[0]
    x_now, y_now, w_now, h_now = boxes_now[0]

    # Calculate the displacement in the x-direction
    x_displacement = x_now - x_before

    # Calculate the time difference in seconds
    time_before = datetime.fromisoformat(timestamp_before)
    time_now = datetime.fromisoformat(timestamp_now)
    time_difference = (time_now - time_before).total_seconds()

    # Check for valid time difference
    if time_difference == 0:
        print("Time difference is zero")
        return 0.0

    # Calculate the velocity in the x-direction (pixels per second)
    velocity_x = x_displacement / time_difference

    return velocity_x

def webcam_detect_speed():
    previous_speed = 0
    while True:
        time.sleep(0.01)
       
        timestampBefore = system_model.variableStates.get("timestampBefore") or datetime.now(timezone.utc).strftime("%Y-%m-%dT%H_%M_%S.%f")
        timestampNow = system_model.variableStates.get("timestampNow") or timestampBefore

        if timestampNow != timestampBefore:
            cameraBefore_base64 = system_model.variableStates.get("cameraBefore")
            cameraNow_base64 = system_model.variableStates.get("cameraNow")
            if cameraBefore_base64 and cameraNow_base64:
                try:
                    image_before = decode_base64_image(cameraBefore_base64)
                    image_now = decode_base64_image(cameraNow_base64)

                    velocity = calculate_velocity(image_before, image_now, timestampBefore, timestampNow)
                    
                    # If the calculated velocity is significantly lower than the previous speed, consider it stopped.
                    if system_model.getVariableValue("Q5") == "0":
                        velocity = 0

                    previous_speed = velocity
                    print(f"Calculated velocity: {velocity} pixels per second")
                    system_model.variableStates["sensorSpeed"] = str(velocity / 200.0)
                except ValueError as e:
                    print(f"Error decoding image: {e}")
                except Exception as e:
                    print(f"An unexpected error occurred: {e}")

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
    Kp = 1.0; Ti = 0.2; Tt=Ti; Ts=0.1; ao = Ts/Tt
    uiBefore = 0
    ui = 0
    bi = Kp * Ts/Ti
    ao = Ts/Tt
    print("pid controller started...")
    while True:
        time.sleep(Ts)

        referenceSpeed = float(system_model.getVariableValue("referenceSpeed") or "0")
        sensorSpeed = float(system_model.getVariableValue("speed") or "0")

        error = referenceSpeed - sensorSpeed

        up = Kp * error
        ud = 0
        utmp = up + uiBefore + ud
        u = clamp(utmp)
        ui = uiBefore + bi * error + ao * (u - utmp)
        uiBefore = ui

        system_model.variableStates["motorPower"] = u
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
    server_thread = threading.Thread(target=system_model.run_server, args=('localhost', 8089))
    server_thread.start()

    # start keyboard interaction (optional)
    input_thread = threading.Thread(target=keyboard_input)
    input_thread.start()

    threading.Thread(target=pid_controller).start()
    threading.Thread(target=webcam_detect_speed).start()
