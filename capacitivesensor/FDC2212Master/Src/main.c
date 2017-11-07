/**
  ******************************************************************************
  * File Name          : main.c
  * Description        : Main program body
  ******************************************************************************
  *
  * COPYRIGHT(c) 2016 STMicroelectronics
  *
  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  *   1. Redistributions of source code must retain the above copyright notice,
  *      this list of conditions and the following disclaimer.
  *   2. Redistributions in binary form must reproduce the above copyright notice,
  *      this list of conditions and the following disclaimer in the documentation
  *      and/or other materials provided with the distribution.
  *   3. Neither the name of STMicroelectronics nor the names of its contributors
  *      may be used to endorse or promote products derived from this software
  *      without specific prior written permission.
  *
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
  * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
  * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
  * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
  * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
  * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
  * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  *
  ******************************************************************************
  */
/* Includes ------------------------------------------------------------------*/
#include "stm32f0xx_hal.h"
#include "usb_device.h"

/* USER CODE BEGIN Includes */
#include "stm32f0xx_hal_flash.h"
#include "stm32f0xx_hal_flash_ex.h"

__attribute__((section(".config"))) volatile uint16_t flashconfig[32];
volatile uint16_t config[32];


#define CONFIG_KEY_VALID           0
#define CONFIG_KEY_SCANMODE        1
#define CONFIG_KEY_MAX_SLAVE       2
#define CONFIG_KEY_CYCLE_TIME      3
#define CONFIG_KEY_MEASURE_TIME    4
#define CONFIG_KEY_MEASURE_CURRENT 5
#define CONFIG_KEY_LAST            6

#define CONFIG_VALID     config[CONFIG_KEY_VALID]
#define CYCLE_TIME       config[CONFIG_KEY_CYCLE_TIME]
#define MAX_SLAVE        config[CONFIG_KEY_MAX_SLAVE]

#define MEASURE_CURRENT   config[CONFIG_KEY_MEASURE_CURRENT]
#define MEASURE_TIME      config[CONFIG_KEY_MEASURE_TIME]
#define MEASURE_DURATION  (MEASURE_TIME * 2500)
#define SCANMODE          config[CONFIG_KEY_SCANMODE]
//#define MEASURE_DURATION  0x8329

#define USE_USB
#define CONFIGURE_SENSOR

#define SCANMODE_CONTINUOUS  1
#define SCANMODE_SYNCSTOP    2
#define SCANMODE_EARLYSTOP   3

#include "usbd_cdc_if.h"
/* USER CODE END Includes */

/* Private variables ---------------------------------------------------------*/
TIM_HandleTypeDef htim3;

UART_HandleTypeDef huart2;

/* USER CODE BEGIN PV */
/* Private variables ---------------------------------------------------------*/

#define UART_BUFFER_SIZE 8
volatile uint8_t* uart_read_buf, *uart_write_buf;
volatile int uart_read_pos, uart_read_end, uart_write_pos, uart_write_end;
volatile int tick;
extern USBD_HandleTypeDef hUsbDeviceFS;
char usb_buf[64];
uint8_t usb_pos;
char response[255];
/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_USART2_UART_Init(void);
static void MX_TIM3_Init(void);

/* USER CODE BEGIN PFP */
/* Private function prototypes -----------------------------------------------*/

/* USER CODE END PFP */

/* USER CODE BEGIN 0 */

#define MODE_STOP        1
#define MODE_CONFIGURE   2
#define MODE_RUN         3
int mode = MODE_CONFIGURE;

void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
  if (htim->Instance==TIM3) //check if the interrupt comes from TIM3
  {
    tick++;
  }
}

void send_usb(char* data) {
#ifdef USE_USB
  if(*data==0) return;
  while(CDC_Transmit_FS((uint8_t*)data, strlen(data)) == USBD_BUSY);
  *data=0;
#endif
}

void try_send_usb(char* data) {
#ifdef USE_USB
  if(*data==0) return;
  if(CDC_Transmit_FS((uint8_t*)data, strlen(data)) != USBD_BUSY) {
    *data=0;
  }
#endif
}

void printconfig() {
  strcat(response, "** Current Configuration:\r\n");
  char line[32];
  if(SCANMODE == SCANMODE_CONTINUOUS) {
    sprintf(line, "scan_mode       = continuous\r\n"); strcat(response, line);
  } else if(SCANMODE == SCANMODE_EARLYSTOP) {
    sprintf(line, "scan_mode       = earlystop\r\n"); strcat(response, line);
  } else if(SCANMODE == SCANMODE_SYNCSTOP) {
    sprintf(line, "scan_mode       = syncstop\r\n"); strcat(response, line);
  }
  {
    sprintf(line, "max_slave       = %i\r\n", MAX_SLAVE); strcat(response, line);
    sprintf(line, "cycle_time      = %i\r\n", CYCLE_TIME); strcat(response, line);
    sprintf(line, "measure_time    = %i\r\n", MEASURE_TIME); strcat(response, line);
    sprintf(line, "measure_current = %i\r\n", MEASURE_CURRENT); strcat(response, line);
  }
}

void loadconfig() {
  if(flashconfig[CONFIG_KEY_VALID] == 0) {
    strcat(response, "** Loading default configuration.\r\n");
    config[CONFIG_KEY_VALID] = 1;
    MAX_SLAVE = 32;
    SCANMODE = SCANMODE_SYNCSTOP;
    CYCLE_TIME = 20;
    MEASURE_TIME = 13;
    MEASURE_CURRENT = 0x1E;
  } else {
    strcat(response, "** Loading configuration from flash.\r\n");
    for(int i=0; i<CONFIG_KEY_LAST; i++) {
      config[i] = flashconfig[i];
    }
  }
}

void storeconfig() {
  strcat(response, "** Saving configuration to flash.\r\n");
  uint32_t ret;
  FLASH_EraseInitTypeDef stru;
  stru.NbPages = 1;
  stru.PageAddress = (uint32_t) &flashconfig[0];
  stru.TypeErase = TYPEERASE_PAGEERASE;
  HAL_FLASH_Unlock();
  HAL_FLASHEx_Erase(&stru, &ret);
  HAL_FLASH_Lock();

  HAL_FLASH_Unlock();
  for(int i=0; i<CONFIG_KEY_LAST; i++) {
    ret = HAL_FLASH_Program(TYPEPROGRAM_HALFWORD, (uint32_t) &flashconfig[i], config[i]);
  }
  HAL_FLASH_Lock();
}

void usb_received(uint8_t byte) {
#ifdef USE_USB
 if(byte == '\r') {
  sprintf(response, "\r\n");
  usb_buf[usb_pos] = 0;
  int number;
  if(strcmp("stop", usb_buf)== 0 || (mode == MODE_RUN && strcmp("", usb_buf)==0)) {
    strcat(response, "** Configuration Mode\r\n");
    mode = MODE_STOP;
  } else if(strcmp("go", usb_buf)==0 || (mode == MODE_STOP && strcmp("", usb_buf)==0)) {
    strcat(response, "** Continuing Measurement\r\n");
    mode = MODE_CONFIGURE;
  } else if(strcmp("set scan_mode continuous", usb_buf)==0) {
    strcat(response, "** Switched To Continuous Measurement Mode\r\n");
    SCANMODE = SCANMODE_CONTINUOUS;
    printconfig();
  } else if(strcmp("set scan_mode earlystop", usb_buf)==0) {
    strcat(response, "** Switched To Early Stop Measurement Mode\r\n");
    SCANMODE = SCANMODE_EARLYSTOP;
    printconfig();
  } else if(strcmp("set scan_mode syncstop", usb_buf)==0) {
    strcat(response, "** Switched To Synchronized Stop Measurement Mode\r\n");
    SCANMODE = SCANMODE_SYNCSTOP;
    printconfig();
  } else if(sscanf(usb_buf, "set max_slave %i", &number) == 1) {
    if(number < 0 || number > 255) {
      strcat(response, "** max_slave has to be between 0 and 255\r\n");
    } else {
      MAX_SLAVE = number;
      printconfig();
    }
  } else if(sscanf(usb_buf, "set cycle_time %i", &number) == 1) {
    if(number <= MEASURE_TIME) {
      strcat(response, "** cycle_time has to be greater than measure_time\r\n");
    } else {
      CYCLE_TIME = number;
      printconfig();
    }
  } else if(sscanf(usb_buf, "set measure_time %i", &number) == 1) {
    if(number >= CYCLE_TIME) {
      strcat(response, "** measure_time has to be less than cycle_time\r\n");
    } else if(number < 1 || number > 13) {
      strcat(response, "** measure_time has to be between 1 and 13\r\n");
    } else {
      MEASURE_TIME = number;
      printconfig();
    }
  } else if(sscanf(usb_buf, "set measure_current %i", &number) == 1) {
    if(number < 0 || number > 0x1F) {
      strcat(response, "** measure_current has to be between 1 and 31\r\n");
    } else {
      MEASURE_CURRENT = number;
      printconfig();
    }
  } else if(strcmp("set", usb_buf)==0) {
    printconfig();
  } else if(strcmp("reset", usb_buf)==0) {
    config[CONFIG_KEY_VALID] = 0;
    storeconfig();
    loadconfig();
    printconfig();
  } else if(strcmp("save", usb_buf)==0) {
    storeconfig();
    loadconfig();
    printconfig();
  } else if(strcmp("load", usb_buf)==0) {
    loadconfig();
    printconfig();
  } else if(strcmp("set scan_mode", usb_buf)==0) {
    strcat(response, "** Available commands:\r\n");
    strcat(response, "set scan_mode continuous\r\n");
    strcat(response, "set scan_mode earlystop\r\n");
    strcat(response, "set scan_mode syncstop\r\n");
  } else if(strcmp("help", usb_buf)==0) {
    strcat(response, "** Available commands:\r\n");
    strcat(response, "set                show configuration\r\n");
    strcat(response, "set [key] [value]  change configuration [key] to [value]\r\n");
    strcat(response, "load               load configuration from flash\r\n");
    strcat(response, "save               store configuration to flash\r\n");
    strcat(response, "reset              reset configuration to default\r\n");
    strcat(response, "** Press return to continue measurement\r\n");
  } else {
    strcat(response, "** Invalid command ");
    strcat(response, usb_buf);
    strcat(response, "\r\n");
  }
  usb_pos = 0;
 } else if(byte == '\n') {
   // ignore
 } else if(byte == 0x7F) {
   // backspace
   char str[2] = {byte, 0}; strcat(response, str);
   if(usb_pos > 0) usb_pos--;
 } else if(usb_pos < 63 && mode == MODE_STOP) {
   char str[2] = {byte, 0}; strcat(response, str);
   usb_buf[usb_pos++] = byte;
 }
#endif
}

int uart_can_read_len(int len) {
 return ((uart_read_end - uart_read_pos + UART_BUFFER_SIZE) % UART_BUFFER_SIZE) >= len;
}
void uart_flush() {
 uart_read_pos = uart_read_end;
}
uint8_t uart_read() {
 uint8_t ret = uart_read_buf[uart_read_pos];
 uart_read_pos = (uart_read_pos + 1) % UART_BUFFER_SIZE;
 return ret;
}
int uart_write(char data) {
 int next = (uart_write_end + 1) % UART_BUFFER_SIZE;
 if(next == uart_write_pos) return 0;
 uart_write_buf[uart_write_end] = data;
 uart_write_end = next;
 __HAL_UART_ENABLE_IT(&huart2, UART_IT_TXE);
 return 1;
}
int uart_write_complete() {
  return uart_write_pos == uart_write_end;
}

void send_uart(char* data, int len) {
  int i;
  HAL_GPIO_WritePin(USART2_OE_GPIO_Port, USART2_OE_Pin, GPIO_PIN_SET);
  __HAL_UART_SEND_REQ(&huart2, UART_SENDBREAK_REQUEST);
  for(i=0; i<100; i++);
  for(i=0; i<len; i++) uart_write(data[i]);
  while(!uart_write_complete());
}

/* USER CODE END 0 */

int main(void)
{

  /* USER CODE BEGIN 1 */
    uart_read_pos = -1; uart_read_end = -1;
    uart_write_pos = -1; uart_write_end = -1;
    tick = 0;
    int i;
    int slaves = 0;
    loadconfig();

  /* USER CODE END 1 */

  /* MCU Configuration----------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* Configure the system clock */
  SystemClock_Config();

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_USART2_UART_Init();
  MX_USB_DEVICE_Init();
  MX_TIM3_Init();

  /* USER CODE BEGIN 2 */
  HAL_TIM_Base_Start_IT(&htim3);

  HAL_GPIO_WritePin(USART2_OE_GPIO_Port, USART2_OE_Pin, GPIO_PIN_RESET);
  HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);
  __HAL_UART_ENABLE_IT(&huart2, UART_IT_RXNE);
  __HAL_UART_ENABLE_IT(&huart2, UART_IT_ERR);

  uart_read_buf = malloc(UART_BUFFER_SIZE);
  uart_write_buf = malloc(UART_BUFFER_SIZE);
  uart_read_pos = uart_read_end = uart_write_pos = uart_write_end = 0;

  tick = 0; while(tick < 50);

#ifdef USE_USB
  USBD_CDC_ReceivePacket(&hUsbDeviceFS);
#endif


  tick = 0; while(tick < 500);
  tick = 0;

  sprintf(response, "** Capacitive Sensor Starting - Press Return To Configure\r\n");

  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
    while (1) {
  /* USER CODE END WHILE */

  /* USER CODE BEGIN 3 */

        switch(mode) {

        case MODE_CONFIGURE:
            {
                sprintf(response,"** Detecting Sensors\r\n"); send_usb(response);
                slaves = 0;
                for(int slave = 1; slave < MAX_SLAVE; slave++) {
                    // query status
                    HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
                    {char data[] = {slave, 0x10}; send_uart(data, 2);}
                    HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);
                    tick = 0; while(tick < 10);
                    // no response? -> try next slave
                    if(!uart_can_read_len(2)) {
                        uart_flush();
                        continue;
                    }
                    HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
                    sprintf(response,"Found sensor #%02x\r\n", slave); send_usb(response);


                    // report channels at the corresponding broadcast message
                    {tick = 0; char data[] = {slave, 0x12, 0x00, slaves * 4}; send_uart(data, 4); while(tick < 10); }
                    {tick = 0; char data[] = {slave, 0x22, 0x01, slaves * 4}; send_uart(data, 4); while(tick < 10);}

                    #ifdef CONFIGURE_SENSOR
                    {tick = 0; char data[] = {slave, 0x13, MEASURE_CURRENT, MEASURE_DURATION >> 8, MEASURE_DURATION & 0xFF}; send_uart(data, 5); while(tick < 10);}
                    {tick = 0; char data[] = {slave, 0x23, MEASURE_CURRENT, MEASURE_DURATION >> 8, MEASURE_DURATION & 0xFF}; send_uart(data, 5); while(tick < 10);}
                    #endif

                    // global settings
                    if(SCANMODE == SCANMODE_SYNCSTOP) {
                        // scan at start of other broadcast message
                        {tick = 0; char data[] = {slave, 0x11, 0x01, 0x00}; send_uart(data, 4); while(tick < 10);}
                        {tick = 0; char data[] = {slave, 0x21, 0x00, 0x00}; send_uart(data, 4); while(tick < 10);}
                        // stop on 0xFF broadcast
                        {tick = 0; char data[] = {slave, 0x01, 0xFF, 0x00}; send_uart(data, 4); while(tick < 10);}
                    } else if(SCANMODE == SCANMODE_EARLYSTOP) {
                        // scan at start of other broadcast message
                        {tick = 0; char data[] = {slave, 0x11, 0x01, 0x00}; send_uart(data, 4); while(tick < 10);}
                        {tick = 0; char data[] = {slave, 0x21, 0x00, 0x00}; send_uart(data, 4); while(tick < 10);}
                        // disable stop broadcast
                        {tick = 0; char data[] = {slave, 0x01}; send_uart(data, 2); while(tick < 10);}
                    } else if(SCANMODE == SCANMODE_CONTINUOUS) {
                        // enable continuous scan
                        {tick = 0; char data[] = {slave, 0x02}; send_uart(data, 2); while(tick < 10);}
                    }
                    HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);

                    slaves++;
                    uart_flush();
                }

                sprintf(response,"** Configuration completed\r\n"); send_usb(response);

                mode = MODE_RUN;
                break;
            }
        case MODE_STOP:
            {
                if(*response != 0) try_send_usb(response);
                break;
            }
        case MODE_RUN:
            {
                int channelCount = (SCANMODE == SCANMODE_CONTINUOUS) ? 1 : 2;

                sprintf(response,"\r\n\t"); send_usb(response);
                for(int current = 0; current < channelCount; current++) {
                    HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
                    // send corresponding broadcast
                    {char data[] = {0x00, current}; send_uart(data, 2);}
                    HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);

                    int received = 0;
                    while(tick < CYCLE_TIME - 1) {
                        if(!uart_can_read_len(4)) continue;

                        response[0] = uart_read();
                        response[1] = uart_read();
                        response[2] = uart_read();
                        response[3] = uart_read();
                        int32_t value = *((int32_t*)(response));

                        if(mode != MODE_RUN) break;
                        sprintf(response,"%lu\t", value); send_usb(response);
                        received++;
                    }

                    if(SCANMODE == SCANMODE_SYNCSTOP) {
                        char data[] = {0x00, 0xFF}; send_uart(data, 2);
                    }

                    while(tick < CYCLE_TIME);
                    tick -= CYCLE_TIME;

                    if(mode != MODE_RUN) break;
                    for(i=received; i<slaves; i++) {
                        sprintf(response,"-0\t"); send_usb(response);
                    }
                    uart_flush();
                }

                break;
            }
        default:
            {
                sprintf(response,"Invalid mode %i\r\n\t", mode); send_usb(response);
            }
        }
    }

    sprintf(response,"Exiting main loop\r\n"); send_usb(response);
  /* USER CODE END 3 */

}

/** System Clock Configuration
*/
void SystemClock_Config(void)
{

  RCC_OscInitTypeDef RCC_OscInitStruct;
  RCC_ClkInitTypeDef RCC_ClkInitStruct;
  RCC_PeriphCLKInitTypeDef PeriphClkInit;

  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI48;
  RCC_OscInitStruct.HSI48State = RCC_HSI48_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_NONE;
  HAL_RCC_OscConfig(&RCC_OscInitStruct);

  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_HSI48;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_1);

  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USB;
  PeriphClkInit.UsbClockSelection = RCC_USBCLKSOURCE_HSI48;
  HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit);

  HAL_SYSTICK_Config(HAL_RCC_GetHCLKFreq()/1000);

  HAL_SYSTICK_CLKSourceConfig(SYSTICK_CLKSOURCE_HCLK);

  /* SysTick_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(SysTick_IRQn, 0, 0);
}

/* TIM3 init function */
void MX_TIM3_Init(void)
{

  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;

  htim3.Instance = TIM3;
  htim3.Init.Prescaler = 24000;
  htim3.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim3.Init.Period = 1;
  htim3.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  HAL_TIM_Base_Init(&htim3);

  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  HAL_TIM_ConfigClockSource(&htim3, &sClockSourceConfig);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim3, &sMasterConfig);

}

/* USART2 init function */
void MX_USART2_UART_Init(void)
{

  huart2.Instance = USART2;
  huart2.Init.BaudRate = 115200;
  huart2.Init.WordLength = UART_WORDLENGTH_8B;
  huart2.Init.StopBits = UART_STOPBITS_1;
  huart2.Init.Parity = UART_PARITY_NONE;
  huart2.Init.Mode = UART_MODE_TX_RX;
  huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart2.Init.OverSampling = UART_OVERSAMPLING_16;
  huart2.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart2.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_TXINVERT_INIT|UART_ADVFEATURE_RXINVERT_INIT;
  huart2.AdvancedInit.TxPinLevelInvert = UART_ADVFEATURE_TXINV_ENABLE;
  huart2.AdvancedInit.RxPinLevelInvert = UART_ADVFEATURE_RXINV_ENABLE;
  HAL_UART_Init(&huart2);

}

/** Configure pins as
        * Analog
        * Input
        * Output
        * EVENT_OUT
        * EXTI
*/
void MX_GPIO_Init(void)
{

  GPIO_InitTypeDef GPIO_InitStruct;

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOA_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOA, USART2_OE_Pin|LED_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pins : USART2_OE_Pin LED_Pin */
  GPIO_InitStruct.Pin = USART2_OE_Pin|LED_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

}

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

#ifdef USE_FULL_ASSERT

/**
   * @brief Reports the name of the source file and the source line number
   * where the assert_param error has occurred.
   * @param file: pointer to the source file name
   * @param line: assert_param error line source number
   * @retval None
   */
void assert_failed(uint8_t* file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
    ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */

}

#endif

/**
  * @}
  */

/**
  * @}
*/

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
