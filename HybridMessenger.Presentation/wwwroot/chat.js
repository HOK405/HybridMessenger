let connection;
let peerConnections = {};
let ChatID;
let myConnectionId;

async function startConnection(chatId, hubUrl, accessToken) {
    ChatID = chatId;

    connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
            accessTokenFactory: () => accessToken
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

    registerEventHandlers(chatId);

    await connection.start();
    await connection.invoke("JoinGroupAsync", chatId);
    myConnectionId = await connection.invoke("CreatePeerConnection", chatId);
}

function registerEventHandlers(chatId) {
    connection.on("Send", (message) => {
        console.log(message);
    });

    connection.on("ConnectPeer", (userId) => {
        if (userId !== myConnectionId && !peerConnections[userId]) {
            setupPeerConnection(userId, ChatID);
        }
    });

    connection.on("CallStarted", async (chatId) => {
        await startWebRtc(chatId);
    });

    connection.on("ReceiveOffer", async (chatId, callerId, offer) => {
        if (!peerConnections[callerId]) {
            setupPeerConnection(callerId, chatId);
        }

        const peerConnection = peerConnections[callerId];
        const offerObj = new RTCSessionDescription(JSON.parse(offer));
        await peerConnection.setRemoteDescription(offerObj);
        const answer = await peerConnection.createAnswer();
        await peerConnection.setLocalDescription(answer);
        connection.invoke("SendAnswer", callerId, JSON.stringify(answer));

        processIceCandidateQueue(callerId);
    });

    connection.on("ReceiveAnswer", async (callerId, answerJson) => {
        const peerConnection = peerConnections[callerId];
        try {
            const answerObj = new RTCSessionDescription(JSON.parse(answerJson));
            if (peerConnection.signalingState === "have-local-offer") {
                await peerConnection.setRemoteDescription(answerObj);
                processIceCandidateQueue(callerId);
            } else {
                console.error("Received an answer when not expecting one:", peerConnection.signalingState);
            }
        } catch (error) {
            console.error("Error handling answer:", error);
        }
    });

    connection.on("ReceiveIceCandidate", async (callerId, candidateJson) => {
        if (!peerConnections[callerId]) {
            setupPeerConnection(callerId);
        }

        const peerConnection = peerConnections[callerId];
        try {
            const parsedJson = JSON.parse(candidateJson);
            const candidateObj = parsedJson.candidate;
            if (candidateObj && candidateObj.candidate && candidateObj.sdpMid != null && candidateObj.sdpMLineIndex != null) {
                const iceCandidate = new RTCIceCandidate({
                    candidate: candidateObj.candidate,
                    sdpMid: candidateObj.sdpMid,
                    sdpMLineIndex: candidateObj.sdpMLineIndex
                });

                if (peerConnection.remoteDescription) {
                    await peerConnection.addIceCandidate(iceCandidate);
                } else {
                    if (!peerConnection.iceCandidateQueue) {
                        peerConnection.iceCandidateQueue = [];
                    }
                    peerConnection.iceCandidateQueue.push(iceCandidate);
                }
            } else {
                console.error("Invalid ICE candidate received:", candidateObj);
            }
        } catch (error) {
            console.error("Error parsing or adding ICE candidate:", error);
        }
    });
}

async function startWebRtc(chatId) {
    const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
    displayVideo(stream, 'local');

    for (const [callerId, peerConnection] of Object.entries(peerConnections)) {
        if (callerId !== myConnectionId) {
            stream.getTracks().forEach(track => {
                peerConnection.addTrack(track, stream);
            });

            const offer = await peerConnection.createOffer();
            await peerConnection.setLocalDescription(offer);
            connection.invoke("SendOffer", chatId, callerId, JSON.stringify(offer));
        }
    }
}

function setupPeerConnection(callerId, chatId) {
    console.log("Setting up peer connection for caller: " + callerId);
    const peerConnection = new RTCPeerConnection({
        iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
    });
    peerConnections[callerId] = peerConnection;

    navigator.mediaDevices.getUserMedia({ video: true, audio: true }).then(stream => {
        stream.getTracks().forEach(track => {
            peerConnection.addTrack(track, stream);
        });
    });

    peerConnection.ontrack = event => {
        console.log("Received track from caller: " + callerId);
        displayVideo(event.streams[0], callerId);
    };

    peerConnection.onicecandidate = event => {
        if (event.candidate) {
            connection.invoke("SendIceCandidate", chatId, JSON.stringify({ candidate: event.candidate }));
        }
    };

    peerConnection.oniceconnectionstatechange = () => {
        if (peerConnection.iceConnectionState === 'disconnected') {
            console.log("Peer connection disconnected for caller: " + callerId);
            peerConnection.close();
            delete peerConnections[callerId];
        }
    };
}

function processIceCandidateQueue(callerId) {
    console.log("Adding the candidate to the queue...");
    const peerConnection = peerConnections[callerId];
    if (peerConnection && peerConnection.iceCandidateQueue) {
        while (peerConnection.iceCandidateQueue.length > 0) {
            const candidate = peerConnection.iceCandidateQueue.shift();
            peerConnection.addIceCandidate(candidate).catch(error => {
                console.error("Error adding ICE candidate from queue:", error);
            });
        }
    }
}

function displayVideo(stream, id) {
    let videoElement = document.getElementById(id === 'local' ? 'localVideo' : 'remoteVideo-' + id);
    if (!videoElement) {
        videoElement = document.createElement("video");
        videoElement.id = id === 'local' ? 'localVideo' : 'remoteVideo-' + id;
        document.getElementById('videoContainer').appendChild(videoElement);
    }
    videoElement.srcObject = stream;
    videoElement.autoplay = true;
    videoElement.controls = true;
    videoElement.playsInline = true;
    videoElement.muted = id === 'local' ? true : false;
}

async function endCall() {
    Object.values(peerConnections).forEach(pc => pc.close());
    peerConnections = {};

    const localVideo = document.getElementById('localVideo');
    if (localVideo && localVideo.srcObject) {
        const tracks = localVideo.srcObject.getTracks();
        tracks.forEach(track => {
            console.log("Stopping track: ", track.kind);
            track.stop();
        });
    }

    const videoContainer = document.getElementById('videoContainer');
    while (videoContainer.firstChild) {
        videoContainer.removeChild(videoContainer.firstChild);
    }

    if (connection) {
        await connection.stop();
    }
}

function scrollToBottom(id) {
    var element = document.getElementById(id);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}