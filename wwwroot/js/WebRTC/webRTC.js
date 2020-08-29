
"use strict";

let localStream;
let pc1;
let pc2;

//var pcConfig = {
//    'iceServers': [
//        { "urls": ["turn:turn.automate-2015.com:3478"], "username": "kittiya", "credential": "Mhor.fibo7" },
//        { "urls": ["stun:stun.automate-2015.com:3478"], "username": "kittiya", "credential": "Mhor.fibo7" }
//    ],"iceTransportPolicy": "all", "iceCandidatePoolSize": "0"
//};

var pcConfig = { "iceServers": [{ "urls": ["stun:stun.l.google.com:19302"] }], "iceTransportPolicy": "all", "iceCandidatePoolSize": "0" };

// HUB Connection SignalR
var loginToken = document.getElementById("tokenData").value;
console.log(loginToken);
var connectionHub = new signalR.HubConnectionBuilder()
    .withUrl("/AutomateHub", { accessTokenFactory: () => this.loginToken })
    .withAutomaticReconnect()
    .build();

connectionHub.start().then(function () {
}).catch(function (err) {
	return console.error(err.toString());
});

// Client Received Message
connectionHub.on("ReceiveRTCMessage", async function (data) {

    var message = JSON.parse(data);
    if (message.answer) {

        console.log('pc1 setRemoteDescription start');
        try {
            const remoteDesc = new RTCSessionDescription(message.answer);
            await pc1.setRemoteDescription(remoteDesc);
            onSetRemoteSuccess(pc1);
        } catch (e) {
            onSetSessionDescriptionError(e);
        }

    }
    else if (message.offer) {

        console.log('pc2 setRemoteDescription start');
        try {
            const remoteDesc = new RTCSessionDescription(message.offer);
            await pc2.setRemoteDescription(remoteDesc);
            onSetRemoteSuccess(pc2);
        } catch (e) {
            onSetSessionDescriptionError();
        }

        console.log('pc2 createAnswer start');
        // Since the 'remote' side has no media stream we need
        // to pass in the right constraints in order for it to
        // accept the incoming offer of audio and video.
        try {
            const answer = await pc2.createAnswer();
            await onCreateAnswerSuccess(answer);
        } catch (e) {
            onCreateSessionDescriptionError(e);
        }

    }
    else if (message.onIce) {

        const peerConnection = message.onIce.peerConnection;
        const iceCandidate = message.onIce.candidate;

        if (iceCandidate) {
            if (peerConnection == 'pc1') {
                try {
                    const newIceCandidate = new RTCIceCandidate(iceCandidate);
                    await pc2.addIceCandidate(newIceCandidate);
                    onAddIceCandidateSuccess(pc1);
                } catch (e) {
                    onAddIceCandidateError(peerConnection, e);
                }
                console.log(`${peerConnection} ICE candidate:\n${iceCandidate ? iceCandidate.candidate : '(null)'}`);
            }
            else {
                try {
                    const newIceCandidate = new RTCIceCandidate(iceCandidate);
                    await pc1.addIceCandidate(newIceCandidate);
                    onAddIceCandidateSuccess(pc1);
                } catch (e) {
                    onAddIceCandidateError(peerConnection, e);
                }
                console.log(`${peerConnection} ICE candidate:\n${iceCandidate ? iceCandidate.candidate : '(null)'}`);
            }
        }
    }

});

// RTCPeerToPeer Connection
const startButton = document.getElementById('startButton');
const callButton = document.getElementById('callButton');
const hangupButton = document.getElementById('hangupButton');
callButton.disabled = true;
hangupButton.disabled = true;
startButton.addEventListener('click', start);
callButton.addEventListener('click', call);
hangupButton.addEventListener('click', hangup);


const pc1StateDiv = document.querySelector('div#pc1State');
const pc1IceStateDiv = document.querySelector('div#pc1IceState');
const pc1ConnStateDiv = document.querySelector('div#pc1ConnState');
const pc2StateDiv = document.querySelector('div#pc2State');
const pc2IceStateDiv = document.querySelector('div#pc2IceState');
const pc2ConnStateDiv = document.querySelector('div#pc2ConnState');


let startTime;
const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');

localVideo.addEventListener('loadedmetadata', function () {
    console.log(`Local video videoWidth: ${this.videoWidth}px,  videoHeight: ${this.videoHeight}px`);
});

remoteVideo.addEventListener('loadedmetadata', function () {
    console.log(`Remote video videoWidth: ${this.videoWidth}px,  videoHeight: ${this.videoHeight}px`);
});

remoteVideo.addEventListener('resize', () => {
    console.log(`Remote video size changed to ${remoteVideo.videoWidth}x${remoteVideo.videoHeight}`);
    // We'll use the first onsize callback as an indication that video has started
    // playing out.
    if (startTime) {
        const elapsedTime = window.performance.now() - startTime;
        console.log('Setup time: ' + elapsedTime.toFixed(3) + 'ms');
        startTime = null;
    }
});


const offerOptions = {
    offerToReceiveAudio: 1,
    offerToReceiveVideo: 1
};

function getName(pc) {
    return (pc === pc1) ? 'pc1' : 'pc2';
}

function getOtherPc(pc) {
    return (pc === pc1) ? pc2 : pc1;
}

async function start() {
    console.log('Requesting local stream');
    startButton.disabled = true;

    try {
        startGetStream();
        console.log('Received local stream');

        callButton.disabled = false;
    } catch (e) {
        alert(`getUserMedia() error: ${e.name}`);
    }


    // new rtc connection for pc1
    pc1 = new RTCPeerConnection(pcConfig);
    console.log('Created local peer connection object pc1');
    pc1StateDiv.textContent = pc1.signalingState || pc1.readyState;
    pc1.addEventListener('signalingstatechange', e => stateCallback1());

    pc1IceStateDiv.textContent = pc1.iceConnectionState;
    pc1.addEventListener('iceconnectionstatechange', e => onIceStateChange(pc1, e));
    pc1.addEventListener('connectionstatechange', e => connStateCallback1());
    pc1.addEventListener('icecandidate', e => onIceCandidate(pc1, e));

    // new rtc connection for pc2
    pc2 = new RTCPeerConnection(pcConfig);
    console.log('Created remote peer connection object pc2');
    pc2StateDiv.textContent = pc2.signalingState || pc2.readyState;
    pc2.addEventListener('signalingstatechange', e => stateCallback2());

    pc2IceStateDiv.textContent = pc2.iceConnectionState;
    pc2.addEventListener('iceconnectionstatechange', e => onIceStateChange(pc2, e));
    pc2.addEventListener('connectionstatechange', e => connStateCallback2());
    pc2.addEventListener('icecandidate', e => onIceCandidate(pc2, e));
    pc2.addEventListener('track', gotRemoteStream);

}

function getSelectedSdpSemantics() {
    const sdpSemanticsSelect = document.querySelector('#sdpSemantics');
    const option = sdpSemanticsSelect.options[sdpSemanticsSelect.selectedIndex];
    return option.value === '' ? {} : { sdpSemantics: option.value };
}

async function call() {

    callButton.disabled = true;
    hangupButton.disabled = false;
    console.log('Starting call');
    startTime = window.performance.now();
    const videoTracks = localStream.getVideoTracks();
    const audioTracks = localStream.getAudioTracks();
    if (videoTracks.length > 0) {
        console.log(`Using video device: ${videoTracks[0].label}`);
    }
    if (audioTracks.length > 0) {
        console.log(`Using audio device: ${audioTracks[0].label}`);
    }
       

    localStream.getTracks().forEach(track => pc1.addTrack(track, localStream));
    console.log('Added local stream to pc1');

    try {
        console.log('pc1 createOffer start');
        const offer = await pc1.createOffer(offerOptions);
        await onCreateOfferSuccess(offer);
    } catch (e) {
        onCreateSessionDescriptionError(e);
    }
}

function stateCallback1() {
    let state;
    if (pc1) {
        state = pc1.signalingState || pc1.readyState;
        console.log(`pc1 state change callback, state: ${state}`);
        pc1StateDiv.textContent += ` => ${state}`;
    }
}

function stateCallback2() {
    let state;
    if (pc2) {
        state = pc2.signalingState || pc2.readyState;
        console.log(`pc2 state change callback, state: ${state}`);
        pc2StateDiv.textContent += ` => ${state}`;
    }
}



function onCreateSessionDescriptionError(error) {
    console.log(`Failed to create session description: ${error.toString()}`);
}

async function onCreateOfferSuccess(desc) {
    console.log('pc1 setLocalDescription start');
    try {
        await pc1.setLocalDescription(desc, function () {
            connectionHub.invoke("SendRTCMessage", JSON.stringify({ 'offer': desc })).catch(function (err) {
                return console.error(err.toString());
            });
        });
        onSetLocalSuccess(pc1);
    } catch (e) {
        onSetSessionDescriptionError();
    }
}

async function onCreateAnswerSuccess(desc) {
    console.log('pc2 setLocalDescription start');
    try {
        await pc2.setLocalDescription(desc, function () {

        connectionHub.invoke("SendRTCMessage", JSON.stringify({ 'answer': desc })).catch(function (err) {
            return console.error(err.toString());
        });

        });
        onSetLocalSuccess(pc2);
    } catch (e) {
        onSetSessionDescriptionError(e);
    }

}

function onSetLocalSuccess(pc) {
    console.log(`${getName(pc)} setLocalDescription complete`);
}

function onSetRemoteSuccess(pc) {
    console.log(`${getName(pc)} setRemoteDescription complete`);
}

function onSetSessionDescriptionError(error) {
    console.log(`Failed to set session description: ${error.toString()}`);
}

function gotRemoteStream(e) {

    console.log('gotRemoteStream', e.track, e.streams[0]);

    // reset srcObject to work around minor bugs in Chrome and Edge.
    remoteVideo.srcObject = null;
    remoteVideo.srcObject = e.streams[0];

}

async function onIceCandidate(peerConnection, event) {

    if (getName(peerConnection) == 'pc1') {
        connectionHub.invoke("SendRTCMessage", JSON.stringify({ 'onIce': { 'peerConnection': 'pc1', 'candidate': event.candidate } })).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        connectionHub.invoke("SendRTCMessage", JSON.stringify({ 'onIce': { 'peerConnection': 'pc2', 'candidate': event.candidate } })).catch(function (err) {
            return console.error(err.toString());
        });
    }
}

function onAddIceCandidateSuccess(pc) {
    console.log(`${getName(pc)} addIceCandidate success`);
}

function onAddIceCandidateError(pc, error) {
    console.log(`${getName(pc)} failed to add ICE Candidate: ${error.toString()}`);
}

function onIceStateChange(pc, event) {

    if (pc) {
        var iceState = pc.iceConnectionState;
        if (pc === pc1) {
            pc1IceStateDiv.textContent += ` => ${iceState}`;
        }
        else if (pc === pc2) {
            pc2IceStateDiv.textContent += ` => ${iceState}`;
        }

        console.log(`${getName(pc)} ICE state: ${pc.iceConnectionState}`);
        console.log('ICE state change event: ', event);
    }
}
function connStateCallback1() {
    if (pc1) {
        const { connectionState } = pc1;
        console.log(`pc1 connection state change callback, state: ${connectionState}`);
        pc1ConnStateDiv.textContent += ` => ${connectionState}`;
    }
}

function connStateCallback2() {
    if (pc2) {
        const { connectionState } = pc2;
        console.log(`pc2 connection state change callback, state: ${connectionState}`);
        pc2ConnStateDiv.textContent += ` => ${connectionState}`;
    }
}

//function upgrade() {
//    upgradeButton.disabled = true;
//    const videoSource = videoSelect.value;
//    navigator.mediaDevices
//        .getUserMedia({ video: { deviceId: videoSource ? { exact: videoSource } : undefined } })
//        .then(stream => {
//            const videoTracks = stream.getVideoTracks();
//            if (videoTracks.length > 0) {
//                console.log(`Using video device: ${videoTracks[0].label}`);
//            }
//            localStream.addTrack(videoTracks[0]);
//            localVideo.srcObject = null;
//            localVideo.srcObject = localStream;
//            pc1.addTrack(videoTracks[0], localStream);
//            return pc1.createOffer();
//        })
//        .then(offer => pc1.setLocalDescription(offer))
//        .then(() => {
//             connectionHub.invoke("SendRTCMessage", JSON.stringify({ 'offer': pc1.localDescription })).catch(function (err) {
//                return console.error(err.toString());
//            });
//        });
    
//        //.then(offer => pc1.setLocalDescription(offer))
//        //.then(() => pc2.setRemoteDescription(pc1.localDescription))
//        //.then(() => pc2.createAnswer())
//        //.then(answer => pc2.setLocalDescription(answer))
//        //.then(() => pc1.setRemoteDescription(pc2.localDescription));
//}

function hangup() {

    console.log('Ending call');
    pc1.close();
    pc2.close();

    pc1StateDiv.textContent += ` => ${pc1.signalingState}` || pc1.readyState;
    pc2StateDiv.textContent += ` => ${pc2.signalingState}` || pc2.readyState;
    pc1IceStateDiv.textContent += ` => ${pc1.iceConnectionState}`;
    pc2IceStateDiv.textContent += ` => ${pc2.iceConnectionState}`;

    pc1 = null;
    pc2 = null;

    const videoTracks = localStream.getVideoTracks();
    videoTracks.forEach(videoTrack => {
        videoTrack.stop();
        localStream.removeTrack(videoTrack);
    });

    const audioTracks = localStream.getAudioTracks();
    audioTracks.forEach(audioTracks => {
        audioTracks.stop();
        localStream.removeTrack(audioTracks);
    });

    localVideo.srcObject = null;
    localVideo.srcObject = localStream;

    hangupButton.disabled = true;
    startButton.disabled = false;
}

// WEBRTC get resouce
const videoElement = document.querySelector('video#localVideo');
const audioInputSelect = document.querySelector('select#audioSource');
const audioOutputSelect = document.querySelector('select#audioOutput');
const videoSelect = document.querySelector('select#videoSource');
const selectors = [audioInputSelect, audioOutputSelect, videoSelect];

audioOutputSelect.disabled = !('sinkId' in HTMLMediaElement.prototype);

function gotDevices(deviceInfos) {
    // Handles being called several times to update labels. Preserve values.
    const values = selectors.map(select => select.value);
    selectors.forEach(select => {
        while (select.firstChild) {
            select.removeChild(select.firstChild);
        }
    });
    for (let i = 0; i !== deviceInfos.length; ++i) {
        const deviceInfo = deviceInfos[i];
        const option = document.createElement('option');
        option.value = deviceInfo.deviceId;
        if (deviceInfo.kind === 'audioinput') {
            option.text = deviceInfo.label || `microphone ${audioInputSelect.length + 1}`;
            audioInputSelect.appendChild(option);
        } else if (deviceInfo.kind === 'audiooutput') {
            option.text = deviceInfo.label || `speaker ${audioOutputSelect.length + 1}`;
            audioOutputSelect.appendChild(option);
        } else if (deviceInfo.kind === 'videoinput') {
            option.text = deviceInfo.label || `camera ${videoSelect.length + 1}`;
            videoSelect.appendChild(option);
        } else {
            console.log('Some other kind of source/device: ', deviceInfo);
        }
    }
    selectors.forEach((select, selectorIndex) => {
        if (Array.prototype.slice.call(select.childNodes).some(n => n.value === values[selectorIndex])) {
            select.value = values[selectorIndex];
        }
    });
}

navigator.mediaDevices.enumerateDevices().then(gotDevices).catch(handleError);

// Attach audio output device to video element using device/sink ID.
function attachSinkId(element, sinkId) {
    if (typeof element.sinkId !== 'undefined') {
        element.setSinkId(sinkId)
            .then(() => {
                console.log(`Success, audio output device attached: ${sinkId}`);
            })
            .catch(error => {
                let errorMessage = error;
                if (error.name === 'SecurityError') {
                    errorMessage = `You need to use HTTPS for selecting audio output device: ${error}`;
                }
                console.error(errorMessage);
                // Jump back to first output device in the list as it's the default.
                audioOutputSelect.selectedIndex = 0;
            });
    } else {
        console.warn('Browser does not support output device selection.');
    }
}

function changeAudioDestination() {
    const audioDestination = audioOutputSelect.value;
    attachSinkId(videoElement, audioDestination);
}

function gotStream(stream) {
    window.stream = stream; // make stream available to console
    videoElement.srcObject = stream;
    localStream = stream;

    // Refresh button list in case labels have become available
    return navigator.mediaDevices.enumerateDevices();
}

function handleError(error) {
    console.log('navigator.MediaDevices.getUserMedia error: ', error.message, error.name);
}

function startGetStream() {
    if (window.stream) {
        window.stream.getTracks().forEach(track => {
            track.stop();
        });
    }
    const audioSource = audioInputSelect.value;
    const videoSource = videoSelect.value;
    const constraints = {
        audio: { deviceId: audioSource ? { exact: audioSource } : undefined },
        video:{ deviceId: videoSource ? { exact: videoSource } : undefined }
    };
    navigator.mediaDevices.getUserMedia(constraints).then(gotStream).then(gotDevices).catch(handleError);
}
