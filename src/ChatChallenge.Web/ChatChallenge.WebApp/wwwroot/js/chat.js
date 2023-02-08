document.addEventListener('DOMContentLoaded', function () {

    // Start the connection.
    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/chat')
        .build();

    // Create a function that the hub can call to broadcast messages.
    connection.on('broadcastMessage', function (chatMessageObj) {
        // Html encode display name and message.
        var messageDateTime = new Date(chatMessageObj.messageDateTime).toISOString().slice(0, 19).replace(/-/g, "/").replace("T", " ");
        var encodedName = chatMessageObj.userName;
        var encodedMsg = chatMessageObj.messageText;

        // Add the message to the page.
        var liElement = document.createElement('li');
        liElement.innerHTML = '<strong>' + messageDateTime + ' | ' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg;
        document.getElementById('messagesList').appendChild(liElement);
    });

    connection.on('loadChatHistory', function (chatHistory) {
        var messageList = document.getElementById('messagesList');
        chatHistory.map(function (chatMessage) {
            var messageDateTime = new Date(chatMessage.messageDateTime).toISOString().slice(0, 19).replace(/-/g, "/").replace("T", " ");
            var encodedName = chatMessage.userName;
            var encodedMsg = chatMessage.messageText;
            var liElement = document.createElement('li');
            liElement.innerHTML = '<strong>' + messageDateTime + ' | ' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg;
            messageList.appendChild(liElement);
        });
        document.getElementById('divLoading').setAttribute("style", "display:none");
    });



    // Transport fallback functionality is now built into start.
    connection.start()
        .then(function () {
            console.log('connection started');

            connection.invoke('retriveChatHistory');

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