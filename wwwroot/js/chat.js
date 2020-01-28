
"use strict";
var loginToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5YjlmYjlhYi0wN2RhLTQwOTYtODlkZi1lZWI5NjEyZmZlMWMiLCJzdWIiOiJtaG9yLmZpYm83QGdtYWlsLmNvbSIsImVtYWlsIjoibWhvci5maWJvN0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJNYWNoaW5lIiwiRmFjdG9yeU5hbWUiOiJUYW5qYWkgRGVsaXZlcnkiLCJuYmYiOjE1ODAxOTMwNjksImV4cCI6MTU4ODA1NTQ2OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNzcvIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNzcvIn0.u6uIdJ-aTNtPyNgRrdeQG2qw81pB8_9tFEe0Ihxkd8k";
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => this.loginToken })
    .withAutomaticReconnect()
    .build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveData", function (userMsg, msg) {
    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //var encodedMsg = user + " says " + msg;
    //var li = document.createElement("li");
    //li.textContent = encodedMsg;
    //document.getElementById("messagesList").appendChild(li);
    var myObj = JSON.parse(msg);
    document.getElementById("demo").innerHTML = myObj.Price;
    //console.log("user: " + userMsg + " message: " + msg);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});