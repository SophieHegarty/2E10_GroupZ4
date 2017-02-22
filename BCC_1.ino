#define Control_Pin 3
#define enableXbee 4
#define ledPin  13

//#define stopBothMotors 2
//#define normalLineFollow 4
#define Buggy_Stop 2  //A
#define Buggy_FollowLine 4 //B
#define Buggy_TurnRight 6 //C
#define Buggy_TurnLeft 8 //D
#define Buggy_RotateLeft 10 //E
#define Buggy_ReducePower 12  //F
#define Buggy_IncreasePower 14 //G
#define Buggy_HalfPower 16 //H
#define Buggy_FullPower 18 //j
//etc

//States:
String message;
bool messageComplete;

void setup(){
  
  Serial.begin(9600);
  pinMode(ledPin, OUTPUT);
  pinMode(Control_Pin, OUTPUT);    // Define output pins
  pinMode(enableXbee, OUTPUT);
  digitalWrite(Control_Pin,LOW);
   
  /**************  End of calibration process **********/

  
  Serial.begin(9600);  // Initiate Serial communication
  Serial.print("+++");  // Enter xbee AT command mode, NB no carriage return here
  delay(1500);          // Guard time
  Serial.println("ATID 3333, CH C, CN");

  while(Serial.read() != -1) {};
  
  message = "";
  messageComplete = false;
}

void loop() {  
  if (messageComplete) {
    message.toLowerCase();
    
    if (message == "A") {
      buggycontrol(Buggy_FollowLine);
      send("Buggy Running");
    }else if (message == "B") {
      buggycontrol(Buggy_Stop);
      send("Buggy Stopping");
    }else if (message == "C") {
     buggycontrol(Buggy_TurnRight);
      send("Buggy Turning Right");
    }else if (message == "D") {
     buggycontrol(Buggy_TurnLeft);
      send("E");
    }else if (message == "rotate left") {
     buggycontrol(Buggy_RotateLeft);
      send("Buggy Rotating Left");
    }else if (message == "F") {
     buggycontrol(Buggy_ReducePower);
      send("Buggy Reducing Power");
    }else if (message == "G") {
     buggycontrol(Buggy_IncreasePower);
      send("Buggy Increasing Power");
    }else if (message == "H") {
     buggycontrol(Buggy_HalfPower);
      send("Buggy Half Power");
    }else if (message == "J") {
     buggycontrol(Buggy_FullPower);
      send("Buggy Full Power");
    }else{
      send("Invalid message: " + message);
    }
    
    message = "";
    messageComplete = false;
  }

  
}

void buggycontrol(int mode){
 digitalWrite(Control_Pin, HIGH);
    delay(mode); //Pulse length
    digitalWrite(Control_Pin, LOW);
    delay(20); //Avoid sending too closly packed pulses
}

void send(String message) {
  Serial.println(message);
  Serial.flush();
}
