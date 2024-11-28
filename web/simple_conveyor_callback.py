import threading
import os
import time

import sys
sys.path.append("simulators")
import simulators.GPIOsim as GPIO
import simulators.iSTRwebServer as webServer

import simulators.keyboard as keyboard

import platform
system = platform.system()
if system == 'Windows':
    import msvcrt
elif system == 'Linux':
    import sys
    import select
else:
    print("Unsupported operating system")

decision = ""


def moveRight():
    GPIO.output(2, GPIO.LOW)
    GPIO.output(3, GPIO.HIGH)

def moveLeft():
    GPIO.output(2, GPIO.HIGH)
    GPIO.output(3, GPIO.LOW)

def stopConveyor():
    GPIO.output(2, GPIO.LOW)
    GPIO.output(3, GPIO.LOW)



def is_key_pressed():
    if system == 'Windows':
        return msvcrt.kbhit()
    else:        
        return select.select([sys.stdin], [], [], 0) == ([sys.stdin], [], [])
    
    

def getChar():
    if system == 'Windows':
        return msvcrt.getch().decode('utf-8')
    else:        
        return sys.stdin.read(1)


def keyboard_input():
    global decision
    try:        
        if is_key_pressed():
            key = getChar()
            print(f"pressed key: {key}")
            if key == "a":
                decision = "automatic_mode"
                print("automatic_mode")
            elif key == "m":
                decision = "manual_mode"    
                print("manual_mode")               
            elif key == "q":
                moveLeft()
            elif key == "e":
                moveRight()
            elif key == "w":
                stopConveyor()                    
            elif key == 'f':
                os._exit(0)
    except Exception as e:
            # Code to handle any other exception
            print(f"An error occurred: {e}")
            os._exit(0)
    return decision
    


def control_the_conveyor():
    print("q:left,  e:right,  w:stop,  a:automatic, m:manual, f:exit program")
    decision = ""
    while True:
        time.sleep(0.01)
        d = keyboard_input()
        decision = d if d != "" else decision
        

def isAtLeft_calback(GPIO, Value):   
    global decision
    print("is_at_left: " + decision)    
    if decision == "automatic_mode":
        moveRight()
    

def isAtRight_calback(GPIO, Value):    
    global decision
    print("is_at_right: " + decision)
    if decision == "automatic_mode":
        moveLeft()
    

def gpio_callback(gpio_id, val):
    print("changed gpio %s: %s" % (gpio_id, val))






if __name__ == "__main__":
    
    GPIO.setup(2, GPIO.OUTPUT, GPIO.LOW)
    GPIO.setup(3, GPIO.OUTPUT, GPIO.LOW)
    GPIO.setup(4, GPIO.INPUT)
    GPIO.setup(5, GPIO.INPUT)
    
    # start web server for the simulator
    server_thread = threading.Thread(target = webServer.run_server, args=('localhost', 8089))
    server_thread.start()

    # start keyboard interaction
    input_thread = threading.Thread(target=control_the_conveyor)
    input_thread.start()
    



    print("setting the interrupt callbacks....")
    # GPIO.add_interrupt_callback(2, gpio_callback)
    # GPIO.add_interrupt_callback(3, gpio_callback)
    GPIO.add_interrupt_callback(4, isAtLeft_calback)
    GPIO.add_interrupt_callback(5, isAtRight_calback)
    GPIO.wait_for_interrupts( epoll_timeout=10, threaded=True)


    GPIO.input(4)


    





    




    
