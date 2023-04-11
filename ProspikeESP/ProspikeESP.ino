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
    ds3502.setWiper(value): sets new wiper value
    ds3502.getWiper(): returns current wiper value
    ds3502.setWiperDefault(value):  sets the value of wiper when device turns on

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
//I2C
#include <Wire.h>

#include <Adafruit_DS3502.h>
#include <NewPing.h>
#include <Servo.h>
#include <SoftwareSerial.h>
#include <AccelStepper.h>

Servo reloadServo;
Adafruit_DS3502 ds3502 = Adafruit_DS3502();

// Linear actuator
#define RPWM 25 // Forward Level PWM
#define FPWM 26 // Reverse Level PWM
#define R_EN 27 // Forward Enable when HIGH
#define L_EN 14 // Reverse Enable when HIGH

//Arduino UART
#define RXp2 16
#define TXp2 17

//Arduino - Arduino UART
//SoftwareSerial mySerial(6, 7);

//Linear Actuator Home
#define homePoint 14



// Stepper motor
const int dirPin1 = 2;
const int stepPin1 = 3; 
const int enablePin = 4;
int dist = 630;
unsigned long timeout = 3000;
unsigned long startTime;
AccelStepper stepper(AccelStepper::DRIVER, stepPin1, dirPin1);

int motorSpeed = 0;
int linAct = homePoint;
int command;
char error;
int oldSpeed = 0;

void setup() {
  Serial.begin(9600);
  //mySerial.begin(9600);
  pinMode(enablePin, OUTPUT);
  stepper.setMaxSpeed(300);
  stepper.setAcceleration(200);
  stepper.setSpeed(40);
  Wire.begin(8); 
  reloadServo.attach(9);
  digitalWrite(enablePin, HIGH);
  Wire.onReceive(receiveEvent);
 /* pinMode(RPWM, OUTPUT);
  pinMode(FPWM, OUTPUT);
  pinMode(R_EN, OUTPUT);
  pinMode(L_EN, OUTPUT);
  pinMode(ECHO, INPUT);
  pinMode(TRIG, OUTPUT); */

  reloadServo.write(10);
  
  //attachInterrupt(digitalPinToInterrupt(2), interruptRoutine, FALLING);
  //Serial.println("WAAAA");
  
  // Wait until serial port is opened
  //while (!Serial) { delay(1); }
  Serial.println("Start");
  if (!ds3502.begin()) {
    Serial.println("Couldn't find DS3502 chip");
    while (1);
  }
  Serial.println("Found DS3502 chip");

  
  
    sendLinAct(homePoint);

    //ds3502.setWiperDefault(0);

}

/*void interruptRoutine(){
  String message = mySerial.readStringUntil('\n');
  
}*/
void reload(){ // Sequence that lowers the stepper enough for a ball to pass
  reloadServo.write(90);
  delay(650);
  reloadServo.write(10);
  Serial.println("Reload");
  
}

void motorSpeedFunc(int newSpeed, int oldSpeed){
    int tempSpeed = oldSpeed;
    int step1 = 1;
    int diff = abs(newSpeed - oldSpeed);
    if ((newSpeed - oldSpeed) < 0){
      step1 = -1;
    }
    if (newSpeed != 0 && newSpeed - oldSpeed > 0){
      diff = diff + 8;
    }
    for (int i = 0; i <= diff; i++){

      tempSpeed = tempSpeed +  step1;
      if (tempSpeed < 0){
        tempSpeed = 0;
      }
      ds3502.setWiper(tempSpeed);
      delay(75);
    }
    if (newSpeed != 0 && newSpeed - oldSpeed > 0){
      for (int d = 8; d > 0; d--){
        tempSpeed = tempSpeed - 1;
        ds3502.setWiper(tempSpeed);
        delay(50);
      }
    }

  
}
void resetFeed(){ // Send the feed down until limit swicth is pressed
Serial.println("Feed");
}


void stepFunction(){
  digitalWrite(enablePin, LOW);
  delay(500);

  Serial.println("Moving up");
  stepper.move(dist);
  stepper.runToPosition();
  delay(1000);

  Serial.println("Moving down");
  stepper.move(-dist);
  stepper.runToPosition();
  delay(1000);

  digitalWrite(enablePin, HIGH);
}
void launch(){ // Insert stepper code here
  stepFunction();

}
void sendLinAct(int linAct){
  Serial.println("Transmiting");
        Wire.beginTransmission(13);
        Wire.write(linAct);
        Wire.endTransmission();
        Serial.println("Done Transmitting");
}
void receiveEvent(int bytes) {
  error = Wire.read();    // read one character from the I2C
  Serial.print(error);
}

void loop() {
  Serial.println("Looping");
  //error = '';
  while (!Serial.available()) { // Wait for Serial input
    delay(100);
    Serial.println("Waiting");
  }
   
   
//Wait for incoming data
  //if (Serial.available() > 0) {
    // Read the incoming data up to the first comma
    String data = Serial.readStringUntil(',');
    Serial.println(data);
    // Convert the first value to an integer
    motorSpeed = data.toInt();
  
    // Read the second value up to the newline character
    data = Serial.readStringUntil('.');
    Serial.println(data);
    // Convert the second value to an integer
    linAct = data.toInt();
    

    data = Serial.readStringUntil('\n');
    Serial.println(data);
    int command = data.toInt();
    switch (command) {
      case 1: // 
      
          reload();
          //oldSpeed = motorSpeed;
      break;
      case 2: 

          resetFeed();
      //oldSpeed = motorSpeed;

      break;
      case 3:
        launch();
        //oldSpeed = motorSpeed;
  
      break;
      case 4: // Shutoff sequence
        sendLinAct(homePoint);
        motorSpeedFunc(0, oldSpeed);
        oldSpeed = motorSpeed;

      break;
      case 5: // Just adjusting the sliders
        sendLinAct(linAct);
        motorSpeedFunc(motorSpeed, oldSpeed);
        //ds3502.setWiper(motorSpeed);
        
        oldSpeed = motorSpeed;
      
      break;
      case 6: // Demo Mode
        if (motorSpeed == 50 && linAct == 16){
          reload();
        }
        sendLinAct(linAct);
        //ds3502.setWiper(motorSpeed);
        motorSpeedFunc(motorSpeed,oldSpeed); 
        startTime = millis();
        
        while (error != 'D' || millis() - startTime > timeout){
          //delay(100);
        }
        launch();
        sendLinAct(homePoint);
        startTime = millis();
        
        while (error != 'D' || millis() - startTime > timeout){
          //delay(100);
        }
        //ds3502.setWiper(0);
        oldSpeed = motorSpeed;
      break;
      case 0: // Code that runs when no commands present, as is the case when it is the automatic sequence
        reload();
        sendLinAct(linAct);
        //ds3502.setWiper(motorSpeed);
        motorSpeedFunc(motorSpeed,oldSpeed); 
        
         startTime = millis();
        while (error != 'D' || millis() - startTime > timeout){
          //delay(100);
        }
        launch();
        sendLinAct(homePoint);
        startTime = millis();
        
        while (error != 'D' || millis() - startTime > timeout){
          //delay(100);
        }
        //ds3502.setWiper(0);
        oldSpeed = motorSpeed;
        //motorSpeed = 0;
        //motorSpeedFunc(motorSpeed, oldSpeed);
        //oldSpeed = motorSpeed;
        Serial.write("Ready");
      break;
    }

    
   
    
  
  //}
   
}
