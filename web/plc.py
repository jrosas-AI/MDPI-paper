import re
import time
import msvcrt
import os




def initialize_plc(plc):
    for i in [1, 2, 3, 50]:
        plc[f"TP{i}.IN"]        = "0"
        plc[f"TP{i}.IN.before"] = "0"
        plc[f"TP{i}.PT"]        = "0"
        plc[f"TP{i}.ET"]        = "0"
        plc[f"TP{i}.Q"]         = "0"
    for i in range(32):  
        plc[f"I{i}"]   = "0"
        plc[f"Q{i}"]    = "0"
        plc[f"M{i}"]    = "0"
    
    for byte in range(8):  
        for bit in range(8):  
            plc[f"M{byte}.{bit}"]   = "0"
            
        

def process_the_tp_timers(regs, deltaTme):        
    # Define the regex pattern to match the numbers after "TP" and before the dot
    pattern = re.compile(r'TP(\d+)\.')

    # Extract the numbers using a list comprehension
    numbers = [int(match.group(1)) for item in regs for match in [pattern.search(item)] if match]

    # Convert the list to a set to remove duplicates, then back to a list
    unique_numbers = list(set(numbers))

    for number in unique_numbers:
        timer_IN        = f"TP{number}.IN"
        timer_IN_before = f"TP{number}.IN.before"
        timer_ET        = f"TP{number}.ET"
        timer_Q         = f"TP{number}.Q"
        timer_PT        = f"TP{number}.PT"

        et = float(regs[timer_ET])
        pt = float(regs[timer_PT])

        if regs[timer_IN] == "1" and regs[timer_IN_before]!= "1":
            regs[timer_IN_before] = "1"
            regs[timer_ET] = "0" 
            regs[timer_Q]  = "1"

        if et >=  pt:
            regs[timer_Q] = "0"                               

        if regs[timer_IN_before] == "1" and et <   pt :
                et =  et + deltaTme

        if regs[timer_IN_before] == "1" and et >=  pt:            
                regs[timer_Q] = "0"
        if regs[timer_IN] == "0" and et >=  pt:
            et = 0 
            regs[timer_IN_before] = "0" 
        
        regs[timer_ET] = str(et)

def current_time_microseconds():
    return int(time.perf_counter() * 1e6)


def scan_cycle(plc_program, world_state):
    plc_registers = {}
    initialize_plc(plc_registers)
    
    nextDelta = 0
    
    while True:
        #if "I11" in world_state:
        #    print(f"plc_in={world_state["I11"]}")
        deltaTime1 = current_time_microseconds()         
        time.sleep(0.001)
        deltaTime2 = current_time_microseconds() 
        

        # Update plc_registers from world_state
        for i in range(32):
            in_key = f'I{i}'
            # q_key = f'Q{i}'
            plc_registers[in_key] = world_state.get(in_key, "0")
            # plc_registers[q_key] = world_state.get(q_key, "0")

        deltaTime = ( (deltaTime2 - deltaTime1) + nextDelta ) /1000  # convert from microseconds to milliseconds
        # print(deltaTime)
        process_the_tp_timers(plc_registers, deltaTime)
        plc_program(plc_registers)

        # Update world_state from plc_registers
        for i in range(32):
            # in_key = f'IN{i}'
            q_key = f'Q{i}'
            # world_state[in_key] = plc_registers[in_key]
            world_state[q_key] = plc_registers[q_key]

        deltaTime3 = current_time_microseconds()
        nextDelta = deltaTime3 - deltaTime2



iiiiii123456 = 0
def my_demo_plc_program_123456(plc):
    global iiiiii123456
    if plc["TP1.Q"] == "0":
        plc["TP1.IN"] = "1"
        plc["TP1.PT"] = "1000" # 3 seconds
        iiiiii123456 = iiiiii123456 + 1
        print(f"start next pulse {iiiiii123456}")
    if plc["TP1.Q"] == "1":
        plc["TP1.IN"] = "0"   
    if msvcrt.kbhit():
        print("Key pressed, exiting loop.")
        os._exit(0)
    print(f"ET={plc["TP1.ET"]}, IN={plc["TP1.IN"]}, PT={plc["TP1.PT"]}, Q={plc["TP1.Q"]}")


if __name__ == "__main__":

    world_state = {
        'IN1': '1',
        'Q1': '1',
        
    }

   # x1 = current_time_microseconds()
   # time.sleep(0.001)
   # x2 = current_time_microseconds()
   # print(x2-x1)    

    scan_cycle(my_demo_plc_program_123456, world_state)
