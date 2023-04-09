/* 
  Prospike Firmware V1 for USB-C ESP32
  March 23, 2023








*/

/* Digital Potentiometer
 *  need Adafruit_DS3502.h library, and to declare Adafruit_DS3502 ds3502 = Adafruit_DS3502();
 Potentiometer value must be between [0, 127]
          with 0 being 0 Ohm and 127 being 10kOhm
          This means the potentiometer step is 78 ohms

    Functions: 
    ds5302.setWiper(value): sets new wiper value
    ds5302.getWiper(): returns current wiper value
    ds5302.setWiperDefault(value):  sets the value of wiper when device turns on

*/
/* Stepper Motor

  With current configuration, only need 
  stepUp(): rotates the stepper for a set amount of steps up rack
  stepDown(): rotates the stepper for a set amount of steps down rack

  check out https://www.schmalzhaus.com/EasyDriver/ for pin layout and possible changes, 
        like changes step to 1/2, 1/4 or 1/8 step, sleep, enable and more.

*/

/* Linear Actuator

  R_EN HIGH means extension
  L_EN HIGH means retraction
      Other pin should be pulled low

  analogWrite to RPWM, FPWM to set the extension and retraction speed.



*/

/* Sonar Sensor
  Old method (commented out): sensorRead() returns the distance measured in cms

  NewPing
  sonar.ping_cm() returns the measured distance in cm
        have a delay(50) (50 millisecond) delay between pings to make sure it works correctly
  

*/
#include <Adafruit_DS3502.h>
#include <NewPing.h>

Adafruit_DS3502 ds3502 = Adafruit_DS3502();

// Linear actuator
#define RPWM 25 // Forward Level PWM
#define FPWM 26 // Reverse Level PWM
#define R_EN 27 // Forward Enable when HIGH
#define L_EN 14 // Reverse Enable when HIGH

// Sonar Sensor
#define ECHO 35 // Sensor input
#define TRIG 32 // Sensor timing output
#define MAX_DISTANCE 200 // Max measuring distance of 200 cm is more than we need for out application 
NewPing sonar(TRIG, ECHO, MAX_DISTANCE); // NewPing setup of pins and max distance
//long duration;
//long distance;

// Stepper motor
#define DIR 16 // Controls the direction of rotation
#define STEP 17 // Each rising edge causes a step to be taken


void setup() {


  pinMode(RPWM, OUTPUT);
  pinMode(FPWM, OUTPUT);
  pinMode(R_EN, OUTPUT);
  pinMode(L_EN, OUTPUT);
  pinMode(ECHO, INPUT);
  pinMode(TRIG, OUTPUT);
  pinMode(DIR, OUTPUT);
  pinMode(STEP, OUTPUT);

  digitalWrite(stp, LOW);
  digitalWrite(dir, LOW);
  
  Serial.begin(115200);
  // Wait until serial port is opened
  while (!Serial) { delay(1); }

  if (!ds3502.begin()) {
    Serial.println("Couldn't find DS3502 chip");
    while (1);
  }
  Serial.println("Found DS3502 chip");


  


  
}

void loop() {




}
// Turns the stepper motor forwards
void stepUp()
{
  
  digitalWrite(dir, LOW); //Pull direction pin low to move "forward"
  for(x= 0; x<5000; x++)  //Loop through for a set amout of steps *WILL NEED TO BE CALIBRATED ON DEVICE*
  {
    digitalWrite(stp,HIGH); //Trigger one step forward
    delay(1);
    digitalWrite(stp,LOW); //Pull step pin low so it can be triggered again
    delay(1);
  }
  
}

// Turns the stepper motor backwards
void stepDown()
{
  
  digitalWrite(dir, HIGH); //Pull direction pin HIGH to move "Backwards"
  for(x= 0; x<5000; x++)  //Loop through for a set amout of steps *WILL NEED TO BE CALIBRATED ON DEVICE*
  {
    digitalWrite(stp,HIGH); //Trigger one step forward
    delay(1);
    digitalWrite(stp,LOW); //Pull step pin low so it can be triggered again
    delay(1);
  }
  
}
/* Manually measuring distance
 * // Read the sensor value
int sensorRead(){

  digitalWite(TRIG, LOW);
  delayMicroseconds(2);

  digitalWite(TRIG, HIGH)
  delayMicroseconds(10);
  digitalWrite(TRIG, LOW);

  duration = pulsIn(ECHO, HIGH);

  return duration * 0.034/2;
} */
