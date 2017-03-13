#define Control_Pin 3
#define InfraRed_Pin 2
#define UltraSonic_Ground_Pin A2
#define UltraSonic_Power_Pin A3
#define UltraSonic_Signal_Pin A4

#define Buggy_Stop 2
#define Buggy_FollowLine 4
#define Buggy_TurnRight 6
#define Buggy_TurnLeft 8
#define Buggy_RotateLeft 10
#define Buggy_ReducePower 12
#define Buggy_IncreasePower 14
#define Buggy_HalfPower 16
#define Buggy_FullPower 18
//Sent to buggycontrol(int) to control the buggy

//Hardware:
#define XBee false
#define Motors false
//XBee controls communication via XBee or cable
//Motors coontrols whether pulses are sent to the control pin
//Tip: Turn this off when not working with the buggy!
//     It interferes with the US obstacle detection.

//Debugging:
#define DEBUG false
#define DEBUGIR false
#define DEBUGUS false
//DEBUG controls verbose output of general messages
//DEBUGIR controls verbose output of Infra Red related messages
//DEBUGUS controls verbose output of Ultra Sonic related messages

//States:
int buggyID;
bool ideal;
String message;
bool messageComplete;
//Used for exchange between serialEvent() and loop()

volatile int gantry;
volatile bool gantryChange;
bool gantryCheck;
volatile bool ignoreIR;
//Used for exchange between IRDetected() and loop()
//Volatile because IRDetected() is an interrupt handler

const unsigned long USCheckInterval = 100; //in milliseconds
unsigned long lastUSCheck;
bool obstacle;

void setup() {
  pinMode(Control_Pin, OUTPUT);
  digitalWrite(Control_Pin, LOW);

  pinMode(InfraRed_Pin, INPUT);
  attachInterrupt(digitalPinToInterrupt(InfraRed_Pin), IRDetected, RISING);

  pinMode(UltraSonic_Power_Pin, OUTPUT);
  digitalWrite(UltraSonic_Power_Pin, HIGH);

  pinMode(UltraSonic_Ground_Pin, OUTPUT);
  digitalWrite(UltraSonic_Ground_Pin, LOW);
  
  Serial.begin(9600);
  if (XBee) {
    Serial.print("+++");
    delay(1500);
    Serial.println("ATID 3304, CH, C, CN");
    delay(1100);
  }
  
  while(Serial.read() != -1) {};

  buggyID = 0;
  message = "";
  messageComplete = false;
  gantry = 0;
  gantryChange = false;
  gantryCheck = false;
  ignoreIR = false;
  ideal = false;
  lastUSCheck = millis();
  obstacle = false;
  
  if (DEBUG) {
    send("Setup done");
  }
}

void loop() {  
  if (messageComplete) {
    message.toLowerCase();
    
    if (message == "Buggy is 1") {
      buggyID = 1;
      send("Buggy ID set to 1");
    }else if (message == "Buggy is 2") {
      buggyID = 2;
      send("Buggy ID set to 2");
    }

    else if ((message.substring(0,3) == "1: " && buggyID==1) || (message.substring(0,3)=="2: " && buggyID == 2)){
    
           if (message.substring(3) == "run") {
            buggycontrol(Buggy_FollowLine);
            ideal = true;
            send("Buggy Running");
          }else if (message.substring(3) == "stop") {
            buggycontrol(Buggy_Stop);
            ideal = false;
            send("Buggy Stopping");
          }else if(message.substring(3) == "leave gantry"){
            ignoreIR = true;
            
            buggycontrol(Buggy_FollowLine);
            delay(1000);
        
            if (DEBUGIR) {
              send("Passed gantry");
            }
            
            ignoreIR = false;
            
          }else if (message.substring(3) == "park right") {
            ignoreIR = true;
            
            buggycontrol(Buggy_TurnRight);
            delay(3000);
            buggycontrol(Buggy_FollowLine);
            delay(3000);
            buggycontrol(Buggy_Stop);
            
            ignoreIR = false;
            
            send("Buggy Parked");
          }else if (message.substring(3) == "park left") {
            ignoreIR = true;
            
            buggycontrol(Buggy_TurnLeft);
            delay(3000);
            buggycontrol(Buggy_FollowLine);
            delay(3000);
            buggycontrol(Buggy_Stop);
            
            ignoreIR = false;
            
            send("Buggy Parked");
          }else if (message.substring(3) == "turn right") {
           buggycontrol(Buggy_TurnRight);
            send("Buggy Turning Right");
          }else if (message.substring(3) == "turn left") {
           buggycontrol(Buggy_TurnLeft);
            send("Buggy Turning Left");
          }else if (message.substring(3) == "rotate left") {
           buggycontrol(Buggy_RotateLeft);
            send("Buggy Rotating Left");
          }else if (message.substring(3) == "reduce power") {
           buggycontrol(Buggy_ReducePower);
            send("Buggy Reducing Power");
          }else if (message.substring(3) == "increase power") {
           buggycontrol(Buggy_IncreasePower);
            send("Buggy Increasing Power");
          }else if (message.substring(3) == "half power") {
           buggycontrol(Buggy_HalfPower);
            send("Buggy Half Power");
          }else if (message.substring(3) == "full power") {
           buggycontrol(Buggy_FullPower);
            send("Buggy Full Power");
          }else if (message.substring(3) == "test") {
            send("Working");
          }else{
            send("Invalid message: " + message);
          }
          
          message = "";
          messageComplete = false;
        }
        int count = 0;
        if(gantryChange == true){
          count = count +1;
          gantryChange = false;
        if (count == 1){
          gantryCheck =true; 
        }
         else {
          gantryCheck = false; 
         }
          buggycontrol(Buggy_Stop);
           if(gantryCheck){
          send("Detected Gantry " + String(gantry));
           }
         }
        
        
        unsigned long t = millis();
        
        if (t - lastUSCheck > USCheckInterval) {
          lastUSCheck = t;
          
          if (hasObstacle()) {
            if (!obstacle) {
              obstacle = true;
              
              send("Obstacle detected");
              
              buggycontrol(Buggy_Stop);
            }
           
          }else{
            if (obstacle) {
              obstacle = false;
              
              send("Obstacle gone");
              if (ideal){
                buggycontrol(Buggy_FollowLine);
              }
              else{
                buggycontrol(Buggy_Stop);
              }
            }
          }
        }
      }
}
void serialEvent(){
  while (Serial.available()) {
    if (messageComplete) continue;
    
    char c = Serial.read();
    if (c != '\n')  {
      message += c;
    }else{
      messageComplete = true;
    }
  }
}

void send(String message) {
  Serial.print(message + "  ");
  Serial.println(buggyID);
  Serial.flush();
}

bool hasObstacle() {
  //Send pulse:
  pinMode(UltraSonic_Signal_Pin, OUTPUT);
  digitalWrite(UltraSonic_Signal_Pin, HIGH);

  delayMicroseconds(10);

  digitalWrite(UltraSonic_Signal_Pin, LOW);

  delayMicroseconds(3);
  
  //Receive pulse:
  pinMode(UltraSonic_Signal_Pin, INPUT);
  
  int dt = pulseIn(UltraSonic_Signal_Pin, HIGH, 2000);
  
  if (DEBUGUS) {
    send("US Pulse length: " + String(dt));
  }
  
  //Process pulse length:
  if (dt == 0) {
    return false;
  }
  
  int s = distanceForPulseLength(dt);
  
  return s <= 200;
}

//Interrupt handler for IR sensors
void IRDetected(){

  if(ignoreIR || gantryChange){
    if (DEBUGIR) {
      send("IR ignored");
    }
    
    return;
  }
  
  while(digitalRead(InfraRed_Pin) != LOW){};
  int t = pulseIn(InfraRed_Pin, HIGH);

  if (DEBUGIR) {
    send("IR Pulse length: " + String(t));
  }
  
  gantry = gantryForPulseLength(t);
  gantryChange = true;
}

//Helping methods:

//Returns the Gantry number for an IR pulse
int gantryForPulseLength(int t){
  if(t >= 500 && t < 1500){
    return 1;
  }else if(t >=1500 && t < 2500){
    return 2;
  }else if(t >=2500 && t < 3500){
    return 3;
  }else{
    if (DEBUGIR) {
      send("Invalid pulse length");
    }
    
    return 0;
  }
}

//Returns the distance of an obstacle in mm for an US pulse
int distanceForPulseLength(int t) {
  return 340 * t / 2000;
}

//Sends pulses of different lengths to the buggy control chip
void buggycontrol(int  mode){
  if (DEBUG) {
    send("buggycontrol(" + String(mode) + ")");
  }

  if (Motors) {
    digitalWrite(Control_Pin, HIGH);
    delay(mode); //Pulse length
    digitalWrite(Control_Pin, LOW);
    delay(20); //Avoid sending too }
  }  }
