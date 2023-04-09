// Define motor driver pins
#include <Wire.h>
#define RPWM 11
#define LPWM 10
#define R_EN 9
#define L_EN 8

// Define sonar sensor pins
#define TRIGGER_PIN 6
#define ECHO_PIN 7

double kp = 1.0;
double ki = 0.0;
double kd = 0.0;

double input, output, setpoint;
double error, last_error, integral, derivative;

double sample_time = 50; // in milliseconds
double output_min = -255;
double output_max = 255;

unsigned long last_time;
void receiveEvent(int bytes) {
  setpoint = Wire.read();    // read one character from the I2C
}
void setup() {
  // initialize variables
  Serial.begin(9600);
  Wire.begin(3); 
  
  input = 0;
  output = 0;
  setpoint = 13;
  error = 0;
  last_error = 0;
  integral = 0;
  derivative = 0;
  last_time = millis();
  pinMode(R_EN, OUTPUT);
  pinMode(L_EN, OUTPUT);
  pinMode(RPWM, OUTPUT);
  pinMode(LPWM, OUTPUT);
  pinMode(TRIGGER_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  digitalWrite(R_EN, LOW);
  digitalWrite(L_EN, LOW);
  Wire.onReceive(receiveEvent);
  //Serial.println("Begin!");
}

void loop() {

  
  // read distance from sonar sensor
  long duration, cm;
  digitalWrite(TRIGGER_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIGGER_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIGGER_PIN, LOW);
  duration = pulseIn(ECHO_PIN, HIGH);
  cm = (duration / 2) / 29.1; // Convert to centimeters
  
  // set input to distance reading
  input = cm;
  
  // compute error
  error = setpoint - input;

  
  // compute integral
  integral += error * (millis() - last_time);
  
  // compute derivative
  derivative = (error - last_error) / (millis() - last_time);
  
  // compute output
  output = kp * error + ki * integral + kd * derivative;
  
  // limit output
  output = constrain(output, output_min, output_max);
  
  // set motor speed based on output
  if (output > 0) {
    digitalWrite(R_EN, HIGH);
    digitalWrite(L_EN, HIGH);
    analogWrite(LPWM, 200);
    analogWrite(RPWM, 0);
  } else if (output < 0) {
    digitalWrite(R_EN, HIGH);
    digitalWrite(L_EN, HIGH);
    analogWrite(RPWM, 200);
    analogWrite(LPWM, 0);
  } else {
    digitalWrite(R_EN, LOW);
    digitalWrite(L_EN, LOW);
    Wire.beginTransmission(8);
    Wire.write('D');
    Wire.endTransmission();
    
  }
  
  // update last_error and last_time
  last_error = error;
  last_time = millis();
  
  // print status information to serial monitor
  /*Serial.print("Distance: ");
  Serial.print(cm);
  Serial.print(" cm | Setpoint: ");
  Serial.print(setpoint);
  Serial.print(" cm | Output: ");
  Serial.println(output);
  */

  
  // delay for sample_time
  delay(sample_time);
}
