export async function registrations(dotNetRef) {
    navigator.serviceWorker.addEventListener('message', function (event) {
        if (event.data && event.data.type === 'HandleNotificationClose') {
            dotNetRef.invokeMethodAsync('OnClose', JSON.stringify(event.data));
        }
        if (event.data && event.data.type === 'HandleNotificationClick') {
            dotNetRef.invokeMethodAsync('OnClick', JSON.stringify(event.data));
        }
        if (event.data && event.data.type === 'HandleNotificationReply') {
            dotNetRef.invokeMethodAsync('OnReply', JSON.stringify(event.data), event.data.reply);
        }
    });
}