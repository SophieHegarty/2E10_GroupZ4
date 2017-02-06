//Constants:
#define InfraRed_Pin 2
#define Control_Pin 3

//Serial port:
#define XBee false

String message;
bool messageComplete;

void setup() {
  pinMode(Control_Pin, OUTPUT);
  digitalWrite(Control_Pin, LOW);

  pinMode(InfraRed_Pin, INPUT);

  Serial.begin(9600);
  if (XBee) {
    Serial.print("+++");
    delay(1500);
    Serial.println("ATID 3333, CH, C, CN");
    delay(1100);
  }
  
  while(Serial.read() != -1) {};

  message = "";
  messageComplete = false;
}

void loop() {
  if (Serial.available() > 0) {
    char c = Serial.read();

    if (c != '!')  {
      message += c;
    }else{
      messageComplete = true;
    }
  }

  message.toLowerCase();

  if (messageComplete) {
    if (message == "turnon") {
      digitalWrite(Control_Pin, HIGH);
      send("Light is on");
    }else if (message == "turnoff") {
      digitalWrite(Control_Pin, LOW);
      send("Light is off");
    }else if (message =="wait a second") {
      delay(1000);
      send("Done");
    }else{
      send("NOT A VALID MESSAGE");
    }

    message = "";
    messageComplete = false;
  }
}

void send(String message) {
  Serial.print(message + "!");
  Serial.flush();
}

