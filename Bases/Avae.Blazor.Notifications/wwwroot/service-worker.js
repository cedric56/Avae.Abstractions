self.addEventListener('install', function (event) {
    console.log('Service Worker installed');
    event.waitUntil(self.skipWaiting());
});

self.addEventListener('activate', function (event) {
    console.log('Service Worker activated');
    event.waitUntil(self.clients.claim());
});

self.addEventListener('notificationclose', function (event) {
    console.log('Close');
    const notification = event.notification;
    const data = notification.data || {};
    event.waitUntil(
        (async function () {
            // Send click to Blazor
            await sendToBlazor({
                data: data,
                type: 'HandleNotificationClose'
            });
        })()
    );
});

self.addEventListener('notificationclick', function (event) {
    const notification = event.notification;
    const action = event.action;
    const data = notification.data || {};
    notification.close();

    event.waitUntil(
        (async function () {
            // Handle action with input (reply)
            if (action && data.replyActionTag === action) {
                // The reply will be handled via notificationreply event
                return;
            }

            // Send click to Blazor
            await sendToBlazor({
                //notification: event.notification,
                action: event.action,
                data: data,
                type: 'HandleNotificationClick'
            });
        })()
    );
});

// Handle notification reply (input)
self.addEventListener('notificationreply', function (event) {
    const notification = event.notification;
    const action = event.action;
    const reply = event.reply;
    const data = notification.data || {};

    notification.close();
    event.waitUntil(
        (async function () {
            await sendToBlazor({
                data: data,
                type: 'HandleNotificationReply',
                reply: reply
            });
        })()
    );
});

// Helper to send data to Blazor
async function sendToBlazor(message) {
    try {
        const allClients = await clients.matchAll({
            type: 'window',
            includeUncontrolled: true
        });

        for (const client of allClients) {
            try {
                await client.postMessage(message);
            } catch (e) {
                console.error(e);
            }
        }
    } catch (error) {
        console.error('Error sending to Blazor:', error);
    }
}