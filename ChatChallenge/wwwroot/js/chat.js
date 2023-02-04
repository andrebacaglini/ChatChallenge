﻿document.addEventListener('DOMContentLoaded', function () {
    
    // Start the connection.
    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/chat')
        .build();

    // Create a function that the hub can call to broadcast messages.
    connection.on('broadcastMessage', function (chatMessageObj) {
        console.log(chatMessageObj);
        // Html encode display name and message.
        var encodedName = chatMessageObj.userName;
        var encodedMsg = chatMessageObj.chatMessageText;
        // Add the message to the page.
        var liElement = document.createElement('li');
        liElement.innerHTML = '<strong>' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg;
        document.getElementById('messagesList').appendChild(liElement);
    });

    // Transport fallback functionality is now built into start.
    connection.start()
        .then(function () {
            console.log('connection started');
            document.getElementById('sendButton').addEventListener('click', function (event) {
                // Call the Send method on the hub.
                connection.invoke('send', userName.innerHTML, messageInput.value);

                // Clear text box and reset focus for next comment.
                messageInput.value = '';
                messageInput.focus();
                event.preventDefault();
            });
        })
        .catch(error => {
            console.error(error.message);
        });
});