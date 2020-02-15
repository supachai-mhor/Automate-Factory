


var loginToken = '';
//var data = "";

$.ajax({
	type: "POST",
	url: 'Conference/GetToken',
	data: '{}',
	contentType: "application/json; charset=utf-8",
	dataType: "json",
	success: OnSuccess(data),
	error: OnErrorCall
});

function OnSuccess(data) {

	loginToken = data;
	console.log(data);

//var connection = new signalR.HubConnectionBuilder()
//    .withUrl("/chatHub", { accessTokenFactory: () => this.loginToken })
//    .withAutomaticReconnect()
//    .build();


//connection.on("ReceiveData", function (userMsg, msg) {
//   console.log("user: " + userMsg + " message: " + msg);
//});

//connection.start().then(function () {

//}).catch(function (err) {
//    return console.error(err.toString());
//});

}

function OnErrorCall() {
	console.log("Whoops something went wrong :( ");
}



