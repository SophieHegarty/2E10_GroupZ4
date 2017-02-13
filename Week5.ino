//Constants:
#define InfraRed_Pin 2
#define Control_Pin 3

#define Buggy_Stop 2
#define Buggy_FollowLine 4
#define Buggy_TurnRight 6
#define Buggy_TurnLeft 8
#define Buggy_RotateLeft 10
#define Buggy_ReducePower 12
#define Buggy_IncreasePower 14
#define Buggy_HalfPower 16
#define Buggy_FullPower 18


//Serial port:
#define XBee true

String message;
bool messageComplete;

volatile int gantry;
volatile bool gantryChange;
volatile bool ignoreIR;

void setup() {
  pinMode(Control_Pin, OUTPUT);
  digitalWrite(Control_Pin, LOW);

  pinMode(InfraRed_Pin, INPUT);
  attachInterrupt(digitalPinToInterrupt(InfraRed_Pin), IRDetected, RISING);

  Serial.begin(9600);
  if (XBee) {
    Serial.print("+++");
    delay(1500);
    Serial.println("ATID 3304, CH, C, CN");
    delay(1100);
  }
  
  while(Serial.read() != -1) {};
  
  message = "";
  messageComplete = false;
  gantry = 0;
  gantryChange = false;
  ignoreIR = false;
}

void loop() {
  if (messageComplete) {
    message.toLowerCase();
    
     if (message == "run") {
     buggycontrol(Buggy_FollowLine);
      send("Buggy Running");
    }else   if (message == "stop") {
     buggycontrol(Buggy_Stop);
      send("Buggy Stopping");
    }else if (message == "turn right") {
     buggycontrol(Buggy_TurnRight);
      send("Buggy Turning Right");
    }else if (message == "turn left") {
     buggycontrol(Buggy_TurnLeft);
      send("Buggy Turning Left");
    }else if (message == "rotate left") {
     buggycontrol(Buggy_RotateLeft);
      send("Buggy Rotating Left");
    }else if (message == "reduce power") {
     buggycontrol(Buggy_ReducePower);
      send("Buggy Reducing Power");
    }else if (message == "increase power") {
     buggycontrol(Buggy_IncreasePower);
      send("Buggy Increasing Power");
    }else if (message == "half power") {
     buggycontrol(Buggy_HalfPower);
      send("Buggy Half Power");
    }else if (message == "full power") {
     buggycontrol(Buggy_FullPower);
      send("Buggy Full Power");
    }else if (message == "test") {
      send("Working");
    }else {
      send(message);
      send("NOT A VALID MESSAGE");
    }
    
    message = "";
    messageComplete = false;
  }
  
  if(gantryChange == true){
    gantryChange = false;
    
    send("Detected Gantry " + String(gantry));
    
    ignoreIR = true;
    
    buggycontrol(Buggy_FollowLine);
    delay(1000);
    
    ignoreIR = false;
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
  Serial.println(message);
  Serial.flush();
}


void IRDetected(){
  if(ignoreIR){
    return;
  }
  
  buggycontrol(Buggy_Stop);
  
  while(digitalRead(InfraRed_Pin) != LOW){};
  int t = pulseIn(InfraRed_Pin, HIGH);
  
  gantry = gantryForPulseLength(t);
  gantryChange = true;
}

int gantryForPulseLength(int t){
  if(t >= 500 && t < 1500){
    return 1;
  }else if(t >=1500 && t < 2500){
    return 2;
  }else if(t >=2500 && t < 3500){
    return 3;
  }else{
    return 0;
  }
}


void buggycontrol(int  mode){
   digitalWrite(Control_Pin, HIGH);
   delay(mode);
   digitalWrite(Control_Pin, LOW);
   delay(20);
}




