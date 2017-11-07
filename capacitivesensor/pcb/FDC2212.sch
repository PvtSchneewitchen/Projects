EESchema Schematic File Version 2
LIBS:power
LIBS:device
LIBS:transistors
LIBS:conn
LIBS:linear
LIBS:regul
LIBS:74xx
LIBS:cmos4000
LIBS:adc-dac
LIBS:memory
LIBS:xilinx
LIBS:microcontrollers
LIBS:dsp
LIBS:microchip
LIBS:analog_switches
LIBS:motorola
LIBS:texas
LIBS:intel
LIBS:audio
LIBS:interface
LIBS:digital-audio
LIBS:philips
LIBS:display
LIBS:cypress
LIBS:siliconi
LIBS:opto
LIBS:atmel
LIBS:contrib
LIBS:valves
LIBS:FDC2212-cache
EELAYER 25 0
EELAYER END
$Descr A4 11693 8268
encoding utf-8
Sheet 1 1
Title ""
Date ""
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
$Comp
L FDC2212 U3
U 1 1 56CB5FD7
P 4550 2600
F 0 "U3" H 4900 1850 60  0000 C CNN
F 1 "FDC2212" H 4900 2550 60  0000 C CNN
F 2 "Housings_DFN_QFN:DFN-12-1EP_4x4mm_Pitch0.5mm" H 4900 1900 60  0001 C CNN
F 3 "" H 4900 1900 60  0000 C CNN
	1    4550 2600
	1    0    0    -1  
$EndComp
$Comp
L STM32F030F4P6 U1
U 1 1 56CB606F
P 2400 2500
F 0 "U1" H 2850 1350 60  0000 C CNN
F 1 "STM32F030F4P6" H 2800 2450 60  0000 C CNN
F 2 "Housings_SSOP:TSSOP-20_4.4x6.5mm_Pitch0.65mm" H 2400 2500 60  0001 C CNN
F 3 "" H 2400 2500 60  0000 C CNN
	1    2400 2500
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR01
U 1 1 56CB6156
P 6550 3300
F 0 "#PWR01" H 6550 3050 50  0001 C CNN
F 1 "GND" H 6550 3150 50  0000 C CNN
F 2 "" H 6550 3300 50  0000 C CNN
F 3 "" H 6550 3300 50  0000 C CNN
	1    6550 3300
	1    0    0    -1  
$EndComp
$Comp
L +3.3V #PWR02
U 1 1 56CB618B
P 3850 2100
F 0 "#PWR02" H 3850 1950 50  0001 C CNN
F 1 "+3.3V" H 3850 2240 50  0000 C CNN
F 2 "" H 3850 2100 50  0000 C CNN
F 3 "" H 3850 2100 50  0000 C CNN
	1    3850 2100
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR03
U 1 1 56CB61C1
P 3850 3500
F 0 "#PWR03" H 3850 3250 50  0001 C CNN
F 1 "GND" H 3850 3350 50  0000 C CNN
F 2 "" H 3850 3500 50  0000 C CNN
F 3 "" H 3850 3500 50  0000 C CNN
	1    3850 3500
	1    0    0    -1  
$EndComp
$Comp
L +3.3V #PWR04
U 1 1 56CB62E1
P 1800 1850
F 0 "#PWR04" H 1800 1700 50  0001 C CNN
F 1 "+3.3V" H 1800 1990 50  0000 C CNN
F 2 "" H 1800 1850 50  0000 C CNN
F 3 "" H 1800 1850 50  0000 C CNN
	1    1800 1850
	1    0    0    -1  
$EndComp
$Comp
L +3.3V #PWR05
U 1 1 56CB6397
P 7700 2450
F 0 "#PWR05" H 7700 2300 50  0001 C CNN
F 1 "+3.3V" H 7700 2590 50  0000 C CNN
F 2 "" H 7700 2450 50  0000 C CNN
F 3 "" H 7700 2450 50  0000 C CNN
	1    7700 2450
	1    0    0    -1  
$EndComp
$Comp
L INDUCTOR L1
U 1 1 56CB64E2
P 6050 2600
F 0 "L1" V 6000 2600 50  0000 C CNN
F 1 "18uH" V 6150 2600 50  0000 C CNN
F 2 "Resistors_SMD:R_1210_HandSoldering" H 6050 2600 50  0001 C CNN
F 3 "" H 6050 2600 50  0000 C CNN
	1    6050 2600
	0    1    1    0   
$EndComp
$Comp
L C C2
U 1 1 56CB6541
P 6000 3150
F 0 "C2" H 6025 3250 50  0000 L CNN
F 1 "33pF" H 6025 3050 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 6038 3000 50  0001 C CNN
F 3 "" H 6000 3150 50  0000 C CNN
	1    6000 3150
	0    1    1    0   
$EndComp
$Comp
L C C1
U 1 1 56CB6A39
P 6050 2250
F 0 "C1" H 6075 2350 50  0000 L CNN
F 1 "33pF" H 6075 2150 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 6088 2100 50  0001 C CNN
F 3 "" H 6050 2250 50  0000 C CNN
	1    6050 2250
	0    1    1    0   
$EndComp
$Comp
L INDUCTOR L2
U 1 1 56CB6A7D
P 6050 3500
F 0 "L2" V 6000 3500 50  0000 C CNN
F 1 "18uH" V 6150 3500 50  0000 C CNN
F 2 "Resistors_SMD:R_1210_HandSoldering" H 6050 3500 50  0001 C CNN
F 3 "" H 6050 3500 50  0000 C CNN
	1    6050 3500
	0    1    1    0   
$EndComp
$Comp
L GND #PWR06
U 1 1 56CB6F63
P 5600 3700
F 0 "#PWR06" H 5600 3450 50  0001 C CNN
F 1 "GND" H 5600 3550 50  0000 C CNN
F 2 "" H 5600 3700 50  0000 C CNN
F 3 "" H 5600 3700 50  0000 C CNN
	1    5600 3700
	1    0    0    -1  
$EndComp
$Comp
L +3.3V #PWR07
U 1 1 56CB6F83
P 5150 3700
F 0 "#PWR07" H 5150 3550 50  0001 C CNN
F 1 "+3.3V" H 5150 3840 50  0000 C CNN
F 2 "" H 5150 3700 50  0000 C CNN
F 3 "" H 5150 3700 50  0000 C CNN
	1    5150 3700
	-1   0    0    1   
$EndComp
$Comp
L ST485 U2
U 1 1 56CB7A06
P 2400 4100
F 0 "U2" H 2650 3550 60  0000 C CNN
F 1 "ST485" H 2650 4050 60  0000 C CNN
F 2 "Housings_SOIC:SOIC-8_3.9x4.9mm_Pitch1.27mm" H 2400 4100 60  0001 C CNN
F 3 "" H 2400 4100 60  0000 C CNN
	1    2400 4100
	1    0    0    -1  
$EndComp
$Comp
L +3.3V #PWR08
U 1 1 56CB7F6B
P 3300 3900
F 0 "#PWR08" H 3300 3750 50  0001 C CNN
F 1 "+3.3V" H 3300 4040 50  0000 C CNN
F 2 "" H 3300 3900 50  0000 C CNN
F 3 "" H 3300 3900 50  0000 C CNN
	1    3300 3900
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR09
U 1 1 56CB8009
P 3300 4800
F 0 "#PWR09" H 3300 4550 50  0001 C CNN
F 1 "GND" H 3300 4650 50  0000 C CNN
F 2 "" H 3300 4800 50  0000 C CNN
F 3 "" H 3300 4800 50  0000 C CNN
	1    3300 4800
	1    0    0    -1  
$EndComp
$Comp
L R R4
U 1 1 56CB8429
P 4000 2550
F 0 "R4" V 4080 2550 50  0000 C CNN
F 1 "1K8R" V 4000 2550 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 3930 2550 50  0001 C CNN
F 3 "" H 4000 2550 50  0000 C CNN
	1    4000 2550
	1    0    0    -1  
$EndComp
$Comp
L R R3
U 1 1 56CB852C
P 4250 2550
F 0 "R3" V 4330 2550 50  0000 C CNN
F 1 "1K8R" V 4250 2550 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 4180 2550 50  0001 C CNN
F 3 "" H 4250 2550 50  0000 C CNN
	1    4250 2550
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR010
U 1 1 56CB8B38
P 3000 1850
F 0 "#PWR010" H 3000 1600 50  0001 C CNN
F 1 "GND" H 3000 1700 50  0000 C CNN
F 2 "" H 3000 1850 50  0000 C CNN
F 3 "" H 3000 1850 50  0000 C CNN
	1    3000 1850
	-1   0    0    1   
$EndComp
$Comp
L GND #PWR011
U 1 1 56CB9959
P 7600 4200
F 0 "#PWR011" H 7600 3950 50  0001 C CNN
F 1 "GND" H 7600 4050 50  0000 C CNN
F 2 "" H 7600 4200 50  0000 C CNN
F 3 "" H 7600 4200 50  0000 C CNN
	1    7600 4200
	-1   0    0    1   
$EndComp
$Comp
L +3.3V #PWR012
U 1 1 56CB9988
P 8700 4250
F 0 "#PWR012" H 8700 4100 50  0001 C CNN
F 1 "+3.3V" H 8700 4390 50  0000 C CNN
F 2 "" H 8700 4250 50  0000 C CNN
F 3 "" H 8700 4250 50  0000 C CNN
	1    8700 4250
	1    0    0    -1  
$EndComp
$Comp
L CONN_01X04 P2
U 1 1 56CB9FD2
P 5800 4400
F 0 "P2" H 5800 4650 50  0000 C CNN
F 1 "CONN_01X04" V 5900 4400 50  0000 C CNN
F 2 "Pin_Headers:Pin_Header_Straight_1x04" H 5800 4400 50  0001 C CNN
F 3 "" H 5800 4400 50  0000 C CNN
	1    5800 4400
	-1   0    0    1   
$EndComp
$Comp
L +5V #PWR013
U 1 1 56CBA661
P 4550 3950
F 0 "#PWR013" H 4550 3800 50  0001 C CNN
F 1 "+5V" H 4550 4090 50  0000 C CNN
F 2 "" H 4550 3950 50  0000 C CNN
F 3 "" H 4550 3950 50  0000 C CNN
	1    4550 3950
	1    0    0    -1  
$EndComp
$Comp
L CONN_01X04 P3
U 1 1 56CBA93C
P 5800 5000
F 0 "P3" H 5800 5250 50  0000 C CNN
F 1 "CONN_01X04" V 5900 5000 50  0000 C CNN
F 2 "Pin_Headers:Pin_Header_Straight_1x04" H 5800 5000 50  0001 C CNN
F 3 "" H 5800 5000 50  0000 C CNN
	1    5800 5000
	-1   0    0    1   
$EndComp
$Comp
L CONN_01X02 P5
U 1 1 56CBAE8D
P 8500 2300
F 0 "P5" H 8500 2450 50  0000 C CNN
F 1 "CONN_01X02" V 8600 2300 50  0000 C CNN
F 2 "Connect:SMB_Straight" H 8500 2300 50  0001 C CNN
F 3 "" H 8500 2300 50  0000 C CNN
	1    8500 2300
	1    0    0    -1  
$EndComp
$Comp
L CONN_01X02 P4
U 1 1 56CBB34E
P 8500 1750
F 0 "P4" H 8500 1900 50  0000 C CNN
F 1 "CONN_01X02" V 8600 1750 50  0000 C CNN
F 2 "Connect:SMB_Straight" H 8500 1750 50  0001 C CNN
F 3 "" H 8500 1750 50  0000 C CNN
	1    8500 1750
	1    0    0    -1  
$EndComp
$Comp
L OPAMP-2CH U4
U 1 1 56CB6000
P 6750 2500
F 0 "U4" H 7100 1950 60  0000 C CNN
F 1 "OPAMP-2CH" H 7100 2450 60  0000 C CNN
F 2 "Housings_SSOP:MSOP-8_3x3mm_Pitch0.65mm" H 7100 2250 60  0001 C CNN
F 3 "" H 7100 2250 60  0000 C CNN
	1    6750 2500
	1    0    0    -1  
$EndComp
$Comp
L LED D2
U 1 1 56CBBB66
P 3850 3850
F 0 "D2" H 3850 3950 50  0000 C CNN
F 1 "LED" H 3850 3750 50  0000 C CNN
F 2 "LEDs:LED_0603" H 3850 3850 50  0001 C CNN
F 3 "" H 3850 3850 50  0000 C CNN
	1    3850 3850
	-1   0    0    1   
$EndComp
$Comp
L LED D1
U 1 1 56CBC09B
P 3850 4050
F 0 "D1" H 3850 4150 50  0000 C CNN
F 1 "LED" H 3850 3950 50  0000 C CNN
F 2 "LEDs:LED_0603" H 3850 4050 50  0001 C CNN
F 3 "" H 3850 4050 50  0000 C CNN
	1    3850 4050
	-1   0    0    1   
$EndComp
$Comp
L R R1
U 1 1 56CBC526
P 4200 4050
F 0 "R1" V 4280 4050 50  0000 C CNN
F 1 "560R" V 4200 4050 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 4130 4050 50  0001 C CNN
F 3 "" H 4200 4050 50  0000 C CNN
	1    4200 4050
	0    1    1    0   
$EndComp
$Comp
L R R2
U 1 1 56CBC5DB
P 4200 3850
F 0 "R2" V 4280 3850 50  0000 C CNN
F 1 "560R" V 4200 3850 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 4130 3850 50  0001 C CNN
F 3 "" H 4200 3850 50  0000 C CNN
	1    4200 3850
	0    1    1    0   
$EndComp
$Comp
L R R5
U 1 1 56CBD54C
P 4550 4100
F 0 "R5" V 4630 4100 50  0000 C CNN
F 1 "680R" V 4550 4100 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 4480 4100 50  0001 C CNN
F 3 "" H 4550 4100 50  0000 C CNN
	1    4550 4100
	1    0    0    -1  
$EndComp
$Comp
L R R6
U 1 1 56CBDED7
P 4550 4400
F 0 "R6" V 4630 4400 50  0000 C CNN
F 1 "120R" V 4550 4400 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 4480 4400 50  0001 C CNN
F 3 "" H 4550 4400 50  0000 C CNN
	1    4550 4400
	1    0    0    -1  
$EndComp
$Comp
L R R7
U 1 1 56CBDF31
P 4550 4700
F 0 "R7" V 4630 4700 50  0000 C CNN
F 1 "680R" V 4550 4700 50  0000 C CNN
F 2 "Resistors_SMD:R_0603_HandSoldering" V 4480 4700 50  0001 C CNN
F 3 "" H 4550 4700 50  0000 C CNN
	1    4550 4700
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR014
U 1 1 56CBFAFC
P 4300 4850
F 0 "#PWR014" H 4300 4600 50  0001 C CNN
F 1 "GND" H 4300 4700 50  0000 C CNN
F 2 "" H 4300 4850 50  0000 C CNN
F 3 "" H 4300 4850 50  0000 C CNN
	1    4300 4850
	1    0    0    -1  
$EndComp
$Comp
L CONN_01X03 P1
U 1 1 56CC05C3
P 3500 1550
F 0 "P1" H 3500 1750 50  0000 C CNN
F 1 "SWD" V 3600 1550 50  0000 C CNN
F 2 "Pin_Headers:Pin_Header_Straight_1x03" H 3500 1550 50  0001 C CNN
F 3 "" H 3500 1550 50  0000 C CNN
	1    3500 1550
	0    -1   -1   0   
$EndComp
Wire Wire Line
	5450 2850 6550 2850
Wire Wire Line
	3850 3050 3450 3050
Wire Wire Line
	3850 2100 3850 3050
Wire Wire Line
	3450 3150 4150 3150
Wire Wire Line
	3850 3150 3850 3500
Wire Wire Line
	1800 3050 2200 3050
Wire Wire Line
	1800 1850 1800 3050
Wire Wire Line
	7700 2450 7700 2650
Wire Wire Line
	7700 2650 7650 2650
Wire Wire Line
	5750 2750 5450 2750
Wire Wire Line
	5750 2250 5750 2750
Wire Wire Line
	5750 2250 5900 2250
Connection ~ 5750 2600
Wire Wire Line
	6350 2250 6200 2250
Connection ~ 6350 2850
Wire Wire Line
	5450 3050 5750 3050
Wire Wire Line
	5750 3050 5750 3700
Wire Wire Line
	5850 3150 5750 3150
Connection ~ 5750 3150
Wire Wire Line
	5450 2950 6350 2950
Wire Wire Line
	6350 2950 6350 3500
Wire Wire Line
	6150 3150 6350 3150
Connection ~ 6350 3150
Wire Wire Line
	6350 1700 6350 2850
Connection ~ 6350 2600
Wire Wire Line
	5600 3150 5600 3700
Wire Wire Line
	5750 3700 7650 3700
Wire Wire Line
	7650 3700 7650 2950
Connection ~ 5750 3500
Wire Wire Line
	4150 2750 4350 2750
Wire Wire Line
	3450 2950 4150 2950
Wire Wire Line
	4150 2950 4150 2750
Wire Wire Line
	4350 3150 4250 3150
Wire Wire Line
	4250 3150 4250 3250
Wire Wire Line
	4250 3250 3450 3250
Wire Wire Line
	4350 3250 4350 3350
Wire Wire Line
	4350 3350 3450 3350
Wire Wire Line
	3450 2850 4350 2850
Wire Wire Line
	4150 3050 4350 3050
Wire Wire Line
	4150 3150 4150 3050
Connection ~ 3850 3150
Wire Wire Line
	4350 2950 4250 2950
Wire Wire Line
	4250 2950 4250 3050
Connection ~ 4250 3050
Wire Wire Line
	2200 4550 1900 4550
Wire Wire Line
	1900 4550 1900 3350
Wire Wire Line
	1900 3350 2200 3350
Wire Wire Line
	2200 3450 2000 3450
Wire Wire Line
	2000 3450 2000 4250
Wire Wire Line
	2000 4250 2200 4250
Wire Wire Line
	2200 3550 2100 3550
Wire Wire Line
	2100 3550 2100 4350
Wire Wire Line
	2100 4350 2200 4350
Wire Wire Line
	3300 4250 3100 4250
Wire Wire Line
	3300 3900 3300 4250
Wire Wire Line
	4900 4550 6000 4550
Wire Wire Line
	3850 2350 4250 2350
Wire Wire Line
	4000 2350 4000 2400
Connection ~ 4000 2350
Connection ~ 3850 2350
Wire Wire Line
	4250 2700 4250 2750
Connection ~ 4250 2750
Wire Wire Line
	4000 2700 4000 2850
Connection ~ 4000 2850
Wire Wire Line
	4250 2350 4250 2400
Wire Wire Line
	3000 1850 3000 2200
Wire Wire Line
	2200 2200 3400 2200
Wire Wire Line
	2200 2200 2200 2650
Wire Wire Line
	7050 4700 7150 4700
Wire Wire Line
	8050 4800 8700 4800
Wire Wire Line
	8700 4800 8700 4250
Wire Wire Line
	4900 4250 6000 4250
Wire Wire Line
	4800 4450 6000 4450
Wire Wire Line
	4300 4350 3100 4350
Wire Wire Line
	4800 4350 6000 4350
Wire Wire Line
	5350 4850 6000 4850
Wire Wire Line
	5250 4950 6000 4950
Wire Wire Line
	5150 5050 6000 5050
Wire Wire Line
	5050 5150 6000 5150
Wire Wire Line
	7650 2950 8100 2950
Wire Wire Line
	8000 2850 7650 2850
Connection ~ 6350 2250
Wire Wire Line
	7650 2850 7650 2750
Wire Wire Line
	6450 2650 6550 2650
Wire Wire Line
	6450 2750 6550 2750
Connection ~ 6450 2650
Wire Wire Line
	4300 4250 4300 4350
Wire Wire Line
	4300 4250 4800 4250
Wire Wire Line
	4800 4250 4800 4350
Connection ~ 4550 4250
Connection ~ 4550 4550
Connection ~ 4550 4850
Wire Wire Line
	4900 4850 4900 4550
Wire Wire Line
	4900 3950 4900 4250
Wire Wire Line
	5350 4850 5350 4250
Connection ~ 5350 4250
Wire Wire Line
	5250 4950 5250 4350
Connection ~ 5250 4350
Wire Wire Line
	5150 5050 5150 4450
Connection ~ 5150 4450
Wire Wire Line
	5050 5150 5050 4550
Connection ~ 5050 4550
Connection ~ 4550 3950
Wire Wire Line
	3100 4450 4300 4450
Wire Wire Line
	4300 4450 4300 4550
Wire Wire Line
	4300 4550 4800 4550
Wire Wire Line
	4800 4550 4800 4450
Wire Wire Line
	3400 2200 3400 1750
Connection ~ 3000 2200
Wire Wire Line
	3450 2650 3500 2650
Wire Wire Line
	3500 2650 3500 1750
Wire Wire Line
	3450 2750 3600 2750
Wire Wire Line
	3600 2750 3600 1750
Wire Wire Line
	2200 4350 2200 4450
Wire Wire Line
	6550 2950 6550 3300
NoConn ~ 2200 2750
NoConn ~ 2200 2850
NoConn ~ 2200 2950
NoConn ~ 2200 3250
NoConn ~ 3450 3450
Wire Wire Line
	4300 4850 4900 4850
Wire Wire Line
	3300 4800 3300 4550
Wire Wire Line
	3300 4550 3100 4550
Wire Wire Line
	6300 4800 7150 4800
Connection ~ 4900 3950
$Comp
L +5V #PWR015
U 1 1 56CC9BAD
P 6300 4350
F 0 "#PWR015" H 6300 4200 50  0001 C CNN
F 1 "+5V" H 6300 4490 50  0000 C CNN
F 2 "" H 6300 4350 50  0000 C CNN
F 3 "" H 6300 4350 50  0000 C CNN
	1    6300 4350
	1    0    0    -1  
$EndComp
Wire Wire Line
	6300 4350 6300 4800
Wire Wire Line
	4550 3950 4900 3950
$Comp
L C C5
U 1 1 56CCB240
P 6800 4600
F 0 "C5" H 6825 4700 50  0000 L CNN
F 1 "10uF" H 6825 4500 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 6838 4450 50  0001 C CNN
F 3 "" H 6800 4600 50  0000 C CNN
	1    6800 4600
	-1   0    0    1   
$EndComp
$Comp
L C C7
U 1 1 56CCB60A
P 8400 4550
F 0 "C7" H 8425 4650 50  0000 L CNN
F 1 "10uF" H 8425 4450 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 8438 4400 50  0001 C CNN
F 3 "" H 8400 4550 50  0000 C CNN
	1    8400 4550
	-1   0    0    1   
$EndComp
$Comp
L LDO U5
U 1 1 56CB96D0
P 7350 4550
F 0 "U5" H 7600 4150 60  0000 C CNN
F 1 "LDO" H 7600 4500 60  0000 C CNN
F 2 "TO_SOT_Packages_SMD:SOT-223" H 7350 4550 60  0001 C CNN
F 3 "" H 7350 4550 60  0000 C CNN
	1    7350 4550
	1    0    0    -1  
$EndComp
Wire Wire Line
	6800 4200 8400 4200
Wire Wire Line
	7050 4200 7050 4700
Wire Wire Line
	8400 4200 8400 4400
Wire Wire Line
	8400 4700 8400 4800
Connection ~ 8400 4800
Connection ~ 7600 4200
Wire Wire Line
	6800 4200 6800 4450
Connection ~ 7050 4200
Wire Wire Line
	6800 4750 6800 4800
Connection ~ 6800 4800
$Comp
L C C4
U 1 1 56CCC1F4
P 5400 3500
F 0 "C4" H 5425 3600 50  0000 L CNN
F 1 "0.1uF" H 5425 3400 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 5438 3350 50  0001 C CNN
F 3 "" H 5400 3500 50  0000 C CNN
	1    5400 3500
	0    1    1    0   
$EndComp
Wire Wire Line
	5600 3500 5550 3500
Connection ~ 5600 3500
Wire Wire Line
	5450 3250 5450 3350
Wire Wire Line
	5450 3350 5150 3350
Wire Wire Line
	5150 3350 5150 3700
Wire Wire Line
	5250 3500 5150 3500
Connection ~ 5150 3500
$Comp
L C C6
U 1 1 56CCCBDF
P 6950 3250
F 0 "C6" H 6975 3350 50  0000 L CNN
F 1 "0.1uF" H 6975 3150 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 6988 3100 50  0001 C CNN
F 3 "" H 6950 3250 50  0000 C CNN
	1    6950 3250
	0    1    1    0   
$EndComp
$Comp
L +3.3V #PWR016
U 1 1 56CCCCB4
P 7350 3250
F 0 "#PWR016" H 7350 3100 50  0001 C CNN
F 1 "+3.3V" H 7350 3390 50  0000 C CNN
F 2 "" H 7350 3250 50  0000 C CNN
F 3 "" H 7350 3250 50  0000 C CNN
	1    7350 3250
	1    0    0    -1  
$EndComp
Wire Wire Line
	7350 3250 7100 3250
Wire Wire Line
	6800 3250 6550 3250
$Comp
L C C3
U 1 1 56CCD815
P 2000 2400
F 0 "C3" H 2025 2500 50  0000 L CNN
F 1 "0.1uF" H 2025 2300 50  0000 L CNN
F 2 "Capacitors_SMD:C_0603_HandSoldering" H 2038 2250 50  0001 C CNN
F 3 "" H 2000 2400 50  0000 C CNN
	1    2000 2400
	0    -1   -1   0   
$EndComp
Wire Wire Line
	1850 2400 1800 2400
Connection ~ 1800 2400
Wire Wire Line
	2150 2400 2200 2400
Connection ~ 2200 2400
Connection ~ 6550 3250
Wire Wire Line
	5600 3150 5450 3150
Wire Wire Line
	8050 4700 8100 4700
Wire Wire Line
	8100 4700 8100 4200
Connection ~ 8100 4200
Wire Wire Line
	3850 3500 4850 3500
Connection ~ 3850 3500
Wire Wire Line
	8300 1700 6350 1700
Wire Wire Line
	6450 1800 6450 2750
Wire Wire Line
	6450 1800 8300 1800
Wire Wire Line
	3450 3550 3600 3550
Wire Wire Line
	4350 3500 4350 4050
Connection ~ 4350 3500
Wire Wire Line
	3600 3550 3600 3850
Wire Wire Line
	3600 3850 3650 3850
Connection ~ 4350 3850
Wire Wire Line
	3650 4050 3300 4050
Wire Wire Line
	3300 4050 3300 4100
Connection ~ 3300 4100
NoConn ~ 2200 3150
Wire Wire Line
	8000 2850 8000 2350
Wire Wire Line
	8000 2350 8300 2350
Wire Wire Line
	8100 2950 8100 2250
Wire Wire Line
	8100 2250 8300 2250
$EndSCHEMATC
