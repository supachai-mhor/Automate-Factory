function OnReceiveLoadMessageToUser(data, email) {

    //console.log(data);
    var messageData = JSON.parse(data);

    if (messageData != null) {

        var mcImg = document.getElementById("imgProfileChatMessage").getAttribute('src');


        for (i = 0; i < messageData.length; i++) {

            // search result
            var options = { weekday: 'short', year: 'numeric', month: 'numeric', day: 'numeric' };
            var optionsTimes = { hour: 'numeric', minute: 'numeric', hour12: true };

            var getDate = new Date(messageData[i].messageDate);
            var dataDate = getDate.toLocaleString('en-US', options);

            var todayDate = new Date();
            var nowTodayDate = todayDate.toLocaleString('en-US', options);
            if (nowTodayDate == dataDate) {
                dataDate = "Today";
            }

            CheckLastTimeFotmatMessage(dataDate);

            var allMessageList = document.getElementById("divMessageList");

            if (messageData[i].senderId == email) {
                // send by me

                var text = messageData[i].message;
                if (text != "") {

                    text = text.replace(/\n/g, "<br>");
                    var splitText = text.split('<br>');
                    if (splitText.length > 0) {
                        for (t = 0; t < splitText.length; t++) {

                            if (splitText[t].length > 30) {
                                checkTextLength = true;
                                break;
                            }
                        }
                    }
                    const div = document.createElement('div');

                    var nowTime = getDate.toLocaleString('en-US', optionsTimes);

                    div.className = 'd-flex justify-content-end mb-4';

                    if (messageData[i].receiverMessageStatus == 1) {
                        nowTime += " | Read";
                    }

                    if (text.length > 30 && checkTextLength == true) {
                        div.innerHTML = '<div class="msg_cotainer_send" style="width: 50%;" >' + text + '<span class="msg_time_send">' + nowTime + '</span></div>' +
                            '<div class="img_cont_msg"><img src="' + myImg + '" class="rounded-circle user_img_msg"></div>';

                    }
                    else {
                        div.innerHTML = '<div class="msg_cotainer_send" >' + text + '<span class="msg_time_send">' + nowTime + '</span></div>' +
                            '<div class="img_cont_msg"><img src="' + myImg + '" class="rounded-circle user_img_msg"></div>';
                    }

                    allMessageList.appendChild(div);

                }
            }
            else {

                // send by machine

                var text = messageData[i].message;
                var checkTextLength = false;
                if (text != "") {

                    text = text.replace(/\n/g, "<br>");
                    var splitText = text.split('<br>');
                    if (splitText.length > 0) {
                        for (t = 0; t < splitText.length; t++) {

                            if (splitText[t].length > 30) {
                                checkTextLength = true;
                                break;
                            }
                        }
                    }
                    const div = document.createElement('div');
                    var nowTime = getDate.toLocaleString('en-US', optionsTimes);

                    div.className = 'd-flex justify-content-start mb-4';

                    if (text.length > 30 && checkTextLength == true) {

                        div.innerHTML = '<div class="img_cont_msg"><img src="' + mcImg + '" class="rounded-circle user_img_msg"></div>' +
                            '<div class="msg_cotainer" style="width: 50%;">' + text + '<span class="msg_time">' + nowTime + '</span></div>';
                    }
                    else {
                        div.innerHTML = '<div class="img_cont_msg"><img src="' + mcImg + '" class="rounded-circle user_img_msg"></div>' +
                            '<div class="msg_cotainer" >' + text + '<span class="msg_time">' + nowTime + '</span></div>';
                    }

                    allMessageList.appendChild(div);
                }
            }

        }
    }

}




