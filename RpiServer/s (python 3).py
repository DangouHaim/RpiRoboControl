from __future__ import print_function
from AMSpi import AMSpi
from os import system, name
from threading import Thread

import socket
import psutil
import time

Connected = False
Debug = True

def clear(): 
  
    # for windows 
    if name == 'nt': 
        _ = system('cls') 
  
    # for mac and linux(here, os.name is 'posix') 
    else: 
        _ = system('clear')

def reprint(data):
	clear()
	print(data)
	time.sleep(1)

def cpu_state():
    while Connected:
    	reprint("CPU: " + str(psutil.cpu_percent()) + "%")

def stop_all():
	amspi.set_74HC595_pins(7, 8, 25)
	amspi.set_L293D_pins(22, 10, 9, 11)

	amspi.stop_dc_motors([amspi.DC_Motor_1, amspi.DC_Motor_2, amspi.DC_Motor_3, amspi.DC_Motor_4])

# For BOARD pin numbering use AMSpi(True) otherwise BCM is used
with AMSpi() as amspi:

	stop_all()

	thread = Thread(target = cpu_state, args=())

	

	while True:
		try:
			time.sleep(0.1)
			
			stop_all()
			
			s = socket.socket()
			s.bind(("", 9090))
			s.listen(10)
			reprint("Server started...")
	
			c, a = s.accept()
			reprint("Connected...")

			if not Debug:
				thread.start()

			while True:
				time.sleep(0.1)
				
				data = c.recv(1024)
					
				if data:
					data = data.strip()
					if Debug:
						print(data)
						
					if (data.lower() == "w-down"):
						amspi.run_dc_motors([amspi.DC_Motor_3, amspi.DC_Motor_4], clockwise=False)

					if (data.lower() == "s-down" ):
						amspi.run_dc_motors([amspi.DC_Motor_3, amspi.DC_Motor_4])

					if (data.lower() == "a-down" ):
						amspi.run_dc_motors([amspi.DC_Motor_3])
						amspi.run_dc_motors([amspi.DC_Motor_4], clockwise=False)

					if (data.lower() == "d-down" ):
						amspi.run_dc_motors([amspi.DC_Motor_4])
						amspi.run_dc_motors([amspi.DC_Motor_3], clockwise=False)

					if (data.lower() == "up"):
						stop_all()

		except:
			stop_all()
			Connected = False
			s.close()