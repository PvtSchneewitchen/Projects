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

/* USER CODE BEGIN Includes */
#include "stdio.h"
#include "string.h"

#define SLAVE_ID          0x02

#define MEASURE_CURRENT   0x1E
#define MEASURE_DURATION  0x8329

/* USER CODE END Includes */

/* Private variables ---------------------------------------------------------*/
I2C_HandleTypeDef hi2c1;

UART_HandleTypeDef huart1;

/* USER CODE BEGIN PV */
/* Private variables ---------------------------------------------------------*/
#define FDC_ADDR                0x2a

#define FDC_DATA_CH0            0x00
#define FDC_DATA_LSB_CH0        0x01
#define FDC_DATA_CH1            0x02
#define FDC_DATA_LSB_CH1        0x03

#define FDC_RCOUNT_CH0          0x08
#define FDC_RCOUNT_CH1          0x09
#define FDC_SETTLECOUNT_CH0     0x10
#define FDC_SETTLECOUNT_CH1     0x11
#define FDC_CLOCK_DIVIDER_CH0   0x14
#define FDC_CLOCK_DIVIDER_CH1   0x15
#define FDC_STATUS              0x18
#define FDC_STATUS_CONFIG       0x19
#define FDC_MUX_CONFIG          0x1B
#define FDC_DRIVE_CURRENT_CH0   0x1E
#define FDC_DRIVE_CURRENT_CH1   0x1F
#define FDC_CONFIG              0x1A

#define ACTION_NONE        0
#define ACTION_MEASURE1    1
#define ACTION_MEASURE2    2
#define ACTION_CONFIGURE1  3
#define ACTION_CONFIGURE2  4
#define ACTION_MEASURECONT 5
#define ACTION_STOP        6

#define STATUS_IDLE        0
#define STATUS_MEASURE1    1
#define STATUS_MEASURE2    2
#define STATUS_MEASURECONT 3

volatile int status = STATUS_IDLE, action = ACTION_NONE;
volatile uint32_t cap1 = 0, cap2 = 0, cap = 42;
volatile uint16_t status1 = 0, status2 = 0;

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_I2C1_Init(void);
static void MX_USART1_UART_Init(void);

/* USER CODE BEGIN PFP */
/* Private function prototypes -----------------------------------------------*/

/* USER CODE END PFP */

/* USER CODE BEGIN 0 */

// 00 xx       broadcast key xx
// ss            address slave ss
// ss 01 xx pp     stop measuring on broadcast xx byte pp
// ss 02           start continuous measurement (on channel 1)
// ss c0           request channel c status
// ss c1 xx pp     measure channel c on broadcast xx byte pp
// ss c2 xx pp     provide channel c result on broadcast xx byte pp
// ss c3 ii dd dd  for channel c use current ii and duration dd dd
#define UART_CH1_MASK        0x10
#define UART_CH2_MASK        0x20
#define UART_MEASURE_MASK    0x01
#define UART_REPORT_MASK     0x02
#define UART_CONFIG_MASK     0x03
#define UART_STATUS_MASK     0x00

#define UART_CONTINUOUS_MASK 0x02
#define UART_STOP_MASK       0x01

void write(uint16_t reg, uint16_t data) {
  uint8_t pdata[2] = {(data >> 8) & 0xFF, data & 0xFF};
  HAL_StatusTypeDef ret = HAL_I2C_Mem_Write(&hi2c1, FDC_ADDR * 2, reg, 1, pdata, 2, 2000);
  if(ret != HAL_OK) {
    int i = ret;
    i++;
  }
}

uint16_t read(uint16_t reg) {
  uint8_t pdata[2];
  HAL_StatusTypeDef ret = HAL_I2C_Mem_Read(&hi2c1, FDC_ADDR * 2, reg, 1, pdata, 2, 2000);
  if(ret != HAL_OK) {
    int i = ret;
    i++;
  }
  return (pdata[0] << 8) | pdata[1];
}

#define UNCONFIGURED         255


volatile int measure_ch1_pos = UNCONFIGURED;
volatile int measure_ch1_key = UNCONFIGURED;

volatile int report_ch1_pos = UNCONFIGURED;
volatile int report_ch1_key = UNCONFIGURED;

volatile int current_ch1 = MEASURE_CURRENT;
volatile int duration_ch1 = MEASURE_DURATION;


volatile int measure_ch2_pos = UNCONFIGURED;
volatile int measure_ch2_key = UNCONFIGURED;

volatile int report_ch2_pos = UNCONFIGURED;
volatile int report_ch2_key = UNCONFIGURED;

volatile int current_ch2 = MEASURE_CURRENT;
volatile int duration_ch2 = MEASURE_DURATION;

volatile int stop_pos = UNCONFIGURED;
volatile int stop_key = UNCONFIGURED;


#define UART_EXPECT_ADDR 0
#define UART_EXPECT_KEY 1
#define UART_EXPECT_DATA 2
int uart_state = UART_EXPECT_ADDR;
uint8_t uart_addr = 0, uart_key = 0, uart_pos = 0;
uint8_t tx_buf[8];
uint8_t tx_pos = 0, tx_len = 0;
volatile int transmitting = 0;

void report(UART_HandleTypeDef* uart, void* data, uint8_t len) {
  HAL_GPIO_WritePin(UART_DE_GPIO_Port, UART_DE_Pin, GPIO_PIN_SET);
  transmitting = 1;
  memcpy(tx_buf, data, len);
  tx_len = len;
  tx_pos = 0;
  __HAL_UART_ENABLE_IT(uart, UART_IT_TXE);
}


void handle(UART_HandleTypeDef* uart) {
  int error = 0;
  if(__HAL_UART_GET_IT(uart, UART_IT_FE) != RESET) {
    // Frame Error
    if(transmitting == 0) uart_state = UART_EXPECT_ADDR;
    __HAL_UART_CLEAR_IT(uart, UART_CLEAR_FEF);
    error = 2;
  }

  if(__HAL_UART_GET_IT(uart, UART_IT_PE) != RESET) {
    // Parity Error
    __HAL_UART_CLEAR_IT(uart, UART_CLEAR_PEF);
    error = 1;
  }

  if(__HAL_UART_GET_IT(uart, UART_IT_NE) != RESET) {
    // Noise Error
    __HAL_UART_CLEAR_IT(uart, UART_CLEAR_NEF);
    error = 1;
  }

  if(__HAL_UART_GET_IT(uart, UART_IT_ORE) != RESET) {
    // Overrun Error
    __HAL_UART_CLEAR_IT(uart, UART_CLEAR_OREF);
    error = 1;
  }

  if(__HAL_UART_GET_IT(uart, UART_IT_TXE) != RESET) {
    // Transmit buffer empty
    if(tx_pos < tx_len) {
      uart->Instance->TDR = tx_buf[tx_pos];
      tx_pos++;
    }
    if(tx_pos == tx_len) {
      __HAL_UART_ENABLE_IT(uart, UART_IT_TC);
      __HAL_UART_DISABLE_IT(uart, UART_IT_TXE);
    }
  }

  if(__HAL_UART_GET_IT(uart, UART_IT_TC) != RESET) {
    // Transfer complete
    if(tx_pos == tx_len) {
      HAL_GPIO_WritePin(UART_DE_GPIO_Port, UART_DE_Pin, GPIO_PIN_RESET);
      __HAL_UART_DISABLE_IT(uart, UART_IT_TC);
      __HAL_UART_CLEAR_IT(uart, UART_CLEAR_TCF);
      transmitting = 0;
    }
  }

  if(error == 2) {
    __HAL_UART_SEND_REQ(uart, UART_RXDATA_FLUSH_REQUEST);
    return;
  }

  if(__HAL_UART_GET_IT(uart, UART_IT_RXNE) != RESET) {
    // data available
    if(uart_state == UART_EXPECT_ADDR) {
      uart_addr = uart->Instance->RDR;
      uart_state = UART_EXPECT_KEY;
    } else if(uart_state == UART_EXPECT_KEY) {
      uart_key = uart->Instance->RDR;
      uart_pos = 0;
      uart_state = UART_EXPECT_DATA;
    } else if(uart_state == UART_EXPECT_DATA) {
      uart_pos++;
    }

    if(uart_state == UART_EXPECT_DATA) {
      if(uart_addr == 0) {
        if(uart_key == stop_key && uart_pos == stop_pos) {
          action = ACTION_STOP;
        }
        if(uart_key == measure_ch1_key && uart_pos == measure_ch1_pos) {
          cap1 = 0;
          action = ACTION_MEASURE1;
        }
        if(uart_key == measure_ch2_key && uart_pos == measure_ch2_pos) {
          cap2 = 0;
          action = ACTION_MEASURE2;
        }
        if(uart_key == report_ch1_key && uart_pos == report_ch1_pos) {
          uint32_t value = (status == STATUS_MEASURECONT) ? cap : cap1;
          int i; for(i=0; i<20; i++);
          report(uart, &value, sizeof(value));
        }
        if(uart_key == report_ch2_key && uart_pos == report_ch2_pos) {
          uint32_t value = (status == STATUS_MEASURECONT) ? cap : cap2;
          int i; for(i=0; i<20; i++);
          report(uart, &value, sizeof(value));
        }
      } else if(uart_addr == SLAVE_ID) {
        if(status==STATUS_MEASURECONT) status = STATUS_IDLE;
        if(uart_key == (UART_CH1_MASK + UART_MEASURE_MASK)) {
          if(uart_pos == 0) {measure_ch1_key = UNCONFIGURED; measure_ch1_pos = UNCONFIGURED; }
          if(uart_pos == 1) measure_ch1_key = uart->Instance->RDR;
          if(uart_pos == 2) measure_ch1_pos = uart->Instance->RDR;
        }
        if(uart_key == (UART_CH2_MASK + UART_MEASURE_MASK)) {
          if(uart_pos == 0) {measure_ch2_key = UNCONFIGURED; measure_ch2_pos = UNCONFIGURED; }
          if(uart_pos == 1) measure_ch2_key = uart->Instance->RDR;
          if(uart_pos == 2) measure_ch2_pos = uart->Instance->RDR;
        }
        if(uart_key == (UART_CH1_MASK + UART_REPORT_MASK)) {
          if(uart_pos == 0) {report_ch1_key = UNCONFIGURED; report_ch1_pos = UNCONFIGURED; }
          if(uart_pos == 1) report_ch1_key = uart->Instance->RDR;
          if(uart_pos == 2) report_ch1_pos = uart->Instance->RDR;
        }
        if(uart_key == (UART_CH2_MASK + UART_REPORT_MASK)) {
          if(uart_pos == 0) {report_ch2_key = UNCONFIGURED; report_ch2_pos = UNCONFIGURED; }
          if(uart_pos == 1) report_ch2_key = uart->Instance->RDR;
          if(uart_pos == 2) report_ch2_pos = uart->Instance->RDR;
        }
        if(uart_key == (UART_CH1_MASK + UART_CONFIG_MASK)) {
          if(uart_pos == 1) current_ch1 = uart->Instance->RDR;
          if(uart_pos == 2) duration_ch1 = uart->Instance->RDR;
          if(uart_pos == 3) {
            duration_ch1 = (duration_ch1 << 8) | uart->Instance->RDR;
            action = ACTION_CONFIGURE1;
          }
        }
        if(uart_key == (UART_CH2_MASK + UART_CONFIG_MASK)) {
          if(uart_pos == 1) current_ch2 = uart->Instance->RDR;
          if(uart_pos == 2) duration_ch2 = uart->Instance->RDR;
          if(uart_pos == 3) {
            duration_ch2 = (duration_ch2 << 8) | uart->Instance->RDR;
            action = ACTION_CONFIGURE2;
          }
        }
        if(uart_key == (UART_CH1_MASK + UART_STATUS_MASK)) {
          uint16_t value = status1;
          int i; for(i=0; i<20; i++);
          report(uart, &value, 2);
        }
        if(uart_key == (UART_CH2_MASK + UART_STATUS_MASK)) {
          uint16_t value = status2;
          int i; for(i=0; i<20; i++);
          report(uart, &value, 2);
        }
        if(uart_key == (UART_CONTINUOUS_MASK)) {
          action = ACTION_MEASURECONT;
        }
        if(uart_key == (UART_STOP_MASK)) {
          if(uart_pos == 0) {stop_key = UNCONFIGURED; stop_pos = UNCONFIGURED; }
          if(uart_pos == 1) stop_key = uart->Instance->RDR;
          if(uart_pos == 2) stop_pos = uart->Instance->RDR;
        }
      }
    }
    __HAL_UART_SEND_REQ(uart, UART_RXDATA_FLUSH_REQUEST);
  }
}


/* USER CODE END 0 */

int main(void)
{

  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration----------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* Configure the system clock */
  SystemClock_Config();

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_I2C1_Init();
  MX_USART1_UART_Init();

  /* USER CODE BEGIN 2 */

  // enable FDC2212
  HAL_GPIO_WritePin(I2C_SD_GPIO_Port, I2C_SD_Pin, GPIO_PIN_RESET);
  int i; for(i=0; i<10000; i++);
  // turn on LED
  HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);

  // configure FDC2212
  write(FDC_RCOUNT_CH0,         MEASURE_DURATION);
  write(FDC_RCOUNT_CH1,         MEASURE_DURATION);
  write(FDC_SETTLECOUNT_CH0,    0x000A);
  write(FDC_SETTLECOUNT_CH1,    0x000A);
  write(FDC_CLOCK_DIVIDER_CH0,  0x2001);
  write(FDC_CLOCK_DIVIDER_CH1,  0x2001);
  write(FDC_DRIVE_CURRENT_CH0,  MEASURE_CURRENT << 11);
  write(FDC_DRIVE_CURRENT_CH1,  MEASURE_CURRENT << 11);
  write(FDC_STATUS_CONFIG,      0x0001);
  write(FDC_MUX_CONFIG,         0x020D);

  if(read(FDC_RCOUNT_CH0) != MEASURE_DURATION) while(1);

  int config_sleep = 0x3401;
  int config_ch1 =   0x1401;
  int config_ch2 =   0x5401;
  write(FDC_CONFIG,             config_sleep);
  action = ACTION_NONE;
  status = STATUS_IDLE;

  cap1 = cap2 = 0;
  status1 = status2 = 0;

  __HAL_UART_ENABLE_IT(&huart1, UART_IT_RXNE);
  __HAL_UART_ENABLE_IT(&huart1, UART_IT_ERR);

  HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);

  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
  /* USER CODE END WHILE */

  /* USER CODE BEGIN 3 */
    if(HAL_GPIO_ReadPin(I2C_INT_GPIO_Port, I2C_INT_Pin) == GPIO_PIN_RESET) {
      if(status == STATUS_MEASURE1) {
        status1 = read(FDC_STATUS);
        cap1 = read(FDC_DATA_CH0) << 16 | read(FDC_DATA_LSB_CH0);
        status = STATUS_IDLE;
        HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);
        if(stop_pos == UNCONFIGURED) write(FDC_CONFIG, config_sleep);
      } else if(status == STATUS_MEASURE2) {
        status2 = read(FDC_STATUS);
        cap2 = read(FDC_DATA_CH1) << 16 | read(FDC_DATA_LSB_CH1);
        status = STATUS_IDLE;
        HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);
        if(stop_pos == UNCONFIGURED) write(FDC_CONFIG, config_sleep);
      } else if(status == STATUS_MEASURECONT) {
        HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
        status1 = read(FDC_STATUS);
        cap = read(FDC_DATA_CH0) << 16 | read(FDC_DATA_LSB_CH0);
        HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_RESET);
      }
    }

    if(status == STATUS_IDLE && action == ACTION_CONFIGURE1) {
      write(FDC_RCOUNT_CH0, duration_ch1);
      write(FDC_DRIVE_CURRENT_CH0, current_ch1 << 11);
      action = ACTION_NONE;
    } else if(status == STATUS_IDLE && action == ACTION_CONFIGURE2) {
      write(FDC_RCOUNT_CH1, duration_ch2);
      write(FDC_DRIVE_CURRENT_CH1, current_ch2 << 11);
      action = ACTION_NONE;
    } else if(status == STATUS_IDLE && action == ACTION_MEASURE1) {
      write(FDC_CONFIG, config_ch1);
      status = STATUS_MEASURE1;
      action = ACTION_NONE;
      HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
    } else if(status == STATUS_IDLE && action == ACTION_MEASURE2) {
      write(FDC_CONFIG, config_ch2);
      status = STATUS_MEASURE2;
      action = ACTION_NONE;
      HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
    } else if(status == STATUS_IDLE && action == ACTION_MEASURECONT) {
      write(FDC_CONFIG, config_ch1);
      status = STATUS_MEASURECONT;
      action = ACTION_NONE;
      HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
    } else if(status == STATUS_IDLE && action == ACTION_STOP) {
      write(FDC_CONFIG, config_sleep);
      action = ACTION_NONE;
    }
  }
  /* USER CODE END 3 */

}

/** System Clock Configuration
*/
void SystemClock_Config(void)
{

  RCC_OscInitTypeDef RCC_OscInitStruct;
  RCC_ClkInitTypeDef RCC_ClkInitStruct;
  RCC_PeriphCLKInitTypeDef PeriphClkInit;

  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.HSICalibrationValue = 16;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSI;
  RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL12;
  RCC_OscInitStruct.PLL.PREDIV = RCC_PREDIV_DIV1;
  HAL_RCC_OscConfig(&RCC_OscInitStruct);

  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_1);

  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USART1|RCC_PERIPHCLK_I2C1;
  PeriphClkInit.Usart1ClockSelection = RCC_USART1CLKSOURCE_PCLK1;
  PeriphClkInit.I2c1ClockSelection = RCC_I2C1CLKSOURCE_HSI;
  HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit);

  HAL_SYSTICK_Config(HAL_RCC_GetHCLKFreq()/1000);

  HAL_SYSTICK_CLKSourceConfig(SYSTICK_CLKSOURCE_HCLK);

  /* SysTick_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(SysTick_IRQn, 0, 0);
}

/* I2C1 init function */
void MX_I2C1_Init(void)
{

  hi2c1.Instance = I2C1;
  hi2c1.Init.Timing = 0x2000090E;
  hi2c1.Init.OwnAddress1 = 0;
  hi2c1.Init.AddressingMode = I2C_ADDRESSINGMODE_7BIT;
  hi2c1.Init.DualAddressMode = I2C_DUALADDRESS_DISABLE;
  hi2c1.Init.OwnAddress2 = 0;
  hi2c1.Init.OwnAddress2Masks = I2C_OA2_NOMASK;
  hi2c1.Init.GeneralCallMode = I2C_GENERALCALL_DISABLE;
  hi2c1.Init.NoStretchMode = I2C_NOSTRETCH_DISABLE;
  HAL_I2C_Init(&hi2c1);

    /**Configure Analogue filter
    */
  HAL_I2CEx_ConfigAnalogFilter(&hi2c1, I2C_ANALOGFILTER_ENABLE);

}

/* USART1 init function */
void MX_USART1_UART_Init(void)
{

  huart1.Instance = USART1;
  huart1.Init.BaudRate = 115200;
  huart1.Init.WordLength = UART_WORDLENGTH_8B;
  huart1.Init.StopBits = UART_STOPBITS_1;
  huart1.Init.Parity = UART_PARITY_NONE;
  huart1.Init.Mode = UART_MODE_TX_RX;
  huart1.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart1.Init.OverSampling = UART_OVERSAMPLING_16;
  huart1.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart1.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_TXINVERT_INIT|UART_ADVFEATURE_RXINVERT_INIT;
  huart1.AdvancedInit.TxPinLevelInvert = UART_ADVFEATURE_TXINV_ENABLE;
  huart1.AdvancedInit.RxPinLevelInvert = UART_ADVFEATURE_RXINV_ENABLE;
  HAL_UART_Init(&huart1);

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
  __HAL_RCC_GPIOB_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOA, UART_DE_Pin|LED_Pin|I2C_SD_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin : UART_DE_Pin */
  GPIO_InitStruct.Pin = UART_DE_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_HIGH;
  HAL_GPIO_Init(UART_DE_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pins : LED_Pin I2C_SD_Pin */
  GPIO_InitStruct.Pin = LED_Pin|I2C_SD_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /*Configure GPIO pin : I2C_INT_Pin */
  GPIO_InitStruct.Pin = I2C_INT_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(I2C_INT_GPIO_Port, &GPIO_InitStruct);

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
