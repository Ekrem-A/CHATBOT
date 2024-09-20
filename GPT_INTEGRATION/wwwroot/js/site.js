//// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
//// for details on configuring this project to bundle and minify static web assets.

//// Write your JavaScript code.


//const chatMessages = document.querySelector('.chat-messages');
//const messageInput = document.getElementById('message-input');
//const sendButton = document.getElementById('send-button');

//sendButton.addEventListener('click', () => {
//    const message = messageInput.value;
//    if (message.trim() !== '') {
//        appendMessage('user', message);
//        messageInput.value = '';

//        // Burada mesajı backend'e gönderip bot cevabını almanız gerekir
//        // Şimdilik örnek bir cevap ekliyoruz:
//        setTimeout(() => {
//            appendMessage('bot', 'Bu bir örnek bot cevabıdır.');
//        }, 1000);
//    }
//});

//function appendMessage(sender, message) {
//    const messageElement = document.createElement('li');
//    messageElement.classList.add(`${sender}-message`);

//    const messageContent = document.createElement('p');
//    messageContent.textContent = message;

//    messageElement.appendChild(messageContent);
//    chatMessages.appendChild(messageElement);

//    // Yeni mesaj geldiğinde aşağı kaydır
//    chatMessages.scrollTop = chatMessages.scrollHeight;
//}

//sendButton.addEventListener('click', () => {
//    const message = messageInput.value;
//    if (message.trim() !== '') {
//        appendMessage('user', message);
//        messageInput.value
//            = '';

//        // AJAX isteği ile mesajı controller'a gönder
//        fetch('/Image/SendMessage', {  // Controller ve action adını doğru şekilde ayarlayın
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/x-www-form-urlencoded'
//            },
//            body: `message=${encodeURIComponent(message)}`
//        })
//            .then(response => response.json())
//            .then(data
//                => {
//                appendMessage('bot',
//                    data.message);
//            })
//            .catch(error => {
//                console.error('Mesaj gönderme hatası:', error);
//                // Hata durumunda kullanıcıya uygun bir mesaj gösterilebilir
//            });
//    }
//});
